using System;
using GhostDist.Models;

namespace GhostDist.Services
{
    /// <summary>
    /// ファイルフィルタリングプレビューサービス
    /// 実際のアップロードやアーカイブ作成前に、フィルタリング結果を確認する
    /// </summary>
    public class FileFilterPreviewService
    {
        public event EventHandler<string> LogMessage;

        /// <summary>
        /// フィルタリング結果をプレビュー
        /// </summary>
        /// <param name="settings">プロジェクト設定</param>
        /// <returns>成功時true</returns>
        public bool PreviewFiles(ProjectSettings settings)
        {
            OnLogMessage("=== ファイルフィルタリングプレビュー ===");
            OnLogMessage($"プロジェクト: {settings.Name}");
            OnLogMessage($"基準フォルダ: {settings.TargetFolder}");
            OnLogMessage("");

            // ファイルリスト生成
            var updates = new Updates2DauManager
            {
                TargetFolder = settings.TargetFolder
            };

            OnLogMessage("処理対象ファイル名条件:");
            if (!string.IsNullOrWhiteSpace(settings.ProcessName))
            {
                var processPatterns = settings.ProcessName.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var pattern in processPatterns)
                {
                    OnLogMessage($"  {pattern.Trim()}");
                }
            }
            else
            {
                OnLogMessage("  (なし)");
            }

            OnLogMessage("");
            OnLogMessage("除外ファイル名条件:");
            if (!string.IsNullOrWhiteSpace(settings.ExcludeName))
            {
                var excludePatterns = settings.ExcludeName.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var pattern in excludePatterns)
                {
                    OnLogMessage($"  {pattern.Trim()}");
                }
            }
            else
            {
                OnLogMessage("  (なし)");
            }

            OnLogMessage("");
            OnLogMessage("ファイルリストを生成中...");

            try
            {
                var hasNoErrors = updates.Make(settings.ProcessName, settings.ExcludeName);

                if (!hasNoErrors)
                {
                    OnLogMessage("");
                    OnLogMessage("【警告】delete.txtに問題となる可能性のある記述を検出しました:");
                    OnLogMessage(updates.DeleteTxtError);
                    OnLogMessage("");
                }

                OnLogMessage("");
                OnLogMessage($"処理対象ファイル数: {updates.Files.Count} ファイル");
                OnLogMessage("");
                OnLogMessage("処理対象ファイル一覧:");

                if (updates.Files.Count > 0)
                {
                    foreach (var file in updates.Files)
                    {
                        OnLogMessage($"  {file.RelativePath}");
                    }
                }
                else
                {
                    OnLogMessage("  (該当ファイルなし)");
                }

                OnLogMessage("");
                OnLogMessage("=== プレビュー完了 ===");

                return true;
            }
            catch (Exception ex)
            {
                OnLogMessage("");
                OnLogMessage($"エラーが発生しました: {ex.Message}");
                OnLogMessage("=== プレビュー失敗 ===");
                return false;
            }
        }

        protected virtual void OnLogMessage(string message)
        {
            LogMessage?.Invoke(this, message);
        }
    }
}
