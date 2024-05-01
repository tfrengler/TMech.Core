using NUnit.Framework;
using System.Runtime.InteropServices;
using TMech.Core.Utils;

namespace Tests
{
    [NonParallelizable]
    [TestFixture]
    public class FirefoxProviderTests
    {
        #region INTERACTIONS

        private const string Category = "FirefoxProvider";
        [TestCase(Category = Category)]
        public void Install_Latest_Version_Win64()
        {
            using (var FirefoxProvider = new FirefoxProvider(GlobalSetup.FirefoxTempInstallLocation))
            {
                FirefoxProvider.DownloadLatestBrowserVersion(OSPlatform.Windows);
            }
        }

        [TestCase(Category = Category)]
        public void Install_Latest_Version_Linux64()
        {
            using (var FirefoxProvider = new FirefoxProvider(GlobalSetup.FirefoxTempInstallLocation))
            {
                FirefoxProvider.DownloadLatestBrowserVersion(OSPlatform.Linux);
            }
        }

        #endregion
    }
}
