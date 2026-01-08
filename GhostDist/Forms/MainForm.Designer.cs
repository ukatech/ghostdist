namespace GhostDist.Forms
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.CheckedListBox projectListBox;
        private System.Windows.Forms.Button newButton;
        private System.Windows.Forms.Button editButton;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.Button copyButton;
        private System.Windows.Forms.Button moveUpButton;
        private System.Windows.Forms.Button moveDownButton;
        private System.Windows.Forms.Button runButton;
        private System.Windows.Forms.Button allSelectButton;
        private System.Windows.Forms.Button deselectButton;
        private System.Windows.Forms.Button networkSelectButton;
        private System.Windows.Forms.Button uploadSelectButton;
        private System.Windows.Forms.Button narCreateSelectButton;
        private System.Windows.Forms.CheckBox logCheckBox;
        private System.Windows.Forms.CheckBox noLogWindowCheckBox;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutMenuItem;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.projectListBox = new System.Windows.Forms.CheckedListBox();
            this.newButton = new System.Windows.Forms.Button();
            this.editButton = new System.Windows.Forms.Button();
            this.deleteButton = new System.Windows.Forms.Button();
            this.copyButton = new System.Windows.Forms.Button();
            this.moveUpButton = new System.Windows.Forms.Button();
            this.moveDownButton = new System.Windows.Forms.Button();
            this.runButton = new System.Windows.Forms.Button();
            this.allSelectButton = new System.Windows.Forms.Button();
            this.deselectButton = new System.Windows.Forms.Button();
            this.networkSelectButton = new System.Windows.Forms.Button();
            this.uploadSelectButton = new System.Windows.Forms.Button();
            this.narCreateSelectButton = new System.Windows.Forms.Button();
            this.logCheckBox = new System.Windows.Forms.CheckBox();
            this.noLogWindowCheckBox = new System.Windows.Forms.CheckBox();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // projectListBox
            // 
            this.projectListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.projectListBox.CheckOnClick = true;
            this.projectListBox.FormattingEnabled = true;
            this.projectListBox.Location = new System.Drawing.Point(16, 35);
            this.projectListBox.Margin = new System.Windows.Forms.Padding(4);
            this.projectListBox.Name = "projectListBox";
            this.projectListBox.Size = new System.Drawing.Size(334, 514);
            this.projectListBox.TabIndex = 0;
            this.projectListBox.DoubleClick += new System.EventHandler(this.projectListBox_DoubleClick);
            // 
            // newButton
            // 
            this.newButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.newButton.Location = new System.Drawing.Point(358, 35);
            this.newButton.Margin = new System.Windows.Forms.Padding(4);
            this.newButton.Name = "newButton";
            this.newButton.Size = new System.Drawing.Size(156, 29);
            this.newButton.TabIndex = 1;
            this.newButton.Text = "新規作成(&N)...";
            this.newButton.UseVisualStyleBackColor = true;
            this.newButton.Click += new System.EventHandler(this.newButton_Click);
            // 
            // editButton
            // 
            this.editButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.editButton.Location = new System.Drawing.Point(358, 71);
            this.editButton.Margin = new System.Windows.Forms.Padding(4);
            this.editButton.Name = "editButton";
            this.editButton.Size = new System.Drawing.Size(156, 29);
            this.editButton.TabIndex = 2;
            this.editButton.Text = "編集(&E)...";
            this.editButton.UseVisualStyleBackColor = true;
            this.editButton.Click += new System.EventHandler(this.editButton_Click);
            // 
            // deleteButton
            // 
            this.deleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.deleteButton.Location = new System.Drawing.Point(358, 107);
            this.deleteButton.Margin = new System.Windows.Forms.Padding(4);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(156, 29);
            this.deleteButton.TabIndex = 3;
            this.deleteButton.Text = "削除(&D)";
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // copyButton
            // 
            this.copyButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.copyButton.Location = new System.Drawing.Point(358, 143);
            this.copyButton.Margin = new System.Windows.Forms.Padding(4);
            this.copyButton.Name = "copyButton";
            this.copyButton.Size = new System.Drawing.Size(156, 29);
            this.copyButton.TabIndex = 4;
            this.copyButton.Text = "コピー(&C)";
            this.copyButton.UseVisualStyleBackColor = true;
            this.copyButton.Click += new System.EventHandler(this.copyButton_Click);
            // 
            // moveUpButton
            // 
            this.moveUpButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.moveUpButton.Location = new System.Drawing.Point(358, 179);
            this.moveUpButton.Margin = new System.Windows.Forms.Padding(4);
            this.moveUpButton.Name = "moveUpButton";
            this.moveUpButton.Size = new System.Drawing.Size(156, 29);
            this.moveUpButton.TabIndex = 5;
            this.moveUpButton.Text = "上へ移動(&U)";
            this.moveUpButton.UseVisualStyleBackColor = true;
            this.moveUpButton.Click += new System.EventHandler(this.moveUpButton_Click);
            // 
            // moveDownButton
            // 
            this.moveDownButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.moveDownButton.Location = new System.Drawing.Point(358, 215);
            this.moveDownButton.Margin = new System.Windows.Forms.Padding(4);
            this.moveDownButton.Name = "moveDownButton";
            this.moveDownButton.Size = new System.Drawing.Size(156, 29);
            this.moveDownButton.TabIndex = 6;
            this.moveDownButton.Text = "下へ移動(&W)";
            this.moveDownButton.UseVisualStyleBackColor = true;
            this.moveDownButton.Click += new System.EventHandler(this.moveDownButton_Click);
            // 
            // runButton
            // 
            this.runButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.runButton.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Bold);
            this.runButton.Location = new System.Drawing.Point(361, 509);
            this.runButton.Margin = new System.Windows.Forms.Padding(4);
            this.runButton.Name = "runButton";
            this.runButton.Size = new System.Drawing.Size(156, 38);
            this.runButton.TabIndex = 12;
            this.runButton.Text = "実行(&X)";
            this.runButton.UseVisualStyleBackColor = true;
            this.runButton.Click += new System.EventHandler(this.runButton_Click);
            // 
            // allSelectButton
            // 
            this.allSelectButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.allSelectButton.Location = new System.Drawing.Point(358, 260);
            this.allSelectButton.Margin = new System.Windows.Forms.Padding(4);
            this.allSelectButton.Name = "allSelectButton";
            this.allSelectButton.Size = new System.Drawing.Size(156, 29);
            this.allSelectButton.TabIndex = 7;
            this.allSelectButton.Text = "全選択(&A)";
            this.allSelectButton.UseVisualStyleBackColor = true;
            this.allSelectButton.Click += new System.EventHandler(this.allSelectButton_Click);
            // 
            // deselectButton
            // 
            this.deselectButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.deselectButton.Location = new System.Drawing.Point(358, 296);
            this.deselectButton.Margin = new System.Windows.Forms.Padding(4);
            this.deselectButton.Name = "deselectButton";
            this.deselectButton.Size = new System.Drawing.Size(156, 29);
            this.deselectButton.TabIndex = 8;
            this.deselectButton.Text = "選択解除(&S)";
            this.deselectButton.UseVisualStyleBackColor = true;
            this.deselectButton.Click += new System.EventHandler(this.deselectButton_Click);
            // 
            // networkSelectButton
            // 
            this.networkSelectButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.networkSelectButton.Location = new System.Drawing.Point(358, 332);
            this.networkSelectButton.Margin = new System.Windows.Forms.Padding(4);
            this.networkSelectButton.Name = "networkSelectButton";
            this.networkSelectButton.Size = new System.Drawing.Size(156, 29);
            this.networkSelectButton.TabIndex = 9;
            this.networkSelectButton.Text = "更新のみ選択";
            this.networkSelectButton.UseVisualStyleBackColor = true;
            this.networkSelectButton.Click += new System.EventHandler(this.networkSelectButton_Click);
            // 
            // uploadSelectButton
            // 
            this.uploadSelectButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.uploadSelectButton.Location = new System.Drawing.Point(358, 368);
            this.uploadSelectButton.Margin = new System.Windows.Forms.Padding(4);
            this.uploadSelectButton.Name = "uploadSelectButton";
            this.uploadSelectButton.Size = new System.Drawing.Size(156, 29);
            this.uploadSelectButton.TabIndex = 10;
            this.uploadSelectButton.Text = "アップロードのみ選択";
            this.uploadSelectButton.UseVisualStyleBackColor = true;
            this.uploadSelectButton.Click += new System.EventHandler(this.uploadSelectButton_Click);
            // 
            // narCreateSelectButton
            // 
            this.narCreateSelectButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.narCreateSelectButton.Location = new System.Drawing.Point(358, 404);
            this.narCreateSelectButton.Margin = new System.Windows.Forms.Padding(4);
            this.narCreateSelectButton.Name = "narCreateSelectButton";
            this.narCreateSelectButton.Size = new System.Drawing.Size(156, 29);
            this.narCreateSelectButton.TabIndex = 11;
            this.narCreateSelectButton.Text = "NAR作成のみ選択";
            this.narCreateSelectButton.UseVisualStyleBackColor = true;
            this.narCreateSelectButton.Click += new System.EventHandler(this.narCreateSelectButton_Click);
            // 
            // logCheckBox
            // 
            this.logCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.logCheckBox.Location = new System.Drawing.Point(358, 441);
            this.logCheckBox.Margin = new System.Windows.Forms.Padding(4);
            this.logCheckBox.Name = "logCheckBox";
            this.logCheckBox.Size = new System.Drawing.Size(156, 19);
            this.logCheckBox.TabIndex = 13;
            this.logCheckBox.Text = "ログ保存(&L)";
            this.logCheckBox.UseVisualStyleBackColor = true;
            this.logCheckBox.CheckedChanged += new System.EventHandler(this.logCheckBox_CheckedChanged);
            // 
            // noLogWindowCheckBox
            // 
            this.noLogWindowCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.noLogWindowCheckBox.Location = new System.Drawing.Point(358, 469);
            this.noLogWindowCheckBox.Margin = new System.Windows.Forms.Padding(4);
            this.noLogWindowCheckBox.Name = "noLogWindowCheckBox";
            this.noLogWindowCheckBox.Size = new System.Drawing.Size(156, 19);
            this.noLogWindowCheckBox.TabIndex = 14;
            this.noLogWindowCheckBox.Text = "ログ表示しない";
            this.noLogWindowCheckBox.UseVisualStyleBackColor = true;
            this.noLogWindowCheckBox.CheckedChanged += new System.EventHandler(this.noLogWindowCheckBox_CheckedChanged);
            // 
            // menuStrip
            // 
            this.menuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenuItem,
            this.helpMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Padding = new System.Windows.Forms.Padding(5, 2, 0, 2);
            this.menuStrip.Size = new System.Drawing.Size(530, 30);
            this.menuStrip.TabIndex = 15;
            this.menuStrip.Text = "menuStrip";
            // 
            // fileMenuItem
            // 
            this.fileMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitMenuItem});
            this.fileMenuItem.Name = "fileMenuItem";
            this.fileMenuItem.Size = new System.Drawing.Size(82, 24);
            this.fileMenuItem.Text = "ファイル(&F)";
            // 
            // exitMenuItem
            // 
            this.exitMenuItem.Name = "exitMenuItem";
            this.exitMenuItem.Size = new System.Drawing.Size(141, 26);
            this.exitMenuItem.Text = "終了(&X)";
            this.exitMenuItem.Click += new System.EventHandler(this.exitMenuItem_Click);
            // 
            // helpMenuItem
            // 
            this.helpMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutMenuItem});
            this.helpMenuItem.Name = "helpMenuItem";
            this.helpMenuItem.Size = new System.Drawing.Size(79, 24);
            this.helpMenuItem.Text = "ヘルプ(&H)";
            // 
            // aboutMenuItem
            // 
            this.aboutMenuItem.Name = "aboutMenuItem";
            this.aboutMenuItem.Size = new System.Drawing.Size(196, 26);
            this.aboutMenuItem.Text = "バージョン情報(&A)";
            this.aboutMenuItem.Click += new System.EventHandler(this.aboutMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(530, 560);
            this.Controls.Add(this.noLogWindowCheckBox);
            this.Controls.Add(this.logCheckBox);
            this.Controls.Add(this.runButton);
            this.Controls.Add(this.narCreateSelectButton);
            this.Controls.Add(this.uploadSelectButton);
            this.Controls.Add(this.networkSelectButton);
            this.Controls.Add(this.deselectButton);
            this.Controls.Add(this.allSelectButton);
            this.Controls.Add(this.moveDownButton);
            this.Controls.Add(this.moveUpButton);
            this.Controls.Add(this.copyButton);
            this.Controls.Add(this.deleteButton);
            this.Controls.Add(this.editButton);
            this.Controls.Add(this.newButton);
            this.Controls.Add(this.projectListBox);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(527, 363);
            this.Name = "MainForm";
            this.Text = "ゴースト配布系自動化システム";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
