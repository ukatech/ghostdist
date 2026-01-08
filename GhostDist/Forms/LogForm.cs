using System;
using System.IO;
using System.Windows.Forms;

namespace GhostDist.Forms
{
    public partial class LogForm : Form
    {
        private string _logFilePath;
        public bool IsLog { get; set; }

        public LogForm()
        {
            InitializeComponent();

            // ログファイルパスを生成
            var appDir = Path.GetDirectoryName(Application.ExecutablePath);
            var logDir = Path.Combine(appDir, "log");
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            _logFilePath = Path.Combine(logDir, $"log{timestamp}.log");
        }

        /// <summary>
        /// ログメッセージを追加
        /// </summary>
        public void AddLog(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(AddLog), message);
                return;
            }

            logTextBox.AppendText(message + Environment.NewLine);

            // ログファイルに保存
            if (IsLog)
            {
                try
                {
                    var dir = Path.GetDirectoryName(_logFilePath);
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }

                    File.AppendAllText(_logFilePath, message + Environment.NewLine);
                }
                catch (Exception)
                {
                    // ログ保存エラーは無視
                }
            }
        }

        /// <summary>
        /// プログレスバーの値を設定
        /// </summary>
        public void SetProgress(long current, long total)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<long, long>(SetProgress), current, total);
                return;
            }

            if (total > 0)
            {
                // 大きなファイルでもオーバーフローしないようにパーセンテージで表示
                progressBar.Maximum = 100;
                var percent = (int)((current * 100) / total);
                progressBar.Value = Math.Min(percent, 100);
            }
        }

        /// <summary>
        /// 閉じるボタンの有効/無効を設定
        /// </summary>
        public void EnableCloseButton(bool enable)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<bool>(EnableCloseButton), enable);
                return;
            }

            ControlBox = enable;
        }
    }
}
