using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using GhostDist.Models;
using GhostDist.Services;

namespace GhostDist.Forms
{
    public partial class MainForm : Form
    {
        private List<ProjectSettings> _projects;
        private FtpConfiguration _commonFtp;
        private ConfigurationService _configService;
        private string _iniPath;
        private bool _isLog = false;
        private bool _noLogWindow = false;

        public MainForm()
        {
            InitializeComponent();

            _projects = new List<ProjectSettings>();
            _commonFtp = new FtpConfiguration();
            _iniPath = Path.Combine(Application.StartupPath, "ghostdist.ini");
            _configService = new ConfigurationService(_iniPath);

            LoadConfiguration();
            LoadWindowSettings();
        }

        private void LoadConfiguration()
        {
            try
            {
                _configService.LoadGeneralSettings(out _isLog, out _noLogWindow);
                _commonFtp = _configService.LoadCommonFtp();
                _projects = _configService.LoadProjects();

                // UIに反映
                logCheckBox.Checked = _isLog;
                noLogWindowCheckBox.Checked = _noLogWindow;

                // プロジェクトリストに追加
                projectListBox.Items.Clear();
                foreach (var project in _projects)
                {
                    projectListBox.Items.Add(project.Name, project.DefaultCheck);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"設定ファイルの読み込みに失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveConfiguration()
        {
            try
            {
                _configService.SaveProjects(_projects, _commonFtp, _isLog, _noLogWindow);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"設定ファイルの保存に失敗しました: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void newButton_Click(object sender, EventArgs e)
        {
            var editForm = new ProjectEditForm
            {
                CommonFtp = _commonFtp
            };

            if (editForm.ShowDialog() == DialogResult.OK)
            {
                _projects.Add(editForm.Settings);
                projectListBox.Items.Add(editForm.Settings.Name, editForm.Settings.DefaultCheck);
                SaveConfiguration();
            }
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            EditSelectedProject();
        }

        private void projectListBox_DoubleClick(object sender, EventArgs e)
        {
            EditSelectedProject();
        }

        private void EditSelectedProject()
        {
            if (projectListBox.SelectedIndex < 0)
            {
                MessageBox.Show("プロジェクトを選択してください。", "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var index = projectListBox.SelectedIndex;
            var project = _projects[index];
            var editForm = new ProjectEditForm(project)
            {
                CommonFtp = _commonFtp
            };

            if (editForm.ShowDialog() == DialogResult.OK)
            {
                // リストの表示名を更新
                projectListBox.Items[index] = project.Name;
                // 設定を保存
                SaveConfiguration();
            }
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (projectListBox.SelectedIndex < 0)
                return;

            var result = MessageBox.Show("選択されたプロジェクトを削除しますか?", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                _projects.RemoveAt(projectListBox.SelectedIndex);
                projectListBox.Items.RemoveAt(projectListBox.SelectedIndex);
                SaveConfiguration();
            }
        }

        private void runButton_Click(object sender, EventArgs e)
        {
            var checkedProjects = new List<ProjectSettings>();

            for (int i = 0; i < projectListBox.Items.Count; i++)
            {
                if (projectListBox.GetItemChecked(i))
                {
                    checkedProjects.Add(_projects[i]);
                }
            }

            if (checkedProjects.Count == 0)
            {
                MessageBox.Show("実行するプロジェクトを選択してください。", "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // フォームを無効化（元のDelphiコードと同様）
            Enabled = false;

            // ログフォーム表示
            LogForm logForm = null;
            if (!_noLogWindow)
            {
                logForm = new LogForm { IsLog = _isLog };
                logForm.Show();
                logForm.EnableCloseButton(false);
            }

            int successCount = 0;
            int execCount = checkedProjects.Count;

            try
            {
                // 各プロジェクトを実行
                foreach (var project in checkedProjects)
                {
                    try
                    {
                        var ftpConfig = project.UseCommonFtp ? _commonFtp : project.PrivateFtp;
                        bool success = false;

                        switch (project.Type)
                        {
                            case SettingType.Network:
                                success = ExecuteNetworkUpdate(project, ftpConfig, logForm);
                                break;

                            case SettingType.Upload:
                                success = ExecuteNarUpload(project, ftpConfig, logForm);
                                break;

                            case SettingType.NarCreate:
                                success = ExecuteNarCreate(project, logForm);
                                break;
                        }

                        if (success)
                            successCount++;
                    }
                    catch (Exception ex)
                    {
                        logForm?.AddLog($"エラー: {ex.Message}");
                    }
                }

                logForm?.AddLog($"{execCount}項目実行 - 成功{successCount} / 失敗{execCount - successCount}");
            }
            finally
            {
                // フォームを再度有効化
                Enabled = true;
                logForm?.EnableCloseButton(true);
            }
        }

        private bool ExecuteNetworkUpdate(ProjectSettings project, FtpConfiguration ftpConfig, LogForm logForm)
        {
            logForm?.AddLog($"=== {project.Name} (ネットワーク更新) ===");

            var service = new NetworkUpdateService();
            service.LogMessage += (s, msg) => logForm?.AddLog(msg);
            service.ProgressChanged += (s, e) => logForm?.SetProgress(e.BytesProcessed, e.TotalBytes);

            return service.Execute(project, ftpConfig);
        }

        private bool ExecuteNarUpload(ProjectSettings project, FtpConfiguration ftpConfig, LogForm logForm)
        {
            logForm?.AddLog($"=== {project.Name} (NAR作成+アップロード) ===");

            var service = new NetworkUpdateService();
            service.LogMessage += (s, msg) => logForm?.AddLog(msg);
            service.ProgressChanged += (s, e) => logForm?.SetProgress(e.BytesProcessed, e.TotalBytes);

            return service.ExecuteNarUpload(project, ftpConfig);
        }

        private bool ExecuteNarCreate(ProjectSettings project, LogForm logForm)
        {
            logForm?.AddLog($"=== {project.Name} (NAR作成) ===");

            var service = new NarCreationService();
            service.LogMessage += (s, msg) => logForm?.AddLog(msg);

            try
            {
                service.CreateNar(project);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void allSelectButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < projectListBox.Items.Count; i++)
            {
                projectListBox.SetItemChecked(i, true);
            }
        }

        private void deselectButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < projectListBox.Items.Count; i++)
            {
                projectListBox.SetItemChecked(i, false);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveWindowSettings();
            SaveConfiguration();
        }

        private void LoadWindowSettings()
        {
            var settings = Properties.Settings.Default;

            // サイズが保存されていれば復元
            if (settings.WindowSize.Width > 0 && settings.WindowSize.Height > 0)
            {
                var bounds = new Rectangle(settings.WindowLocation, settings.WindowSize);

                // 保存された位置が画面内にあるか確認
                bool isOnScreen = false;
                foreach (Screen screen in Screen.AllScreens)
                {
                    if (screen.WorkingArea.IntersectsWith(bounds))
                    {
                        isOnScreen = true;
                        break;
                    }
                }

                if (isOnScreen)
                {
                    StartPosition = FormStartPosition.Manual;
                    Location = settings.WindowLocation;
                    Size = settings.WindowSize;
                }
            }

            if (settings.WindowMaximized)
            {
                WindowState = FormWindowState.Maximized;
            }
        }

        private void SaveWindowSettings()
        {
            var settings = Properties.Settings.Default;

            // 最大化・最小化状態の場合はRestoreBoundsを使用
            if (WindowState == FormWindowState.Normal)
            {
                settings.WindowLocation = Location;
                settings.WindowSize = Size;
            }
            else
            {
                settings.WindowLocation = RestoreBounds.Location;
                settings.WindowSize = RestoreBounds.Size;
            }

            settings.WindowMaximized = (WindowState == FormWindowState.Maximized);
            settings.Save();
        }

        private void logCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            _isLog = logCheckBox.Checked;
        }

        private void noLogWindowCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            _noLogWindow = noLogWindowCheckBox.Checked;
        }

        private void copyButton_Click(object sender, EventArgs e)
        {
            if (projectListBox.SelectedIndex < 0)
            {
                MessageBox.Show("プロジェクトを選択してください。", "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var source = _projects[projectListBox.SelectedIndex];
            var copy = new ProjectSettings
            {
                Name = source.Name + " (コピー)",
                Type = source.Type,
                Directory = source.Directory,
                HtmlFile = source.HtmlFile,
                NarName = source.NarName,
                ProcessName = source.ProcessName,
                ExcludeName = source.ExcludeName,
                TargetFolder = source.TargetFolder,
                DefaultCheck = source.DefaultCheck,
                UseCommonFtp = source.UseCommonFtp,
                PrivateFtp = new FtpConfiguration
                {
                    Server = source.PrivateFtp.Server,
                    UserId = source.PrivateFtp.UserId,
                    Password = source.PrivateFtp.Password,
                    Passive = source.PrivateFtp.Passive,
                    UseSSL = source.PrivateFtp.UseSSL
                }
            };

            _projects.Add(copy);
            projectListBox.Items.Add(copy.Name, copy.DefaultCheck);
            SaveConfiguration();
        }

        private void moveUpButton_Click(object sender, EventArgs e)
        {
            int idx = projectListBox.SelectedIndex;
            if (idx <= 0)
                return;

            SwapProjects(idx, idx - 1);
            projectListBox.SelectedIndex = idx - 1;
        }

        private void moveDownButton_Click(object sender, EventArgs e)
        {
            int idx = projectListBox.SelectedIndex;
            if (idx < 0 || idx >= _projects.Count - 1)
                return;

            SwapProjects(idx, idx + 1);
            projectListBox.SelectedIndex = idx + 1;
        }

        private void SwapProjects(int idx1, int idx2)
        {
            var temp = _projects[idx1];
            _projects[idx1] = _projects[idx2];
            _projects[idx2] = temp;

            var tempCheck = projectListBox.GetItemChecked(idx1);
            projectListBox.Items[idx1] = _projects[idx1].Name;
            projectListBox.Items[idx2] = _projects[idx2].Name;
            projectListBox.SetItemChecked(idx1, projectListBox.GetItemChecked(idx2));
            projectListBox.SetItemChecked(idx2, tempCheck);

            SaveConfiguration();
        }

        private void SelectByType(SettingType targetType)
        {
            for (int i = 0; i < _projects.Count; i++)
            {
                projectListBox.SetItemChecked(i, _projects[i].Type == targetType);
            }
        }

        private void networkSelectButton_Click(object sender, EventArgs e)
        {
            SelectByType(SettingType.Network);
        }

        private void uploadSelectButton_Click(object sender, EventArgs e)
        {
            SelectByType(SettingType.Upload);
        }

        private void narCreateSelectButton_Click(object sender, EventArgs e)
        {
            SelectByType(SettingType.NarCreate);
        }

        private void exitMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void aboutMenuItem_Click(object sender, EventArgs e)
        {
            using (var aboutDialog = new AboutDialog())
            {
                aboutDialog.ShowDialog(this);
            }
        }
    }
}
