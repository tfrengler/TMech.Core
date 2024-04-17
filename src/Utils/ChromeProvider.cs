using SharpCompress.Archives;
using SharpCompress.Common;
using SharpCompress.Readers;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;

namespace TMech.Utils
{
    /// <summary>
    /// <para>Utility that allows you to maintain the latest test version of Chrome in a directory of your choice. This service handles checking versions as well as downloading, and extrating the latest stable release.</para>
    /// <para>To learn more about Chrome for automation testing see here: https://googlechromelabs.github.io/chrome-for-testing/</para>
    /// </summary>
    public sealed class ChromeProvider : IDisposable
    {
        public DirectoryInfo InstallLocation { get; }
        public const string ManifestURL = "https://googlechromelabs.github.io/chrome-for-testing/last-known-good-versions-with-downloads.json";

        private readonly HttpClient HttpClient;
        private bool IsDisposed;
        private const string VersionFileName = "VERSION";
        private const UnixFileMode UnixFilePermissions = UnixFileMode.UserExecute | UnixFileMode.UserWrite | UnixFileMode.UserRead | UnixFileMode.GroupRead;


        #region Manifest JSON models
        private sealed record Manifest
        {
            [JsonPropertyName("channels")]
            public Channels Channels { get; init; } = new();
        }

        private sealed record Channels
        {
            [JsonPropertyName("Stable")]
            public Channel Stable { get; init; } = new();
        }

        private sealed record Channel
        {
            [JsonPropertyName("version")]
            public string Version { get; init; } = string.Empty;
            [JsonPropertyName("downloads")]
            public Downloads Downloads { get; init; } = new();
        }

        private sealed record Downloads
        {
            [JsonPropertyName("chrome")]
            public Download[] Chrome { get; init; } = Array.Empty<Download>();
            [JsonPropertyName("chromedriver")]
            public Download[] Chromedriver { get; init; } = Array.Empty<Download>();
        }

        private sealed record Download
        {
            [JsonPropertyName("platform")]
            public string Platform { get; init; } = string.Empty;
            [JsonPropertyName("url")]
            public string Url { get; init; } = string.Empty;
        }
        #endregion

        public ChromeProvider(DirectoryInfo installLocation)
        {
            Debug.Assert(installLocation != null);
            InstallLocation = installLocation;

            if (!InstallLocation.Exists) throw new DirectoryNotFoundException("Chrome install directory does not exist: " + installLocation.FullName);
            HttpClient = new HttpClient();
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
        /// Retrieves the version of Chrome currently installed in <see cref="InstallLocation"/>. If Chrome cannot be found (the version file) then an empty string is returned.
        /// </summary>
        public string GetCurrentInstalledVersion()
        {
            var VersionFile = new FileInfo(Path.Combine(InstallLocation.FullName, VersionFileName));
            if (!VersionFile.Exists) return string.Empty;

            return File.ReadAllText(VersionFile.FullName);
        }

        /// <summary>
        /// Retrieves the latest available version of Chrome online.
        /// </summary>
        public string GetLatestAvailableVersion(OSPlatform platform)
        {
            return GetVersionAndDownloadURLs(platform).Item1;
        }

        /// <summary>
        /// <para>Downloads and extracts Chrome and its corresponding webdriver into <see cref="InstallLocation"/> if the currently installed version is lower than the latest available version OR if Chrome is not installed. Only major revisions are counted when factoring in version differences.</para>
        /// NOTE: If there is a currently installed version of Chrome it will not be removed first! Existing files will merely be overwritten. This might leave certain version-specific files behind.
        /// </summary>
        /// <param name="skipDriver">If <see langword="true"/> then only the browser will be installed, skipping the webdriver.</param>
        /// <param name="forceUpdate">Whether to force a download and install of Chrome even if the installed version is already the newest.</param>
        /// <returns><see langword="true"/> if Chrome was downloaded and installed, <see langword="false"/> otherwise</returns>
        public bool DownloadLatestVersion(OSPlatform platform, bool skipDriver = false, bool forceUpdate = false)
        {
            (string LatestVersion, string ChromeURL, string ChromedriverURL) = GetVersionAndDownloadURLs(platform);
            string[] DownloadURLs = skipDriver ? new string[] { ChromeURL } : new string[] { ChromeURL, ChromedriverURL };
            string CurrentVersion = GetCurrentInstalledVersion();

            var LatestVersionSanitized = new string(LatestVersion.Split('.').First().Where(char.IsDigit).ToArray());
            var CurrentVersionSanitized = new string(CurrentVersion.Split('.').First().Where(char.IsDigit).ToArray());

            if (!forceUpdate && LatestVersionSanitized == CurrentVersionSanitized) return false;

            foreach (string CurrentDownloadURL in DownloadURLs)
            {
                var Request = new HttpRequestMessage()
                {
                    RequestUri = new Uri(CurrentDownloadURL),
                    Method = HttpMethod.Get
                };

                var CancellationTokenSource = new CancellationTokenSource(30000);
                HttpResponseMessage Response = HttpClient.SendAsync(Request, CancellationTokenSource.Token).GetAwaiter().GetResult();

                long? Downloadsize = Response.Content.Headers.ContentLength;
                Debug.Assert(Downloadsize is not null && Downloadsize > 0);

                var Buffer = new MemoryStream(Convert.ToInt32(Downloadsize));
                Response.Content.ReadAsStream().CopyTo(Buffer);
                Response.Dispose();
                Buffer.Position = 0;
                IReader? Reader = null;

                try
                {
                    IArchive Archive = ArchiveFactory.Open(Buffer);
                    Reader = Archive.ExtractAllEntries();
                    var Options = new ExtractionOptions() { ExtractFullPath = true, Overwrite = true };

                    while (Reader.MoveToNextEntry())
                    {
                        string RootDir = Reader.Entry.Key.Split('/').First();
                        string SanitizedName = Reader.Entry.Key.Replace(RootDir, "").TrimStart('/');

                        if (Reader.Entry.IsDirectory) continue;

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

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                File.SetUnixFileMode(Path.Combine(InstallLocation.FullName, "chrome"), UnixFilePermissions);
                File.SetUnixFileMode(Path.Combine(InstallLocation.FullName, "chrome_crashpad_handler"), UnixFilePermissions);

                if (!skipDriver)
                {
                    File.SetUnixFileMode(Path.Combine(InstallLocation.FullName, "chromedriver"), UnixFilePermissions);
                }
            }

            File.WriteAllText(Path.Combine(InstallLocation.FullName, VersionFileName), LatestVersion);

            return true;
        }

        public void Dispose()
        {
            if (IsDisposed) return;
            IsDisposed = true;

            GC.SuppressFinalize(this);
            HttpClient?.Dispose();
        }

        ~ChromeProvider()
        {
            HttpClient?.Dispose();
        }

        public Tuple<string, string, string> GetVersionAndDownloadURLs(OSPlatform platform)
        {
            var Request = new HttpRequestMessage()
            {
                RequestUri = new Uri(ManifestURL),
                Method = HttpMethod.Get
            };

            Request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Request.Headers.UserAgent.Add(new ProductInfoHeaderValue("ChromeProvider.API.Service", "1.0"));

            var CancellationTokenSource = new CancellationTokenSource(10000);
            HttpResponseMessage Response = HttpClient.SendAsync(Request, CancellationTokenSource.Token).GetAwaiter().GetResult();

            string ResponseContent = Response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            Response.Dispose();

            Manifest? ReleaseData;
            try
            {
                ReleaseData = JsonSerializer.Deserialize<Manifest>(ResponseContent);
            }
            catch (JsonException error)
            {
                throw new JsonException("Failed to deserialize GitHub response as JSON:" + Environment.NewLine + ResponseContent, error);
            }

            string PlatformName = platform switch
            {
                var x when x == OSPlatform.Windows => "win64",
                var x when x == OSPlatform.Linux => "linux64",
                var x when x == OSPlatform.OSX => "mac-x64",
                _ => throw new PlatformNotSupportedException("Only Win, Linux and Mac platforms are supported")
            };

            Debug.Assert(ReleaseData is not null);
            var ChromeUrl = ReleaseData.Channels.Stable.Downloads.Chrome.Single(current => current.Platform == PlatformName).Url;
            var ChromedriverUrl = ReleaseData.Channels.Stable.Downloads.Chromedriver.Single(current => current.Platform == PlatformName).Url;

            return new Tuple<string, string, string>(ReleaseData.Channels.Stable.Version, ChromeUrl, ChromedriverUrl);
        }
    }
}
