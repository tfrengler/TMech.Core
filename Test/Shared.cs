using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using NUnit.Framework;
using OpenQA.Selenium.Chrome;

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

        public const string Context2Dropdown = "Context2-Dropdown-Id";
        public const string Context2Select = "Context2-Select-Id";

        public const string Context2Option1 = "Context2-Option1-Id";
        public const string Context2Option2 = "Context2-Option2-Id";
        public const string Context2Option3 = "Context2-Option3-Id";

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
    }

    public static class Shared
    {
        public static DirectoryInfo BrowserDriverFolder { get; set; }

        public static ChromeDriver SetUpWebdriverAndGoToTestPage()
        {
            var DriverService = ChromeDriverService.CreateDefaultService(BrowserDriverFolder.FullName);
            DriverService.Start();

            var Options = new ChromeOptions();
            Options.AddArguments(new string[] { "--headless=new", "--window-size=2560,1440" });
            var Webdriver = new ChromeDriver(DriverService, Options);
            Webdriver.Manage().Window.Maximize();

            string ExecutingLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string TestPageURL = "file:///" + new FileInfo(ExecutingLocation + @"\TestPage.html").FullName;

            Webdriver.Navigate().GoToUrl(TestPageURL);
            return Webdriver;
        }
    }
}

[SetUpFixture]
public class GlobalSetup
{
    [OneTimeSetUp]
    public void BeforeAll()
    {
        Trace.Listeners.Add(new ConsoleTraceListener());

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Tests.Shared.BrowserDriverFolder = new DirectoryInfo(@"C:\Temp\Webdrivers");
        }
        else
        {
            Tests.Shared.BrowserDriverFolder = new DirectoryInfo(@"~/Temp/Webdrivers");
        }

        if (!Tests.Shared.BrowserDriverFolder.Exists)
            throw new Exception($"Error doing global setup. Webdriver folder does not exist: {Tests.Shared.BrowserDriverFolder.FullName}");
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