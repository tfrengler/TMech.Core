using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;

namespace TMech.Core.Utils
{
    public enum Browser
    {
        EDGE, FIREFOX, CHROME
    }

    /// <summary>
    /// A webdriver utility that manages a limited pool of webdrivers. Supports creating new instances or reusing the existing ones.
    /// </summary>
    public sealed class WebdriverPool : IDisposable
    {
        private readonly BlockingCollection<IWebDriver> Instances;
        private bool Disposed;
        private bool Started;
        private DriverService WebdriverService;

        /// <summary>Checks whether the service has been started and is running.</summary>
        public bool IsRunning { get => WebdriverService.IsRunning; }
        public bool ReuseInstances { get; }
        public Browser Browser { get; }
        /// <summary>Returns the URL and port that the underlying webdriver service is running on. Will return null if <see cref="Start"/> hasn't been called yet.</summary>
        public Uri? DriverURL { get; private set; }
        public int MaxInstances { get; }
        public DirectoryInfo WebdriverLocation { get; }

        public Func<Uri,IWebDriver> ChromeDriverBuilder { private get; init; }
        public Func<Uri,IWebDriver> FirefoxDriverBuilder { private get; init; }
        public Func<Uri,IWebDriver> EdgeDriverBuilder { private get; init; }

        /// <summary>
        /// Allows you to override the location of where the Chrome executable is located. Primarily meant for being able to swap using Chrome for using Chromium instead.<br/>
        /// Must be called before <see cref="Start"/> otherwise it will have no effect on the initial batch of instances that are created! If called after then it will do nothing.
        /// </summary>
        public DirectoryInfo? ChromeBinaryLocation
        {
            get { return _ChromeBinaryLocation; }
            set { if (!Started) _ChromeBinaryLocation = value ?? throw new ArgumentNullException(nameof(value)); }
        }
        private DirectoryInfo? _ChromeBinaryLocation;

        public WebdriverPool(Browser browser, int maxInstances, DirectoryInfo webdriverLocation, bool reuseInstances = false)
        {
            if (webdriverLocation is null) throw new ArgumentNullException(nameof(webdriverLocation));

            MaxInstances = maxInstances;
            Browser = browser;
            ReuseInstances = reuseInstances;
            Instances = new BlockingCollection<IWebDriver>(maxInstances);
        }

        /// <summary>
        /// Starts the driver service, and creates webdriver instances up until the <see cref="MaxInstances"/>-limit.<br/>
        /// This operation is idempotent and subsequent calls beyond the first will do nothing and throw no exceptions.
        /// </summary>
        public void Start()
        {
            if (Started) return;
            Started = true;

            WebdriverService = Browser switch
            {
                Browser.CHROME => ChromeDriverService.CreateDefaultService(WebdriverLocation.FullName),
                Browser.FIREFOX => FirefoxDriverService.CreateDefaultService(WebdriverLocation.FullName),
                Browser.EDGE => EdgeDriverService.CreateDefaultService(WebdriverLocation.FullName),
                _ => throw new NotImplementedException($"{nameof(Browser)} is not an expected value: " + (int)Browser)
            };

            WebdriverService.SuppressInitialDiagnosticInformation = true;
            WebdriverService.Start();
            DriverURL = WebdriverService.ServiceUrl;

            for (int Index = MaxInstances; Index > 0; Index--)
            {
                CreateAndAddInstance();
            }
        }

        /// <summary>
        /// Requests a webdriver from the pool. Will block the caller until a webdriver instance is free, or throw an exception if the timeout is reached.
        /// </summary>
        /// <exception cref="TimeoutException"></exception>
        public IWebDriver RequestInstance(TimeSpan timeout)
        {
            if (!Started) throw new InvalidOperationException("The webdriver pool has not been started. Make sure to call Start() first before requesting instances");

            if (Instances.TryTake(out IWebDriver? ReturnData, timeout))
            {
                Debug.Assert(ReturnData is not null, "The webdriver pool should not contain null references");
                return ReturnData;
            }

            throw new TimeoutException($"Timed out trying to acquire a webdriver instance from the pool ({timeout})");
        }

        /// <summary>
        /// Releases a webdriver back to the pool. Depending on <see cref="ReuseInstances"/> this will either put the webdriver back in the pool or cause a new instance to be created.
        /// </summary>
        public void Release(IWebDriver webdriverInstance)
        {
            if (Disposed) throw new ObjectDisposedException(nameof(WebdriverPool));
            if (!Started) throw new InvalidOperationException("The webdriver pool has not been started. Make sure to call Start() first before releasing instances");
            if (webdriverInstance is null) throw new ArgumentNullException(nameof(webdriverInstance), "Null references are not be allowed to be released back into the webdriver pool");

            if (ReuseInstances)
            {
                Instances.Add(ResetInstanceToDefaultState(webdriverInstance));
                return;
            }

            webdriverInstance.Quit();
            CreateAndAddInstance();
        }

        /// <summary>
        /// Resets the webdriver to a default state from which it can be reused.
        /// </summary>
        /// <returns>
        /// The webdriver in a default state, meaning:
        /// <list type="bullet">
        /// <item>Any open alerts are dismissed</item>
        /// <item>All open tabs beyond the first are closed</item>
        /// <item>All cookies are removed</item>
        /// <item>Localstorage is cleared</item>
        /// </list>
        /// </returns>
        public static IWebDriver ResetInstanceToDefaultState(IWebDriver webdriverInstance)
        {
            try
            {
                IAlert UnhandledAlert = webdriverInstance.SwitchTo().Alert();
                UnhandledAlert.Dismiss();
            }
            catch (NoAlertPresentException) { }

            if (webdriverInstance.WindowHandles.Count > 1)
            {
                while (webdriverInstance.WindowHandles.Count > 0)
                {
                    int LastTabIndex = webdriverInstance.WindowHandles.Count - 1;
                    webdriverInstance.SwitchTo().Window(webdriverInstance.WindowHandles[LastTabIndex]);
                    webdriverInstance.Close();
                }
            }

            webdriverInstance.Manage().Cookies.DeleteAllCookies();
            if (webdriverInstance is IJavaScriptExecutor JavascriptExecutor)
            {
                if ((bool?)JavascriptExecutor.ExecuteScript("return !!window.localStorage") == true)
                {
                    try
                    {
                        JavascriptExecutor.ExecuteScript("window.localStorage.clear();");
                    }
                    catch (WebDriverException) { }
                }
            }

            return webdriverInstance;
        }

        private void CreateAndAddInstance()
        {
            switch (Browser)
            {
                case Browser.CHROME:
                    {
                        var ChromeOptions = new ChromeOptions();

                        if (_ChromeBinaryLocation is not null)
                        {
                            ChromeOptions.BinaryLocation = Path.Combine(_ChromeBinaryLocation.FullName, "chrome.exe");
                        }

                        ChromeOptions.AddArgument("--headless=new");
                        ChromeOptions.AddArgument("--window-size=2560,1440");
                        ChromeOptions.AddArgument("--no-sandbox");
                        ChromeOptions.AddArgument("--disable-gpu");
                        ChromeOptions.AddArgument("--disable-web-security");
                        ChromeOptions.AddArgument("--disable-software-rasterizer");
                        ChromeOptions.AddArgument("--disable-feature=WebAuthentication");
                        ChromeOptions.AddArgument("--disable-extensions");
                        ChromeOptions.AddArgument("--disable-features=IsolateOrigins,site-per-process");
                        ChromeOptions.AddArgument("--disable-site-isolation-trials");

                        ChromeOptions.Proxy = new Proxy()
                        {
                            IsAutoDetect = false,
                            Kind = ProxyKind.Direct
                        };

                        Instances.Add(new RemoteWebDriver(DriverURL, ChromeOptions));
                        break;
                    }

                default:
                    throw new NotImplementedException("The webdriver pool does not support this browser: " + Enum.GetName(typeof(Browser), Browser));
            }
        }

        public void Dispose()
        {
            if (Disposed) return;

            Instances?.Dispose();
            WebdriverService?.Dispose();

            Disposed = true;
            GC.SuppressFinalize(this);
        }

        ~WebdriverPool()
        {
            Instances?.Dispose();
            WebdriverService?.Dispose();
        }
    }
}
