using NUnit.Framework;
using System;
using System.IO;
using TMech.Core.Utils;

namespace Tests
{
    [NonParallelizable]
    [TestFixture]
    public class FirefoxProviderTests
    {
        [SetUp]
        public void ClearInstallFolder()
        {
            foreach (FileInfo CurrentFile in GlobalSetup.FirefoxTempInstallLocation.EnumerateFiles())
            {
                CurrentFile.Delete();
            }
            foreach (DirectoryInfo CurrentDirectory in GlobalSetup.FirefoxTempInstallLocation.EnumerateDirectories())
            {
                CurrentDirectory.Delete(true);
            }
        }

        #region INSTALLED

        private const string Category_Installed = "FirefoxProvider = Installed";

        [TestCase(Category = Category_Installed)]
        public void Installed_Version_Browser_Win64()
        {
            using (var FirefoxProvider = new FirefoxProvider(GlobalSetup.FirefoxTempInstallLocation))
            {
                string CurrentVersion = FirefoxProvider.GetCurrentInstalledBrowserVersion();
                Assert.That(CurrentVersion, Is.Empty);

                bool Updated = FirefoxProvider.DownloadLatestBrowserVersion(TMech.Platform.Win64);
                Assert.That(Updated, Is.True);

                CurrentVersion = FirefoxProvider.GetCurrentInstalledBrowserVersion();
                Console.WriteLine("Current version: " + CurrentVersion);
                Assert.That(CurrentVersion, Is.Not.Empty);

                Updated = FirefoxProvider.DownloadLatestBrowserVersion(TMech.Platform.Win64);
                Assert.That(Updated, Is.False);
            }
        }

        [TestCase(Category = Category_Installed)]
        public void Installed_Version_Driver_Win64()
        {
            bool Updated;

            using (var FirefoxProvider = new FirefoxProvider(GlobalSetup.FirefoxTempInstallLocation))
            {
                string CurrentVersion = FirefoxProvider.GetCurrentInstalledDriverVersion();
                Assert.That(CurrentVersion, Is.Empty);

                Updated = FirefoxProvider.DownloadLatestDriverVersion(TMech.Platform.Win64);
                Assert.That(Updated, Is.True);

                CurrentVersion = FirefoxProvider.GetCurrentInstalledDriverVersion();
                Console.WriteLine("Current version: " + CurrentVersion);
                Assert.That(CurrentVersion, Is.Not.Empty);

                Updated = FirefoxProvider.DownloadLatestDriverVersion(TMech.Platform.Win64);
                Assert.That(Updated, Is.False);
            }
        }

        [TestCase(Category = Category_Installed)]
        public void Installed_Version_Browser_Linux64()
        {
            using (var FirefoxProvider = new FirefoxProvider(GlobalSetup.FirefoxTempInstallLocation))
            {
                string CurrentVersion = FirefoxProvider.GetCurrentInstalledBrowserVersion();
                Assert.That(CurrentVersion, Is.Empty);

                bool Updated = FirefoxProvider.DownloadLatestBrowserVersion(TMech.Platform.Linux64);
                Assert.That(Updated, Is.True);

                CurrentVersion = FirefoxProvider.GetCurrentInstalledBrowserVersion();
                Console.WriteLine("Current version: " + CurrentVersion);
                Assert.That(CurrentVersion, Is.Not.Empty);

                Updated = FirefoxProvider.DownloadLatestBrowserVersion(TMech.Platform.Linux64);
                Assert.That(Updated, Is.False);
            }
        }

        [TestCase(Category = Category_Installed)]
        public void Installed_Version_Driver_Linux64()
        {
            bool Updated;

            using (var FirefoxProvider = new FirefoxProvider(GlobalSetup.FirefoxTempInstallLocation))
            {
                string CurrentVersion = FirefoxProvider.GetCurrentInstalledDriverVersion();
                Assert.That(CurrentVersion, Is.Empty);

                Updated = FirefoxProvider.DownloadLatestDriverVersion(TMech.Platform.Linux64);
                Assert.That(Updated, Is.True);

                CurrentVersion = FirefoxProvider.GetCurrentInstalledDriverVersion();
                Console.WriteLine("Current version: " + CurrentVersion);
                Assert.That(CurrentVersion, Is.Not.Empty);

                Updated = FirefoxProvider.DownloadLatestDriverVersion(TMech.Platform.Linux64);
                Assert.That(Updated, Is.False);
            }
        }

        #endregion

        #region LATEST

        private const string Category_Latest = "FirefoxProvider = Latest";

        [TestCase(Category = Category_Latest)]
        public void Latest_Version_Browser_Win64()
        {
            using (var FirefoxProvider = new FirefoxProvider(GlobalSetup.FirefoxTempInstallLocation))
            {
                string LatestVersion = FirefoxProvider.GetLatestAvailableBrowserVersion(TMech.Platform.Win64);
                Console.WriteLine("Latest version: " + LatestVersion);
                Assert.That(LatestVersion, Is.Not.Empty);
            }
        }

        [TestCase(Category = Category_Latest)]
        public void Latest_Version_Driver_Win64()
        {
            using (var FirefoxProvider = new FirefoxProvider(GlobalSetup.FirefoxTempInstallLocation))
            {
                string LatestVersion = FirefoxProvider.GetLatestAvailableDriverVersion(TMech.Platform.Win64);
                Console.WriteLine("Latest version: " + LatestVersion);
                Assert.That(LatestVersion, Is.Not.Empty);
            }
        }

        [TestCase(Category = Category_Latest)]
        public void Latest_Version_Browser_Linux64()
        {
            using (var FirefoxProvider = new FirefoxProvider(GlobalSetup.FirefoxTempInstallLocation))
            {
                string LatestVersion = FirefoxProvider.GetLatestAvailableBrowserVersion(TMech.Platform.Linux64);
                Console.WriteLine("Latest version: " + LatestVersion);
                Assert.That(LatestVersion, Is.Not.Empty);
            }
        }

        [TestCase(Category = Category_Latest)]
        public void Latest_Version_Driver_Linux64()
        {
            using (var FirefoxProvider = new FirefoxProvider(GlobalSetup.FirefoxTempInstallLocation))
            {
                string LatestVersion = FirefoxProvider.GetLatestAvailableDriverVersion(TMech.Platform.Linux64);
                Console.WriteLine("Latest version: " + LatestVersion);
                Assert.That(LatestVersion, Is.Not.Empty);
            }
        }

        #endregion
    }
}
 