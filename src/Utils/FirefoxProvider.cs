using OpenQA.Selenium;
using SharpCompress.Archives;
using SharpCompress.Common;
using SharpCompress.Compressors.BZip2;
using SharpCompress.Readers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Formats.Tar;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

/* https://www.7-zip.org/sdk.html
 * https://ironpdf.com/blog/net-help/sevenzip-csharp-guide/
 * https://learn.microsoft.com/en-us/dotnet/api/system.formats.tar?view=net-8.0
 */

namespace TMech.Core.Utils
{
    /// <summary>
    /// <para>Utility that allows you to maintain the latest test version of Firefox in a directory of your choice. This service handles checking versions as well as downloading, and extrating the latest stable release.</para>
    /// <para>To learn more about Firefox for automation testing see here: https://googleFirefoxlabs.github.io/Firefox-for-testing/</para>
    /// </summary>
    public sealed class FirefoxProvider : IDisposable
    {
        public DirectoryInfo InstallLocation { get; }

        private readonly HttpClient HttpClient;
        private bool IsDisposed;
        private const string BrowserVersionFileName = "BROWSER_VERSION";
        private const string DriverVersionFileName = "DRIVER_VERSION";
        private const UnixFileMode FilePermissions = UnixFileMode.UserExecute | UnixFileMode.UserWrite | UnixFileMode.UserRead | UnixFileMode.GroupRead;

        #region Github data structures

        private sealed record GithubApiRelease
        {
            [JsonPropertyName("tag_name")]
            public string TagName { get; set; } = string.Empty;

            [JsonPropertyName("assets")]
            public GithubApiAsset[] Assets { get; set; } = Array.Empty<GithubApiAsset>();
        }

        private sealed record GithubApiAsset
        {
            [JsonPropertyName("name")]
            public string Name { get; set; } = string.Empty;

            [JsonPropertyName("browser_download_url")]
            public string DownloadUrl { get; set; } = string.Empty;
        }

        #endregion

        private record BinaryDownloadAssetData
        {
            public string ReadableVersion { get; init; } = string.Empty;
            public double ComparableVersion { get; init; }
            public Uri? DownloadUri { get; init; }
        }

        /// <summary>Sets or gets the timeouts for requests as well as downloading data. Defaults to 30 seconds.</summary>
        public TimeSpan RequestTimeout { get => HttpClient.Timeout; set => HttpClient.Timeout = value; }

        public FirefoxProvider(DirectoryInfo installLocation)
        {
            Debug.Assert(installLocation != null);
            InstallLocation = installLocation;

            if (!InstallLocation.Exists) throw new DirectoryNotFoundException("Firefox install directory does not exist: " + InstallLocation.FullName);
            HttpClient = new HttpClient(new HttpClientHandler() { AllowAutoRedirect = false })
            {
                Timeout = TimeSpan.FromSeconds(30.0d)
            };
            HttpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("TMech.FirefoxProvider", "1.0"));
        }

        /// <summary>
        /// Deletes all files and folders in <see cref="InstallLocation"/>.
        /// </summary>
        public void ClearInstallLocation()
        {
            foreach (FileInfo CurrentFile in InstallLocation.EnumerateFiles())
            {
                CurrentFile.Delete();
            }
            foreach (DirectoryInfo CurrentDirectory in InstallLocation.EnumerateDirectories())
            {
                CurrentDirectory.Delete(true);
            }
        }

        /// <summary>
        /// Retrieves the version of Firefox currently installed in <see cref="InstallLocation"/>. If Firefox cannot be found (the version file) then an empty string is returned.
        /// </summary>
        public string GetCurrentInstalledBrowserVersion()
        {
            var VersionFile = new FileInfo(Path.Combine(InstallLocation.FullName, BrowserVersionFileName));
            if (!VersionFile.Exists) return string.Empty;

            return File.ReadAllText(VersionFile.FullName);
        }

        /// <summary>
        /// Retrieves the version of Firefox currently installed in <see cref="InstallLocation"/>. If Firefox cannot be found (the version file) then an empty string is returned.
        /// </summary>
        public string GetCurrentInstalledDriverVersion()
        {
            var VersionFile = new FileInfo(Path.Combine(InstallLocation.FullName, DriverVersionFileName));
            if (!VersionFile.Exists) return string.Empty;

            return File.ReadAllText(VersionFile.FullName);
        }

        /// <summary>
        /// Retrieves the latest available version of Firefox online.
        /// </summary>
        public string GetLatestAvailableBrowserVersion(OSPlatform platform)
        {
            return GetBrowserVersionAndDownloadURL(platform).ReadableVersion;
        }

        /// <summary>
        /// Retrieves the latest available version of Firefox online.
        /// </summary>
        public string GetLatestAvailableDriverVersion(OSPlatform platform)
        {
            return GetDriverVersionAndDownloadURL(platform).ReadableVersion;
        }

        /// <summary>
        /// <para>Downloads and extracts Firefox and its corresponding webdriver into <see cref="InstallLocation"/> if the currently installed version is lower than the latest available version OR if Firefox is not installed. Only major revisions are counted when factoring in version differences.</para>
        /// NOTE: If there is a currently installed version of Firefox it will not be removed first! Existing files will merely be overwritten. This might leave certain version-specific files behind.
        /// </summary>
        /// <param name="forceUpdate">Whether to force a download and install of Firefox even if the installed version is already the newest.</param>
        /// <returns><see langword="true"/> if Firefox was downloaded and installed, <see langword="false"/> otherwise</returns>
        public bool DownloadLatestBrowserVersion(OSPlatform platform, bool forceUpdate = false)
        {
            BinaryDownloadAssetData DownloadAssetData = GetBrowserVersionAndDownloadURL(platform);
            string CurrentVersion = GetCurrentInstalledBrowserVersion();

            double CurrentVersionComparable = ParseVersionString(CurrentVersion);

            if (!forceUpdate && CurrentVersionComparable >= DownloadAssetData.ComparableVersion) return false;

            var Request = new HttpRequestMessage()
            {
                RequestUri = DownloadAssetData.DownloadUri,
                Method = HttpMethod.Get
            };

            HttpResponseMessage Response = HttpClient.Send(Request);

            long? Downloadsize = Response.Content.Headers.ContentLength;
            Debug.Assert(Downloadsize is not null && Downloadsize > 0);

            var Buffer = new MemoryStream(Convert.ToInt32(Downloadsize));
            Response.Content.ReadAsStream().CopyTo(Buffer);
            Response.Dispose();
            Buffer.Position = 0;

            switch (platform)
            {
                case var x when x == OSPlatform.Windows:
                    ExtractWin64BrowserToInstallLocation(Buffer);
                    break;
                case var x when x == OSPlatform.Linux:
                    ExtractLinux64BrowserToInstallLocation(Buffer);
                    break;
            }

            File.WriteAllText(Path.Combine(InstallLocation.FullName, BrowserVersionFileName), DownloadAssetData.ReadableVersion);

            return true;
        }

        private void ExtractWin64BrowserToInstallLocation(MemoryStream browserArchiveStream)
        {
            IReader? Reader = null;
            MemoryStream Buffer = ExtractArchiveFromWinInstaller(browserArchiveStream);

            try
            {
                IArchive Archive = ArchiveFactory.Open(Buffer);
                Reader = Archive.ExtractAllEntries();
                var Options = new ExtractionOptions() { ExtractFullPath = true, Overwrite = true };

                const string WindowsArchiveDirectoryPrefix = "core/";

                while (Reader.MoveToNextEntry())
                {
                    if (Reader.Entry.IsDirectory) continue;
                    if (!Reader.Entry.Key.StartsWith("core/")) continue;

                    string SanitizedName = Reader.Entry.Key.Replace(WindowsArchiveDirectoryPrefix, "").TrimStart('/');

                    string FinalName = Path.Combine(InstallLocation.FullName, SanitizedName);
                    Debug.Assert(!string.IsNullOrWhiteSpace(FinalName));

                    new FileInfo(FinalName).Directory?.Create();
                    Reader.WriteEntryToFile(FinalName, Options);
                }
            }
            finally
            {
                Reader?.Dispose();
            }
        }

        private void ExtractLinux64BrowserToInstallLocation(MemoryStream browserArchiveStream)
        {
            using (var BZ2Stream = new BZip2Stream(browserArchiveStream, SharpCompress.Compressors.CompressionMode.Decompress, false))
            {
                TarFile.ExtractToDirectory(BZ2Stream, InstallLocation.FullName, true);
            }
        }

        private BinaryDownloadAssetData GetBrowserVersionAndDownloadURL(OSPlatform platform)
        {
            string PlatformName = platform switch
            {
                var x when x == OSPlatform.Windows => "win64",
                var x when x == OSPlatform.Linux => "linux64",
                var x when x == OSPlatform.OSX => "osx",
                _ => throw new PlatformNotSupportedException("Only Win, Linux and Mac platforms are supported")
            };

            var Request = new HttpRequestMessage(HttpMethod.Head, new Uri($"https://download.mozilla.org/?product=firefox-latest&os={PlatformName}&lang=en-US"));
            var Response = HttpClient.Send(Request, HttpCompletionOption.ResponseHeadersRead);

            Debug.Assert(Response.StatusCode == System.Net.HttpStatusCode.Redirect);
            Debug.Assert(Response.Headers.Location is not null);
            Uri DownloadUri = Response.Headers.Location;

            var VersionCapture = new Regex("releases/(.+?)/").Match(DownloadUri.ToString());
            Debug.Assert(VersionCapture.Success && VersionCapture.Groups.Count > 1);

            string VersionString = VersionCapture.Groups[1].Value;
            Debug.Assert(!string.IsNullOrWhiteSpace(VersionString));

            return new BinaryDownloadAssetData()
            {
                DownloadUri = DownloadUri,
                ReadableVersion = VersionString,
                ComparableVersion = ParseVersionString(VersionString)
            };
        }

        private static double ParseVersionString(string input) => input.Trim().Length == 0 ? 0.0d : double.Parse(input.TrimStart('v'));

        private BinaryDownloadAssetData GetDriverVersionAndDownloadURL(OSPlatform platform)
        {
            string DriverFileNameFragment = platform switch
            {
                var x when x == OSPlatform.Windows => "win64.zip",
                var x when x == OSPlatform.Linux => "linux64.tar.gz",
                var x when x == OSPlatform.OSX => "macos.tar.gz",
                _ => throw new PlatformNotSupportedException("Only Win, Linux and Mac platforms are supported")
            };

            var Request = new HttpRequestMessage(HttpMethod.Get, new Uri("https://api.github.com/repos/mozilla/geckodriver/releases"));
            var Response = HttpClient.Send(Request);

            Response.EnsureSuccessStatusCode();
            string JsonResponseContent = Response.Content.ReadAsStringAsync().GetAwaiter().GetResult().Trim();
            Debug.Assert(JsonResponseContent.Length > 0);

            GithubApiRelease[]? GithubResponseData = JsonSerializer.Deserialize<GithubApiRelease[]>(JsonResponseContent);
            Debug.Assert(GithubResponseData is not null);

            Tuple<double, string>[] Versions = new Tuple<double, string>[GithubResponseData.Length];
            Dictionary<string, GithubApiAsset[]> VersionWithAssetsManifest = new Dictionary<string, GithubApiAsset[]>(GithubResponseData.Length);

            for (int Index = 0; Index < GithubResponseData.Length; Index++)
            {
                string VersionString = GithubResponseData[Index].TagName;

                var NewEntry = new Tuple<double, string>
                (
                    ParseVersionString(VersionString),
                    VersionString
                );

                VersionWithAssetsManifest.Add(VersionString, GithubResponseData[Index].Assets);
                Versions[Index] = NewEntry;
            }

            Tuple<double, string>? NewestVersion = Versions.Max();
            Debug.Assert(NewestVersion is not null);
            Debug.Assert(NewestVersion.Item1 > 0);

            GithubApiAsset[] NewestVersionAssets = VersionWithAssetsManifest[NewestVersion.Item2];
            string NewestVersionDownloadUrl = NewestVersionAssets.Single(x => x.Name.EndsWith(DriverFileNameFragment)).DownloadUrl;

            return new BinaryDownloadAssetData()
            {
                ComparableVersion = NewestVersion.Item1,
                ReadableVersion = NewestVersion.Item2,
                DownloadUri = new Uri(NewestVersionDownloadUrl)
            };
        }

        /// <summary>
        /// Despite Firefox being portable it comes as an installer which is a 7-zip self-extracting archive.
        /// By searching through the byte stream we can find where the installer data ends and extract the actual archive data
        /// and simply treat that as a normal 7-zip archive which can be extracted as per normal.
        /// </summary>
        private MemoryStream ExtractArchiveFromWinInstaller(MemoryStream setupBinaryStream)
        {
            int ArchiveStart = 0;
            const string MagicWord = "@InstallEnd@";

            Span<byte> Buffer = stackalloc byte[50];
            while (true)
            {
                int BytesRead = setupBinaryStream.Read(Buffer);
                if (BytesRead == 0)
                {
                    throw new InvalidOperationException("Win installer end of stream reached. Expected to find magic marker signifying the end of a 7zsfx installer");
                }

                int FoundAt = Encoding.UTF8.GetString(Buffer).IndexOf(MagicWord);

                if (FoundAt > -1)
                {
                    ArchiveStart = Convert.ToInt32(setupBinaryStream.Position - Buffer.Length + FoundAt + MagicWord.Length + 1);
                    break;
                }
            }

            Debug.Assert(ArchiveStart > 0);

            int ArchiveSize = Convert.ToInt32(setupBinaryStream.Length - ArchiveStart);
            var ArchiveBuffer = new byte[ArchiveSize];

            setupBinaryStream.Seek(ArchiveStart, SeekOrigin.Begin);
            setupBinaryStream.Read(ArchiveBuffer, 0, ArchiveSize);

            return new MemoryStream(ArchiveBuffer);
        }

        #region Destructors

        public void Dispose()
        {
            if (IsDisposed) return;
            IsDisposed = true;

            GC.SuppressFinalize(this);
            HttpClient?.Dispose();
        }

        ~FirefoxProvider()
        {
            HttpClient?.Dispose();
        }

        #endregion
    }
}
