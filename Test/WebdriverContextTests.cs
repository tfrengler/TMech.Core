using NUnit.Framework;
using OpenQA.Selenium.Chrome;
using System;
using System.IO;
using TMech.Utils;

namespace Tests
{
    [Parallelizable(ParallelScope.Children)]
    [TestFixture]
    public class WebdriverContextTests
    {
        private readonly Uri RemoteUri = new Uri("http://192.168.178.85:4444");
        private readonly DirectoryInfo DownloadsFolder = new System.IO.DirectoryInfo("C:/Temp");
        private System.Drawing.Size WindowSize = new System.Drawing.Size(1024, 768);

        // Remote, default settings, use Chrome
        // Local with driver, no binary, use Chrome
        // Local with browser, with binary, use Chrome
        // Local with browser, without binary, use Chrome
        // Remote, custom settings, use Chrome
        // Local with browser, without binary, custom settings use Chrome

        // Local with browser, no binary - CHROME
        // Local with browser, no binary - FIREFOX
        // Local with browser, no binary - EDGE
        // Fail when interacting with webdriver before initialization

        private const string Category = "WebdriverContext";
        [TestCase(Category = Category)]
        public void Remote_DefaultInitialize()
        {
            using var Context = WebdriverContext.CreateRemote(TMech.Browser.CHROME, RemoteUri);
            Context.Initialize(true);
        }

        [TestCase(Category = Category)]
        public void Remote_CustomInitialize()
        {
            using var Context = WebdriverContext.CreateRemote(TMech.Browser.CHROME, RemoteUri);
            Context.Initialize(true, WindowSize, new string[] { "--allow-file-access-from-files" }, DownloadsFolder);
        }

        [TestCase(Category = Category)]
        public void Local_WithDriver_NoBinary_DefaultInitialize()
        {
            var DriverService = ChromeDriverService.CreateDefaultService();
            using var Context = WebdriverContext.CreateLocal(DriverService);
            Context.Initialize(true);
        }

        [TestCase(Category = Category)]
        public void Local_WithBrowser_AndBinary_DefaultInitialize()
        {
            using var Context = WebdriverContext.CreateLocal(TMech.Browser.CHROME, GlobalSetup.ChromeLocation);
            Context.Initialize(true);
        }

        [TestCase(Category = Category)]
        public void Local_WithBrowser_NoBinary_CustomInitialize()
        {
            using var Context = WebdriverContext.CreateLocal(TMech.Browser.CHROME);
            Context.Initialize(true, WindowSize, new string[] { "--allow-file-access-from-files" }, DownloadsFolder);
        }

        [TestCase(Category = Category)]
        public void Failure_Usage_Before_Initialization()
        {
            using var Context = WebdriverContext.CreateLocal(TMech.Browser.CHROME);
            Assert.Throws<InvalidOperationException>(() => _ = Context.Webdriver);
        }

        #region BROWSER SPECIFIC

        [TestCase(Category = Category)]
        public void Local_Chrome_DefaultInitialize()
        {
            using var Context = WebdriverContext.CreateLocal(TMech.Browser.CHROME);
            Context.Initialize(true);
        }

        [TestCase(Category = Category)]
        public void Local_Firefox_DefaultInitialize()
        {
            using var Context = WebdriverContext.CreateLocal(TMech.Browser.FIREFOX);
            Context.Initialize(true);
        }

        [TestCase(Category = Category)]
        public void Local_Edge_DefaultInitialize()
        {
            using var Context = WebdriverContext.CreateLocal(TMech.Browser.EDGE);
            Context.Initialize(true);
        }

        #endregion
    }
}
