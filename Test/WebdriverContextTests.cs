using NUnit.Framework;
using System;
using System.IO;
using TMech.Utils;

namespace Tests
{
    [Parallelizable(ParallelScope.Children)]
    [TestFixture]
    public class WebdriverContextTests
    {
        private Uri RemoteUri = new Uri("http://192.168.178.85:4444");
        private DirectoryInfo DownloadsFolder = new System.IO.DirectoryInfo("C:/Temp");
        private System.Drawing.Size WindowSize = new System.Drawing.Size(1024, 768);

        #region REMOTE

        private const string Category_Remote = "WebdriverContext = Remote";
        [TestCase(Category = Category_Remote)]
        public void Remote_Chrome_CustomSettings()
        {
            var Context = WebdriverContext.CreateRemote(TMech.Browser.CHROME, RemoteUri);
            Context.Initialize(true, WindowSize, new string[] { "--allow-file-access-from-files" }, DownloadsFolder);
            Context?.Dispose();
        }

        [TestCase(Category = Category_Remote)]
        public void Remote_Chrome_DefaultSettings()
        {
            var Context = WebdriverContext.CreateRemote(TMech.Browser.CHROME, RemoteUri);
            Context.Initialize(true);
            Context?.Dispose();
        }

        [TestCase(Category = Category_Remote)]
        public void Remote_Firefox_CustomSettings()
        {
            var Context = WebdriverContext.CreateRemote(TMech.Browser.FIREFOX, RemoteUri);
            Context.Initialize(true, WindowSize, new string[] { "-private-window" }, DownloadsFolder);
            Context?.Dispose();
        }

        [TestCase(Category = Category_Remote)]
        public void Remote_Firefox_DefaultSettings()
        {
            var Context = WebdriverContext.CreateRemote(TMech.Browser.FIREFOX, RemoteUri);
            Context.Initialize(true);
            Context?.Dispose();
        }

        #endregion
    }
}
