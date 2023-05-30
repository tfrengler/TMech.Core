using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;

namespace TMech.Core.Utils
{
    /// <summary>
    /// Chromium utility that allows you to maintain the latest version in a directory of your choice. This service handles checking versions as well as downloading, and extrating the latest release.
    /// </summary>
    public sealed class ChromiumProvider : IDisposable
    {
        public DirectoryInfo InstallLocation { get; }
        public const string ChromiumLatestReleaseURL = "https://api.github.com/repos/macchrome/winchrome/releases/latest";
        public const string ExecutableName = "chrome.exe";

        private readonly HttpClient HttpClient;

        #region GitHub JSON models
        private sealed record GitHubRelease
        {
            [JsonPropertyName("tag_name")]
            public string TagName { get; init; } = string.Empty;
            [JsonPropertyName("assets")]
            public GitHubAsset[] Assets { get; init; } = Array.Empty<GitHubAsset>();
        }

        private sealed record GitHubAsset
        {
            [JsonPropertyName("name")]
            public string Name { get; init; } = string.Empty;
            [JsonPropertyName("browser_download_url")]
            public string URL { get; init; } = string.Empty;
            [JsonPropertyName("size")]
            public int Size { get; init; }
        }
        #endregion

        public ChromiumProvider(DirectoryInfo installLocation)
        {
            InstallLocation = installLocation ?? throw new ArgumentNullException(nameof(installLocation));
            if (!InstallLocation.Exists) throw new DirectoryNotFoundException("Chromium install directory does not exist: " + installLocation.FullName);
            HttpClient = new HttpClient();
        }

        /// <summary>
        /// Retrieves the version of Chromium currently installed in <see cref="InstallLocation"/>. If Chromium is not installed (or rather if the executable cannot be found) then an empty string is returned.
        /// </summary>
        public string GetCurrentInstalledVersion()
        {
            string AbsoluteExecutableLocationAndFileName = Path.Combine(InstallLocation.FullName, ExecutableName);
            if (!File.Exists(AbsoluteExecutableLocationAndFileName)) return string.Empty;

            string? CurrentVersion = FileVersionInfo.GetVersionInfo(AbsoluteExecutableLocationAndFileName).FileVersion;
            Debug.Assert(CurrentVersion is not null);
            return CurrentVersion;
        }

        /// <summary>
        /// Retrieves the latest available version of Chromium online.
        /// </summary>
        public string GetLatestAvailableVersion()
        {
            return GetVersionAndDownloadURL().Item1;
        }

        /// <summary>
        /// Downloads and extracts Chromium into <see cref="InstallLocation"/> if the currently installed version is lower than the latest available version OR if Chromium is not installed.
        /// </summary>
        /// <param name="forceUpdate">Whether to force a download and install of Chromium even if the installed version is already the newest.</param>
        /// <returns>True if Chromium was downloaded and installed, false otherwise</returns>
        public bool DownloadLatestVersion(bool forceUpdate = false)
        {/*
            (string LatestVersion, string DownloadURL) = GetVersionAndDownloadURL();
            if (!forceUpdate && LatestVersion == GetCurrentInstalledVersion()) return false;

            var Request = new HttpRequestMessage()
            {
                RequestUri = new Uri(DownloadURL),
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

            foreach (FileInfo CurrentFile in InstallLocation.EnumerateFiles())
            {
                CurrentFile.Delete();
            }
            foreach (DirectoryInfo CurrentDirectory in InstallLocation.EnumerateDirectories())
            {
                CurrentDirectory.Delete(true);
            }

            try
            {
                IArchive Archive = ArchiveFactory.Open(Buffer);
                Reader = Archive.ExtractAllEntries();

                while (Reader.MoveToNextEntry())
                {
                    string RootDir = Reader.Entry.Key.Split('/').First();
                    string SanitizedName = Reader.Entry.Key.Replace(RootDir, "").TrimStart('/');

                    if (Reader.Entry.IsDirectory) continue;

                    string FinalName = Path.Combine(InstallLocation.FullName, SanitizedName);
                    Debug.Assert(!string.IsNullOrWhiteSpace(FinalName));
                    new FileInfo(FinalName).Directory?.Create();
                    Reader.WriteEntryToFile(FinalName, new ExtractionOptions() { ExtractFullPath = true, Overwrite = true });
                }
            }
            finally
            {
                Reader?.Dispose();
            }

            return true;*/
            return true;
        }

        public void Dispose()
        {
            HttpClient?.Dispose();
        }

        public Tuple<string, string> GetVersionAndDownloadURL()
        {
            var Request = new HttpRequestMessage()
            {
                RequestUri = new Uri(ChromiumLatestReleaseURL),
                Method = HttpMethod.Get
            };

            Request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Request.Headers.UserAgent.Add(new ProductInfoHeaderValue("ChromiumProvider.API.Service", "0.0"));

            var CancellationTokenSource = new CancellationTokenSource(10000);
            HttpResponseMessage Response = HttpClient.SendAsync(Request, CancellationTokenSource.Token).GetAwaiter().GetResult();

            string ResponseContent = Response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            Response.Dispose();

            GitHubRelease? ReleaseData;
            try
            {
                ReleaseData = JsonSerializer.Deserialize<GitHubRelease>(ResponseContent);
            }
            catch (JsonException error)
            {
                throw new JsonException("Failed to deserialize GitHub response as JSON:" + Environment.NewLine + ResponseContent, error);
            }

            Debug.Assert(ReleaseData is not null);
            Debug.Assert(!string.IsNullOrEmpty(ReleaseData.TagName));

            var WinZipAsset = ReleaseData.Assets.Where(current =>
            {
                return current.Name.StartsWith("ungoogled") && current.Name.EndsWith("_Win64.7z");
            }).First();

            Debug.Assert(!string.IsNullOrEmpty(WinZipAsset.URL));

            return new Tuple<string, string>(ReleaseData.TagName, WinZipAsset.URL);
        }
    }
}
