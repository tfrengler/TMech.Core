using NUnit.Framework;
using OpenQA.Selenium.Chrome;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace Tests
{
    /// <summary>
    /// Contains all the id's of the elements on the test html-page.
    /// </summary>
    public static class JSElements
    {
        public const string Context1 = "Context1";
        public const string Context2 = "Context2";
        public const string Context3 = "Context3";

        public const string Context1Div1 = "Context1-Div1-Id";
        public const string Context1Div2 = "Context1-Div2-Id";
        public const string Context1Div3 = "Context1-Div3-Id";

        public const string Context2Button = "Context2-Button-Id";
        public const string Context2InputText = "Context2-InputText-Id";
        public const string Context2InputNumber = "Context2-InputNumber-Id";
        public const string Context2Checkbox = "Context2-Checkbox-Id";
        public const string Context2File = "Context2-File-Id";
        public const string Context2DateTime = "Context2-DateTime-Id";
        public const string Context2DateTimeLocal = "Context2-DateTimeLocal-Id";

        public const string Context2InputTextWrapper = "Context2-InputText-Wrapper";
        public const string Context2InputNumberWrapper = "Context2-InputNumber-Wrapper";
        public const string Context2CheckboxWrapper = "Context2-Checkbox-Wrapper";
        public const string Context2FileWrapper = "Context2-File-Wrapper";
        public const string Context2DateTimeWrapper = "Context2-DateTime-Wrapper";
        public const string Context2DateTimeLocalWrapper = "Context2-DateTimeLocal-Wrapper";

        public const string Context2DropdownContainer = "Context2-Select";
        public const string Context2Select = "Context2-Select-Id";
        public const string Context2OptionNIL1 = "Context2-Option-NIL1";
        public const string Context2Option1 = "Context2-Option1-Id";
        public const string Context2Option2 = "Context2-Option2-Id";
        public const string Context2Option3 = "Context2-Option3-Id";

        public const string Context2DropdownMulti = "Context2-SelectMulti";
        public const string Context2SelectMulti = "Context2-SelectMulti-Id";
        public const string Context2OptionNIL2 = "Context2-Option-NIL2";
        public const string Context2Option4 = "Context2-Option4-Id";
        public const string Context2Option5 = "Context2-Option5-Id";
        public const string Context2Option6 = "Context2-Option6-Id";

        public const string Context2Textarea = "Context2-Textarea-Id";
        public const string Context2Hyperlink = "Context2-Hyperlink-Id";
        public const string Context2Image = "Context2-Image-Id";
        public const string Context2RadioButtons = "Context2-RadioButtons-Id";

        public const string Context2Radio1 = "Context2-Radio1-Id";
        public const string Context2Radio2 = "Context2-Radio2-Id";
        public const string Context2Radio3 = "Context2-Radio3-Id";

        public const string Context3Div1 = "Context3-Div1-Id";
        public const string Context3Div2 = "Context3-Div2-Id";
        public const string Context3Div3 = "Context3-Div3-Id";

        public const string Context3Span1 = "Context3-Span1";
        public const string Context3Span2 = "Context3-Span2";
        public const string Context3Div3Span1 = "Context3-Div3-Span1";
        public const string Context3Div3Span2 = "Context3-Div3-Span2";

        public const string StaleContext = "StaleContext";
        public const string StaleContextChild1 = "StaleChild1";
        public const string StaleContextChild2 = "StaleChild2";
        public const string StaleContextChild3 = "StaleChild3";
        public const string StaleContextChild4 = "StaleChild4";
    }

    public sealed class ChromeContext : IDisposable
    {
        public ChromeDriver ChromeDriver { get; }

        public ChromeContext()
        {
            var DriverService = ChromeDriverService.CreateDefaultService(@"C:\Temp\Chromium\chromedriver.exe");
            DriverService.Start();

            var Options = new ChromeOptions();
            Options.AddArgument("--window-size=2560,1440");
            Options.AddArgument("--headless=new");
            Options.BinaryLocation = @"C:\Temp\Chromium\chrome.exe";
            var Webdriver = new ChromeDriver(DriverService, Options);
            Thread.Sleep(1000);
            Webdriver.Manage().Window.Maximize();

            string? ExecutingLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            Debug.Assert(ExecutingLocation is not null);
            string TestPageURL = "file:///" + new FileInfo(ExecutingLocation + @"\TestPage.html").FullName;

            ChromeDriver = Webdriver;
            Webdriver.Navigate().GoToUrl(TestPageURL);
        }

        #region JS FUNCTIONS

        public void JsCopyLastChildOfParentAndAppend(string idOfElement, int timeoutInMS = 0)
        {
            StringBuilder JsFragments = new StringBuilder();
            if (timeoutInMS > 0) JsFragments.Append($"arguments[arguments.length - 1]();await Wait({timeoutInMS});");

            JsFragments.Append($"CopyLastChildOfParentAndAppend(document.querySelector('#{idOfElement}'));");
            ChromeDriver.ExecuteAsyncScript(JsFragments.ToString());
        }

        public void JsRemoveLastChildOfParent(string idOfElement, int timeoutInMS = 0)
        {
            StringBuilder JsFragments = new StringBuilder();
            JsFragments.Append($"arguments[arguments.length - 1]();");
            if (timeoutInMS > 0) JsFragments.Append($"await Wait({timeoutInMS});");

            JsFragments.Append($"RemoveLastChildOfParent(document.querySelector('#{idOfElement}'));");
            ChromeDriver.ExecuteAsyncScript(JsFragments.ToString());
        }

        public void JsHideElement(string idOfElement, int timeoutInMS = 0)
        {
            StringBuilder JsFragments = new StringBuilder();
            JsFragments.Append($"arguments[arguments.length - 1]();");
            if (timeoutInMS > 0) JsFragments.Append($"await Wait({timeoutInMS});");

            JsFragments.Append($"HideElement(document.querySelector('#{idOfElement}'));");
            ChromeDriver.ExecuteAsyncScript(JsFragments.ToString());
        }

        public void JsShowElement(string idOfElement, int timeoutInMS = 0)
        {
            StringBuilder JsFragments = new StringBuilder();
            JsFragments.Append($"arguments[arguments.length - 1]();");
            if (timeoutInMS > 0) JsFragments.Append($"await Wait({timeoutInMS});");

            JsFragments.Append($"ShowElement(document.querySelector('#{idOfElement}'));");
            ChromeDriver.ExecuteAsyncScript(JsFragments.ToString());
        }

        public void JsEnableElement(string idOfElement, int timeoutInMS = 0)
        {
            StringBuilder JsFragments = new StringBuilder();
            JsFragments.Append($"arguments[arguments.length - 1]();");
            if (timeoutInMS > 0) JsFragments.Append($"await Wait({timeoutInMS});");

            JsFragments.Append($"Enable(document.querySelector('#{idOfElement}'));");
            ChromeDriver.ExecuteAsyncScript(JsFragments.ToString());
        }

        public void JsDisableElement(string idOfElement, int timeoutInMS = 0)
        {
            StringBuilder JsFragments = new StringBuilder();
            JsFragments.Append($"arguments[arguments.length - 1]();");
            if (timeoutInMS > 0) JsFragments.Append($"await Wait({timeoutInMS});");

            JsFragments.Append($"Disable(document.querySelector('#{idOfElement}'));");
            ChromeDriver.ExecuteAsyncScript(JsFragments.ToString());
        }

        public void JsSelectElement(string idOfElement, int timeoutInMS = 0)
        {
            StringBuilder JsFragments = new StringBuilder();
            JsFragments.Append($"arguments[arguments.length - 1]();");
            if (timeoutInMS > 0) JsFragments.Append($"await Wait({timeoutInMS});");

            JsFragments.Append($"Select(document.querySelector('#{idOfElement}'));");
            ChromeDriver.ExecuteAsyncScript(JsFragments.ToString());
        }

        public void JsDeselectElement(string idOfElement, int timeoutInMS = 0)
        {
            StringBuilder JsFragments = new StringBuilder();
            JsFragments.Append($"arguments[arguments.length - 1]();");
            if (timeoutInMS > 0) JsFragments.Append($"await Wait({timeoutInMS});");

            JsFragments.Append($"Deselect(document.querySelector('#{idOfElement}'));");
            ChromeDriver.ExecuteAsyncScript(JsFragments.ToString());
        }

        public void JsChangeElementAttribute(string idOfElement, string attributeName, string attributeValue, int timeoutInMS = 0)
        {
            StringBuilder JsFragments = new StringBuilder();
            JsFragments.Append($"arguments[arguments.length - 1]();");
            if (timeoutInMS > 0) JsFragments.Append($"await Wait({timeoutInMS});");

            JsFragments.Append($"ChangeAttribute(document.querySelector('#{idOfElement}'), '{attributeName}', '{attributeValue}');");
            ChromeDriver.ExecuteAsyncScript(JsFragments.ToString());
        }

        public void JsChangeElementText(string idOfElement, string text, int timeoutInMS = 0)
        {
            StringBuilder JsFragments = new StringBuilder();
            JsFragments.Append($"arguments[arguments.length - 1]();");
            if (timeoutInMS > 0) JsFragments.Append($"await Wait({timeoutInMS});");

            JsFragments.Append($"ChangeText(document.querySelector('#{idOfElement}'), '{text}');");
            ChromeDriver.ExecuteAsyncScript(JsFragments.ToString());
        }

        public void JsKillAndReRenderStaleContext(int timeoutInMS, string newText)
        {
            string Script = $"arguments[arguments.length - 1]();KillAndReRenderStaleContext({timeoutInMS}, '{newText}');";
            ChromeDriver.ExecuteAsyncScript(Script);
        }

        #endregion

        #region TEARDOWN

        private bool IsDisposed = false;

        public void Dispose()
        {
            if (IsDisposed) return;
            IsDisposed = true;
            GC.SuppressFinalize(this);
            ChromeDriver?.Dispose();
        }

        ~ChromeContext()
        {
            ChromeDriver?.Dispose();
        }

        #endregion
    }
}

[SetUpFixture]
public class GlobalSetup
{
    public static TimeSpan DefaultFetchContextTimeout { get; } = TimeSpan.FromSeconds(5.0d);
    public static int FetchContextTimeoutMinus1Sec { get; } = 4000;
    public static FileInfo ChromeLocation = new FileInfo(@"C:\Temp\Chromium\chrome.exe");
    public static FileInfo ChromeDriverLocation = new FileInfo(@"C:\Temp\Chromium\chromedriver.exe");

    [OneTimeSetUp]
    public void BeforeAll()
    {
        Trace.Listeners.Add(new ConsoleTraceListener());
    }

    [OneTimeTearDown]
    public void AfterAll()
    {
        Trace.Flush();

        var AllProcesses = Process.GetProcesses();
        var BrowserDriverProcessNames = new string[] { "chromedriver", "geckodriver", "msedgedriver" };

        foreach(var CurrentProcess in AllProcesses)
        {
            if (Array.IndexOf(BrowserDriverProcessNames, CurrentProcess.ProcessName) > -1)
                CurrentProcess.Kill(true);
        }
    }
}