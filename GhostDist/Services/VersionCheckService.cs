using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GhostDist.Services
{
    /// <summary>
    /// GitHubリリース情報
    /// </summary>
    public class GitHubReleaseInfo
    {
        /// <summary>
        /// タグ名（例: "v0.2.2.0"）
        /// </summary>
        public string TagName { get; set; }

        /// <summary>
        /// リリースページのURL
        /// </summary>
        public string HtmlUrl { get; set; }

        /// <summary>
        /// リリース名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// プレリリースかどうか
        /// </summary>
        public bool IsPrerelease { get; set; }
    }

    /// <summary>
    /// バージョンチェックサービス
    /// GitHub APIで最新バージョンを取得し、現在のバージョンと比較する
    /// </summary>
    public class VersionCheckService : IDisposable
    {
        private HttpClient _httpClient;
        private bool _disposed;

        public VersionCheckService()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(10);
            // GitHub APIはUser-Agentヘッダーを要求
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "GhostDist-App");
        }

        /// <summary>
        /// GitHub APIで最新バージョンを取得（非同期）
        /// </summary>
        /// <param name="repoOwner">リポジトリオーナー（例: "ukatech"）</param>
        /// <param name="repoName">リポジトリ名（例: "ghostdist"）</param>
        /// <returns>リリース情報、エラー時はnull</returns>
        public async Task<GitHubReleaseInfo> GetLatestReleaseAsync(string repoOwner, string repoName)
        {
            try
            {
                string apiUrl = $"https://api.github.com/repos/{repoOwner}/{repoName}/releases/latest";
                var response = await _httpClient.GetAsync(apiUrl);

                if (!response.IsSuccessStatusCode)
                    return null;

                var json = await response.Content.ReadAsStringAsync();
                return ParseReleaseJson(json);
            }
            catch (HttpRequestException)
            {
                // ネットワークエラー: 静かに失敗
                return null;
            }
            catch (TaskCanceledException)
            {
                // タイムアウト: 静かに失敗
                return null;
            }
            catch (Exception)
            {
                // その他のエラー: 静かに失敗
                return null;
            }
        }

        /// <summary>
        /// JSON手動解析（正規表現を使用）
        /// NuGetパッケージを追加しないため、軽量な正規表現で解析
        /// </summary>
        /// <param name="json">GitHub APIのJSONレスポンス</param>
        /// <returns>リリース情報、解析失敗時はnull</returns>
        private GitHubReleaseInfo ParseReleaseJson(string json)
        {
            try
            {
                // 正規表現でJSON解析
                var tagMatch = Regex.Match(json, "\"tag_name\"\\s*:\\s*\"([^\"]+)\"");
                var urlMatch = Regex.Match(json, "\"html_url\"\\s*:\\s*\"([^\"]+)\"");
                var nameMatch = Regex.Match(json, "\"name\"\\s*:\\s*\"([^\"]+)\"");
                var prereleaseMatch = Regex.Match(json, "\"prerelease\"\\s*:\\s*(true|false)");

                if (!tagMatch.Success || !urlMatch.Success)
                    return null;

                return new GitHubReleaseInfo
                {
                    TagName = tagMatch.Groups[1].Value,
                    HtmlUrl = urlMatch.Groups[1].Value,
                    Name = nameMatch.Success ? nameMatch.Groups[1].Value : "",
                    IsPrerelease = prereleaseMatch.Success && prereleaseMatch.Groups[1].Value == "true"
                };
            }
            catch (Exception)
            {
                // 解析エラー: 静かに失敗
                return null;
            }
        }

        /// <summary>
        /// バージョン比較: 新しいバージョンが利用可能か？
        /// </summary>
        /// <param name="currentVersion">現在のバージョン（例: "0.2.1.4"）</param>
        /// <param name="latestTagName">GitHub tag_name（例: "v0.2.2.0" または "0.2.2.0"）</param>
        /// <returns>新しいバージョンが利用可能な場合true</returns>
        public bool IsNewerVersionAvailable(Version currentVersion, string latestTagName)
        {
            if (currentVersion == null || string.IsNullOrEmpty(latestTagName))
                return false;

            // "v"プレフィックスを削除
            string versionString = latestTagName.TrimStart('v', 'V');

            if (!Version.TryParse(versionString, out Version latestVersion))
                return false;

            return latestVersion > currentVersion;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _httpClient?.Dispose();
                _disposed = true;
            }
        }
    }
}
