using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace GhostDist.Forms
{
    public partial class AboutDialog : Form
    {
        public AboutDialog()
        {
            InitializeComponent();

            // バージョン情報を設定
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var version = assembly.GetName().Version;
            versionLabel.Text = $"バージョン: {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }

        private void linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                linkLabel.LinkVisited = true;
                Process.Start("https://github.com/ukatech/ghostdist");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"URLを開けませんでした: {ex.Message}", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
