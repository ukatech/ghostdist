using System;
using System.IO;
using System.Windows.Forms;
using GhostDist.Models;
using GhostDist.Services;

namespace GhostDist.Forms
{
    /// <summary>
    /// プロジェクト編集フォーム
    /// </summary>
    public partial class ProjectEditForm : Form
    {
        public ProjectSettings Settings { get; set; }
        public FtpConfiguration CommonFtp { get; set; }

        public ProjectEditForm()
        {
            InitializeComponent();
            Settings = new ProjectSettings();
            InitializeForm();
        }

        public ProjectEditForm(ProjectSettings settings)
        {
            InitializeComponent();
            Settings = settings;
            InitializeForm();
            LoadSettings();
        }

        private void InitializeForm()
        {
            // デフォルト処理タイプを設定
            networkRadio.Checked = true;
            UpdateControlsState();
        }

        private void LoadSettings()
        {
            // 基本設定
            nameEdit.Text = Settings.Name;
            targetFolderEdit.Text = Settings.TargetFolder;
            upDirEdit.Text = Settings.Directory;
            htmlEdit.Text = Settings.HtmlFile;
            narNameEdit.Text = Settings.NarName;
            defaultCheckBox.Checked = Settings.DefaultCheck;

            // 処理タイプ
            switch (Settings.Type)
            {
                case SettingType.Network:
                    networkRadio.Checked = true;
                    break;
                case SettingType.Upload:
                    narUpRadio.Checked = true;
                    break;
                case SettingType.NarCreate:
                    narCreateRadio.Checked = true;
                    break;
            }

            // FTP設定
            useDefaultFTPCheck.Checked = Settings.UseCommonFtp;
            serverEdit.Text = Settings.PrivateFtp.Server;
            idEdit.Text = Settings.PrivateFtp.UserId;
            passwordEdit.Text = Settings.PrivateFtp.Password;
            passiveCheck.Checked = Settings.PrivateFtp.Passive;
            sslCheck.Checked = Settings.PrivateFtp.UseSSL;

            // ファイルパターン
            processNameMemo.Text = Settings.ProcessName;
            escapeNameMemo.Text = Settings.ExcludeName;

            UpdateControlsState();
        }

        private void SaveSettings()
        {
            // 基本設定
            Settings.Name = nameEdit.Text;
            Settings.TargetFolder = targetFolderEdit.Text;
            Settings.Directory = upDirEdit.Text;
            Settings.HtmlFile = htmlEdit.Text;
            Settings.NarName = narNameEdit.Text;
            Settings.DefaultCheck = defaultCheckBox.Checked;

            // 処理タイプ
            if (networkRadio.Checked)
                Settings.Type = SettingType.Network;
            else if (narUpRadio.Checked)
                Settings.Type = SettingType.Upload;
            else
                Settings.Type = SettingType.NarCreate;

            // FTP設定
            Settings.UseCommonFtp = useDefaultFTPCheck.Checked;
            Settings.PrivateFtp.Server = serverEdit.Text;
            Settings.PrivateFtp.UserId = idEdit.Text;
            Settings.PrivateFtp.Password = passwordEdit.Text;
            Settings.PrivateFtp.Passive = passiveCheck.Checked;
            Settings.PrivateFtp.UseSSL = sslCheck.Checked;

            // ファイルパターン
            Settings.ProcessName = processNameMemo.Text;
            Settings.ExcludeName = escapeNameMemo.Text;
        }

        private void UpdateControlsState()
        {
            bool isNetwork = networkRadio.Checked;
            bool needsNar = !isNetwork;
            bool needsFtp = !narCreateRadio.Checked;

            // HTML/NARは非ネットワーク更新のみ
            htmlEdit.Enabled = needsNar;
            htmlLocateButton.Enabled = needsNar;
            narNameEdit.Enabled = needsNar;
            narLocateButton.Enabled = needsNar;

            // FTP設定はNarCreate以外で有効
            serverEdit.Enabled = needsFtp && !useDefaultFTPCheck.Checked;
            idEdit.Enabled = needsFtp && !useDefaultFTPCheck.Checked;
            passwordEdit.Enabled = needsFtp && !useDefaultFTPCheck.Checked;
            passiveCheck.Enabled = needsFtp && !useDefaultFTPCheck.Checked;
            sslCheck.Enabled = needsFtp && !useDefaultFTPCheck.Checked;
            useDefaultFTPCheck.Enabled = needsFtp;
            setDefaultButton.Enabled = needsFtp;

            // アーカイブ生成時の必須ファイル追加
            if (needsNar)
            {
                EnsurePatternExists(processNameMemo, "install.txt");
                EnsurePatternExists(processNameMemo, "updates2.dau");
                EnsurePatternExists(processNameMemo, "ghost/master/updates2.dau");
            }
            else
            {
                // ネットワーク更新時は不要なパターンを削除
                RemovePattern(processNameMemo, "install.txt");
                RemovePattern(processNameMemo, "updates2.dau");
                RemovePattern(processNameMemo, "ghost/master/updates2.dau");
            }
        }

        private void EnsurePatternExists(TextBox textBox, string pattern)
        {
            var lines = textBox.Lines;
            foreach (var line in lines)
            {
                if (line.Trim() == pattern)
                    return;
            }

            textBox.AppendText(Environment.NewLine + pattern);
        }

        private void RemovePattern(TextBox textBox, string pattern)
        {
            var lines = new System.Collections.Generic.List<string>(textBox.Lines);
            lines.RemoveAll(line => line.Trim() == pattern);
            textBox.Lines = lines.ToArray();
        }

        private void networkRadio_CheckedChanged(object sender, EventArgs e)
        {
            UpdateControlsState();
        }

        private void narUpRadio_CheckedChanged(object sender, EventArgs e)
        {
            UpdateControlsState();
        }

        private void narCreateRadio_CheckedChanged(object sender, EventArgs e)
        {
            UpdateControlsState();
        }

        private void useDefaultFTPCheck_CheckedChanged(object sender, EventArgs e)
        {
            UpdateControlsState();
        }

        private void targetFolderLocateButton_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "基準フォルダを選択してください";
                dialog.SelectedPath = targetFolderEdit.Text;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    targetFolderEdit.Text = dialog.SelectedPath;
                }
            }
        }

        private void htmlLocateButton_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "HTML (*.htm,*.html,*.shtml)|*.htm;*.html;*.shtml|すべてのファイル (*.*)|*.*";
                dialog.Title = "配布ページHTMLファイルを選択";
                dialog.FileName = htmlEdit.Text;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    htmlEdit.Text = dialog.FileName;
                }
            }
        }

        private void narLocateButton_Click(object sender, EventArgs e)
        {
            using (var dialog = new SaveFileDialog())
            {
                dialog.Filter = "Nanika Archive,ZIP|*.nar;*.zip|すべてのファイル (*.*)|*.*";
                dialog.Title = "NAR/ZIPファイル名を指定";
                dialog.FileName = narNameEdit.Text;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    narNameEdit.Text = dialog.FileName;
                }
            }
        }

        private void setDefaultButton_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "この設定を共通FTP設定にしますか？",
                "確認",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                if (CommonFtp != null)
                {
                    CommonFtp.Server = serverEdit.Text;
                    CommonFtp.UserId = idEdit.Text;
                    CommonFtp.Password = passwordEdit.Text;
                    CommonFtp.Passive = passiveCheck.Checked;
                    CommonFtp.UseSSL = sslCheck.Checked;
                }

                MessageBox.Show("共通FTP設定に保存されました。", "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            // 必須項目チェック
            if (string.IsNullOrWhiteSpace(nameEdit.Text))
            {
                MessageBox.Show("名称を入力してください。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                nameEdit.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(targetFolderEdit.Text))
            {
                MessageBox.Show("基準フォルダを入力してください。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                targetFolderEdit.Focus();
                return;
            }

            if (!narCreateRadio.Checked)
            {
                // FTP設定チェック
                if (!useDefaultFTPCheck.Checked)
                {
                    if (string.IsNullOrWhiteSpace(serverEdit.Text))
                    {
                        MessageBox.Show("サーバーを入力してください。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        serverEdit.Focus();
                        return;
                    }
                }
            }

            SaveSettings();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void previewButton_Click(object sender, EventArgs e)
        {
            // 基準フォルダのチェック
            if (string.IsNullOrWhiteSpace(targetFolderEdit.Text))
            {
                MessageBox.Show("基準フォルダを入力してください。", "入力エラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                targetFolderEdit.Focus();
                return;
            }

            if (!Directory.Exists(targetFolderEdit.Text))
            {
                MessageBox.Show("基準フォルダが存在しません。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
                targetFolderEdit.Focus();
                return;
            }

            // 一時的な設定オブジェクトを作成
            var tempSettings = new ProjectSettings
            {
                Name = string.IsNullOrWhiteSpace(nameEdit.Text) ? "(プレビュー)" : nameEdit.Text,
                TargetFolder = targetFolderEdit.Text,
                ProcessName = processNameMemo.Text,
                ExcludeName = escapeNameMemo.Text
            };

            // ログフォームを表示
            var logForm = new LogForm { IsLog = false };
            logForm.Show();
            logForm.EnableCloseButton(false);

            try
            {
                // プレビューサービスを実行
                var previewService = new FileFilterPreviewService();
                previewService.LogMessage += (s, msg) => logForm.AddLog(msg);

                previewService.PreviewFiles(tempSettings);
            }
            catch (Exception ex)
            {
                logForm.AddLog($"エラーが発生しました: {ex.Message}");
            }
            finally
            {
                logForm.EnableCloseButton(true);
            }
        }
    }
}
