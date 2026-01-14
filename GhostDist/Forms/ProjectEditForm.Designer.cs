namespace GhostDist.Forms
{
    partial class ProjectEditForm
    {
        private System.ComponentModel.IContainer components = null;

        // コントロール定義
        private System.Windows.Forms.RadioButton networkRadio;
        private System.Windows.Forms.RadioButton narUpRadio;
        private System.Windows.Forms.RadioButton narCreateRadio;
        private System.Windows.Forms.TextBox nameEdit;
        private System.Windows.Forms.Label nameLabel;
        private System.Windows.Forms.TextBox targetFolderEdit;
        private System.Windows.Forms.Label targetFolderLabel;
        private System.Windows.Forms.Button targetFolderLocateButton;
        private System.Windows.Forms.GroupBox serverGroupBox;
        private System.Windows.Forms.TextBox serverEdit;
        private System.Windows.Forms.Label serverLabel;
        private System.Windows.Forms.TextBox idEdit;
        private System.Windows.Forms.Label idLabel;
        private System.Windows.Forms.TextBox passwordEdit;
        private System.Windows.Forms.Label passwordLabel;
        private System.Windows.Forms.CheckBox passiveCheck;
        private System.Windows.Forms.CheckBox sslCheck;
        private System.Windows.Forms.CheckBox useDefaultFTPCheck;
        private System.Windows.Forms.Button setDefaultButton;
        private System.Windows.Forms.TextBox upDirEdit;
        private System.Windows.Forms.Label upDirLabel;
        private System.Windows.Forms.TextBox htmlEdit;
        private System.Windows.Forms.Label htmlLabel;
        private System.Windows.Forms.Button htmlLocateButton;
        private System.Windows.Forms.TextBox narNameEdit;
        private System.Windows.Forms.Label narNameLabel;
        private System.Windows.Forms.Button narLocateButton;
        private System.Windows.Forms.GroupBox fileGroupBox;
        private System.Windows.Forms.TextBox processNameMemo;
        private System.Windows.Forms.Label processNameLabel;
        private System.Windows.Forms.TextBox escapeNameMemo;
        private System.Windows.Forms.Label escapeNameLabel;
        private System.Windows.Forms.CheckBox defaultCheckBox;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button previewButton;
        private System.Windows.Forms.RadioButton ftpRadio;
        private System.Windows.Forms.RadioButton nanaloaderRadio;
        private System.Windows.Forms.TextBox ghostIdEdit;
        private System.Windows.Forms.Label ghostIdLabel;
        private System.Windows.Forms.TextBox uploadUrlEdit;
        private System.Windows.Forms.Label uploadUrlLabel;
        private System.Windows.Forms.Button resetProcessNameButton;
        private System.Windows.Forms.Button resetEscapeNameButton;

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
            this.networkRadio = new System.Windows.Forms.RadioButton();
            this.narUpRadio = new System.Windows.Forms.RadioButton();
            this.narCreateRadio = new System.Windows.Forms.RadioButton();
            this.nameEdit = new System.Windows.Forms.TextBox();
            this.nameLabel = new System.Windows.Forms.Label();
            this.targetFolderEdit = new System.Windows.Forms.TextBox();
            this.targetFolderLabel = new System.Windows.Forms.Label();
            this.targetFolderLocateButton = new System.Windows.Forms.Button();
            this.serverGroupBox = new System.Windows.Forms.GroupBox();
            this.setDefaultButton = new System.Windows.Forms.Button();
            this.useDefaultFTPCheck = new System.Windows.Forms.CheckBox();
            this.sslCheck = new System.Windows.Forms.CheckBox();
            this.passiveCheck = new System.Windows.Forms.CheckBox();
            this.passwordEdit = new System.Windows.Forms.TextBox();
            this.passwordLabel = new System.Windows.Forms.Label();
            this.idEdit = new System.Windows.Forms.TextBox();
            this.idLabel = new System.Windows.Forms.Label();
            this.serverEdit = new System.Windows.Forms.TextBox();
            this.serverLabel = new System.Windows.Forms.Label();
            this.ftpRadio = new System.Windows.Forms.RadioButton();
            this.nanaloaderRadio = new System.Windows.Forms.RadioButton();
            this.uploadUrlLabel = new System.Windows.Forms.Label();
            this.uploadUrlEdit = new System.Windows.Forms.TextBox();
            this.ghostIdLabel = new System.Windows.Forms.Label();
            this.ghostIdEdit = new System.Windows.Forms.TextBox();
            this.upDirEdit = new System.Windows.Forms.TextBox();
            this.upDirLabel = new System.Windows.Forms.Label();
            this.htmlEdit = new System.Windows.Forms.TextBox();
            this.htmlLabel = new System.Windows.Forms.Label();
            this.htmlLocateButton = new System.Windows.Forms.Button();
            this.narNameEdit = new System.Windows.Forms.TextBox();
            this.narNameLabel = new System.Windows.Forms.Label();
            this.narLocateButton = new System.Windows.Forms.Button();
            this.fileGroupBox = new System.Windows.Forms.GroupBox();
            this.escapeNameMemo = new System.Windows.Forms.TextBox();
            this.escapeNameLabel = new System.Windows.Forms.Label();
            this.processNameMemo = new System.Windows.Forms.TextBox();
            this.processNameLabel = new System.Windows.Forms.Label();
            this.resetProcessNameButton = new System.Windows.Forms.Button();
            this.resetEscapeNameButton = new System.Windows.Forms.Button();
            this.previewButton = new System.Windows.Forms.Button();
            this.defaultCheckBox = new System.Windows.Forms.CheckBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.serverGroupBox.SuspendLayout();
            this.fileGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // networkRadio
            // 
            this.networkRadio.AutoSize = true;
            this.networkRadio.Checked = true;
            this.networkRadio.Location = new System.Drawing.Point(203, 45);
            this.networkRadio.Margin = new System.Windows.Forms.Padding(4);
            this.networkRadio.Name = "networkRadio";
            this.networkRadio.Size = new System.Drawing.Size(250, 19);
            this.networkRadio.TabIndex = 2;
            this.networkRadio.TabStop = true;
            this.networkRadio.Text = "ネットワーク更新ファイルアップロード(&N)";
            this.networkRadio.UseVisualStyleBackColor = true;
            this.networkRadio.CheckedChanged += new System.EventHandler(this.networkRadio_CheckedChanged);
            // 
            // narUpRadio
            // 
            this.narUpRadio.AutoSize = true;
            this.narUpRadio.Location = new System.Drawing.Point(203, 72);
            this.narUpRadio.Margin = new System.Windows.Forms.Padding(4);
            this.narUpRadio.Name = "narUpRadio";
            this.narUpRadio.Size = new System.Drawing.Size(311, 19);
            this.narUpRadio.TabIndex = 3;
            this.narUpRadio.Text = "ゴーストアーカイブ(Nar)作成/FTPアップロード(&U)";
            this.narUpRadio.UseVisualStyleBackColor = true;
            this.narUpRadio.CheckedChanged += new System.EventHandler(this.narUpRadio_CheckedChanged);
            // 
            // narCreateRadio
            // 
            this.narCreateRadio.AutoSize = true;
            this.narCreateRadio.Location = new System.Drawing.Point(203, 99);
            this.narCreateRadio.Margin = new System.Windows.Forms.Padding(4);
            this.narCreateRadio.Name = "narCreateRadio";
            this.narCreateRadio.Size = new System.Drawing.Size(212, 19);
            this.narCreateRadio.TabIndex = 4;
            this.narCreateRadio.Text = "ゴーストアーカイブ(Nar)作成(&C)";
            this.narCreateRadio.UseVisualStyleBackColor = true;
            this.narCreateRadio.CheckedChanged += new System.EventHandler(this.narCreateRadio_CheckedChanged);
            // 
            // nameEdit
            // 
            this.nameEdit.Location = new System.Drawing.Point(203, 13);
            this.nameEdit.Margin = new System.Windows.Forms.Padding(4);
            this.nameEdit.Name = "nameEdit";
            this.nameEdit.Size = new System.Drawing.Size(363, 22);
            this.nameEdit.TabIndex = 1;
            // 
            // nameLabel
            // 
            this.nameLabel.AutoSize = true;
            this.nameLabel.Location = new System.Drawing.Point(16, 17);
            this.nameLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.nameLabel.Name = "nameLabel";
            this.nameLabel.Size = new System.Drawing.Size(60, 15);
            this.nameLabel.TabIndex = 0;
            this.nameLabel.Text = "名称(&N):";
            // 
            // targetFolderEdit
            // 
            this.targetFolderEdit.Location = new System.Drawing.Point(203, 130);
            this.targetFolderEdit.Margin = new System.Windows.Forms.Padding(4);
            this.targetFolderEdit.Name = "targetFolderEdit";
            this.targetFolderEdit.Size = new System.Drawing.Size(501, 22);
            this.targetFolderEdit.TabIndex = 6;
            // 
            // targetFolderLabel
            // 
            this.targetFolderLabel.AutoSize = true;
            this.targetFolderLabel.Location = new System.Drawing.Point(16, 134);
            this.targetFolderLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.targetFolderLabel.Name = "targetFolderLabel";
            this.targetFolderLabel.Size = new System.Drawing.Size(102, 15);
            this.targetFolderLabel.TabIndex = 5;
            this.targetFolderLabel.Text = "基準フォルダ(&T):";
            // 
            // targetFolderLocateButton
            // 
            this.targetFolderLocateButton.Location = new System.Drawing.Point(715, 127);
            this.targetFolderLocateButton.Margin = new System.Windows.Forms.Padding(4);
            this.targetFolderLocateButton.Name = "targetFolderLocateButton";
            this.targetFolderLocateButton.Size = new System.Drawing.Size(100, 29);
            this.targetFolderLocateButton.TabIndex = 7;
            this.targetFolderLocateButton.Text = "参照...";
            this.targetFolderLocateButton.UseVisualStyleBackColor = true;
            this.targetFolderLocateButton.Click += new System.EventHandler(this.targetFolderLocateButton_Click);
            // 
            // serverGroupBox
            // 
            this.serverGroupBox.Controls.Add(this.setDefaultButton);
            this.serverGroupBox.Controls.Add(this.useDefaultFTPCheck);
            this.serverGroupBox.Controls.Add(this.sslCheck);
            this.serverGroupBox.Controls.Add(this.passiveCheck);
            this.serverGroupBox.Controls.Add(this.passwordEdit);
            this.serverGroupBox.Controls.Add(this.passwordLabel);
            this.serverGroupBox.Controls.Add(this.idEdit);
            this.serverGroupBox.Controls.Add(this.idLabel);
            this.serverGroupBox.Controls.Add(this.serverEdit);
            this.serverGroupBox.Controls.Add(this.serverLabel);
            this.serverGroupBox.Controls.Add(this.ftpRadio);
            this.serverGroupBox.Controls.Add(this.nanaloaderRadio);
            this.serverGroupBox.Controls.Add(this.uploadUrlLabel);
            this.serverGroupBox.Controls.Add(this.uploadUrlEdit);
            this.serverGroupBox.Location = new System.Drawing.Point(11, 292);
            this.serverGroupBox.Margin = new System.Windows.Forms.Padding(4);
            this.serverGroupBox.Name = "serverGroupBox";
            this.serverGroupBox.Padding = new System.Windows.Forms.Padding(4);
            this.serverGroupBox.Size = new System.Drawing.Size(812, 188);
            this.serverGroupBox.TabIndex = 15;
            this.serverGroupBox.TabStop = false;
            this.serverGroupBox.Text = "サーバー設定";
            // 
            // setDefaultButton
            // 
            this.setDefaultButton.Location = new System.Drawing.Point(425, 16);
            this.setDefaultButton.Margin = new System.Windows.Forms.Padding(4);
            this.setDefaultButton.Name = "setDefaultButton";
            this.setDefaultButton.Size = new System.Drawing.Size(247, 29);
            this.setDefaultButton.TabIndex = 9;
            this.setDefaultButton.Text = "この設定を共通設定にする(&U)";
            this.setDefaultButton.UseVisualStyleBackColor = true;
            this.setDefaultButton.Click += new System.EventHandler(this.setDefaultButton_Click);
            // 
            // useDefaultFTPCheck
            // 
            this.useDefaultFTPCheck.AutoSize = true;
            this.useDefaultFTPCheck.Location = new System.Drawing.Point(262, 24);
            this.useDefaultFTPCheck.Margin = new System.Windows.Forms.Padding(4);
            this.useDefaultFTPCheck.Name = "useDefaultFTPCheck";
            this.useDefaultFTPCheck.Size = new System.Drawing.Size(142, 19);
            this.useDefaultFTPCheck.TabIndex = 8;
            this.useDefaultFTPCheck.Text = "共通設定を使う(&F)";
            this.useDefaultFTPCheck.UseVisualStyleBackColor = true;
            this.useDefaultFTPCheck.CheckedChanged += new System.EventHandler(this.useDefaultFTPCheck_CheckedChanged);
            // 
            // sslCheck
            // 
            this.sslCheck.AutoSize = true;
            this.sslCheck.Location = new System.Drawing.Point(570, 68);
            this.sslCheck.Margin = new System.Windows.Forms.Padding(4);
            this.sslCheck.Name = "sslCheck";
            this.sslCheck.Size = new System.Drawing.Size(73, 19);
            this.sslCheck.TabIndex = 7;
            this.sslCheck.Text = "SSL(&L)";
            this.sslCheck.UseVisualStyleBackColor = true;
            // 
            // passiveCheck
            // 
            this.passiveCheck.AutoSize = true;
            this.passiveCheck.Location = new System.Drawing.Point(425, 67);
            this.passiveCheck.Margin = new System.Windows.Forms.Padding(4);
            this.passiveCheck.Name = "passiveCheck";
            this.passiveCheck.Size = new System.Drawing.Size(126, 19);
            this.passiveCheck.TabIndex = 6;
            this.passiveCheck.Text = "パッシブモード(&V)";
            this.passiveCheck.UseVisualStyleBackColor = true;
            // 
            // passwordEdit
            // 
            this.passwordEdit.Location = new System.Drawing.Point(121, 155);
            this.passwordEdit.Margin = new System.Windows.Forms.Padding(4);
            this.passwordEdit.Name = "passwordEdit";
            this.passwordEdit.PasswordChar = '*';
            this.passwordEdit.Size = new System.Drawing.Size(160, 22);
            this.passwordEdit.TabIndex = 5;
            // 
            // passwordLabel
            // 
            this.passwordLabel.AutoSize = true;
            this.passwordLabel.Location = new System.Drawing.Point(11, 158);
            this.passwordLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.passwordLabel.Name = "passwordLabel";
            this.passwordLabel.Size = new System.Drawing.Size(86, 15);
            this.passwordLabel.TabIndex = 4;
            this.passwordLabel.Text = "パスワード(&P):";
            // 
            // idEdit
            // 
            this.idEdit.Location = new System.Drawing.Point(121, 125);
            this.idEdit.Margin = new System.Windows.Forms.Padding(4);
            this.idEdit.Name = "idEdit";
            this.idEdit.Size = new System.Drawing.Size(160, 22);
            this.idEdit.TabIndex = 3;
            // 
            // idLabel
            // 
            this.idLabel.AutoSize = true;
            this.idLabel.Location = new System.Drawing.Point(11, 128);
            this.idLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.idLabel.Name = "idLabel";
            this.idLabel.Size = new System.Drawing.Size(24, 15);
            this.idLabel.TabIndex = 2;
            this.idLabel.Text = "ID:";
            // 
            // serverEdit
            // 
            this.serverEdit.Location = new System.Drawing.Point(121, 65);
            this.serverEdit.Margin = new System.Windows.Forms.Padding(4);
            this.serverEdit.Name = "serverEdit";
            this.serverEdit.Size = new System.Drawing.Size(288, 22);
            this.serverEdit.TabIndex = 1;
            // 
            // serverLabel
            // 
            this.serverLabel.AutoSize = true;
            this.serverLabel.Location = new System.Drawing.Point(11, 68);
            this.serverLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.serverLabel.Name = "serverLabel";
            this.serverLabel.Size = new System.Drawing.Size(78, 15);
            this.serverLabel.TabIndex = 0;
            this.serverLabel.Text = "サーバー(&S):";
            // 
            // ftpRadio
            // 
            this.ftpRadio.AutoSize = true;
            this.ftpRadio.Checked = true;
            this.ftpRadio.Location = new System.Drawing.Point(11, 23);
            this.ftpRadio.Margin = new System.Windows.Forms.Padding(4);
            this.ftpRadio.Name = "ftpRadio";
            this.ftpRadio.Size = new System.Drawing.Size(72, 19);
            this.ftpRadio.TabIndex = 10;
            this.ftpRadio.TabStop = true;
            this.ftpRadio.Text = "FTP(&F)";
            this.ftpRadio.UseVisualStyleBackColor = true;
            this.ftpRadio.CheckedChanged += new System.EventHandler(this.ftpRadio_CheckedChanged);
            // 
            // nanaloaderRadio
            // 
            this.nanaloaderRadio.AutoSize = true;
            this.nanaloaderRadio.Location = new System.Drawing.Point(110, 23);
            this.nanaloaderRadio.Margin = new System.Windows.Forms.Padding(4);
            this.nanaloaderRadio.Name = "nanaloaderRadio";
            this.nanaloaderRadio.Size = new System.Drawing.Size(102, 19);
            this.nanaloaderRadio.TabIndex = 11;
            this.nanaloaderRadio.Text = "ななろだ (&N)";
            this.nanaloaderRadio.UseVisualStyleBackColor = true;
            this.nanaloaderRadio.CheckedChanged += new System.EventHandler(this.nanaloaderRadio_CheckedChanged);
            // 
            // uploadUrlLabel
            // 
            this.uploadUrlLabel.AutoSize = true;
            this.uploadUrlLabel.Location = new System.Drawing.Point(11, 98);
            this.uploadUrlLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.uploadUrlLabel.Name = "uploadUrlLabel";
            this.uploadUrlLabel.Size = new System.Drawing.Size(102, 15);
            this.uploadUrlLabel.TabIndex = 14;
            this.uploadUrlLabel.Text = "アップロードURL:";
            // 
            // uploadUrlEdit
            // 
            this.uploadUrlEdit.Location = new System.Drawing.Point(121, 95);
            this.uploadUrlEdit.Margin = new System.Windows.Forms.Padding(4);
            this.uploadUrlEdit.Name = "uploadUrlEdit";
            this.uploadUrlEdit.Size = new System.Drawing.Size(620, 22);
            this.uploadUrlEdit.TabIndex = 15;
            // 
            // ghostIdLabel
            // 
            this.ghostIdLabel.AutoSize = true;
            this.ghostIdLabel.Location = new System.Drawing.Point(13, 253);
            this.ghostIdLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.ghostIdLabel.Name = "ghostIdLabel";
            this.ghostIdLabel.Size = new System.Drawing.Size(118, 15);
            this.ghostIdLabel.TabIndex = 12;
            this.ghostIdLabel.Text = "ななろだアイテムID:";
            // 
            // ghostIdEdit
            // 
            this.ghostIdEdit.Location = new System.Drawing.Point(203, 250);
            this.ghostIdEdit.Margin = new System.Windows.Forms.Padding(4);
            this.ghostIdEdit.Name = "ghostIdEdit";
            this.ghostIdEdit.Size = new System.Drawing.Size(311, 22);
            this.ghostIdEdit.TabIndex = 13;
            // 
            // upDirEdit
            // 
            this.upDirEdit.Location = new System.Drawing.Point(203, 160);
            this.upDirEdit.Margin = new System.Windows.Forms.Padding(4);
            this.upDirEdit.Name = "upDirEdit";
            this.upDirEdit.Size = new System.Drawing.Size(311, 22);
            this.upDirEdit.TabIndex = 9;
            this.upDirEdit.Text = "/";
            // 
            // upDirLabel
            // 
            this.upDirLabel.AutoSize = true;
            this.upDirLabel.Location = new System.Drawing.Point(16, 164);
            this.upDirLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.upDirLabel.Name = "upDirLabel";
            this.upDirLabel.Size = new System.Drawing.Size(171, 15);
            this.upDirLabel.TabIndex = 8;
            this.upDirLabel.Text = "アップロード先ディレクトリ(&D):";
            // 
            // htmlEdit
            // 
            this.htmlEdit.Location = new System.Drawing.Point(203, 190);
            this.htmlEdit.Margin = new System.Windows.Forms.Padding(4);
            this.htmlEdit.Name = "htmlEdit";
            this.htmlEdit.Size = new System.Drawing.Size(501, 22);
            this.htmlEdit.TabIndex = 11;
            // 
            // htmlLabel
            // 
            this.htmlLabel.AutoSize = true;
            this.htmlLabel.Location = new System.Drawing.Point(16, 193);
            this.htmlLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.htmlLabel.Name = "htmlLabel";
            this.htmlLabel.Size = new System.Drawing.Size(176, 15);
            this.htmlLabel.TabIndex = 10;
            this.htmlLabel.Text = "配布ページHTMLファイル(&H):";
            // 
            // htmlLocateButton
            // 
            this.htmlLocateButton.Location = new System.Drawing.Point(715, 187);
            this.htmlLocateButton.Margin = new System.Windows.Forms.Padding(4);
            this.htmlLocateButton.Name = "htmlLocateButton";
            this.htmlLocateButton.Size = new System.Drawing.Size(100, 29);
            this.htmlLocateButton.TabIndex = 12;
            this.htmlLocateButton.Text = "参照...";
            this.htmlLocateButton.UseVisualStyleBackColor = true;
            this.htmlLocateButton.Click += new System.EventHandler(this.htmlLocateButton_Click);
            // 
            // narNameEdit
            // 
            this.narNameEdit.Location = new System.Drawing.Point(203, 220);
            this.narNameEdit.Margin = new System.Windows.Forms.Padding(4);
            this.narNameEdit.Name = "narNameEdit";
            this.narNameEdit.Size = new System.Drawing.Size(501, 22);
            this.narNameEdit.TabIndex = 14;
            // 
            // narNameLabel
            // 
            this.narNameLabel.AutoSize = true;
            this.narNameLabel.Location = new System.Drawing.Point(16, 223);
            this.narNameLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.narNameLabel.Name = "narNameLabel";
            this.narNameLabel.Size = new System.Drawing.Size(130, 15);
            this.narNameLabel.TabIndex = 13;
            this.narNameLabel.Text = "nar/zipファイル名(&F):";
            // 
            // narLocateButton
            // 
            this.narLocateButton.Location = new System.Drawing.Point(715, 217);
            this.narLocateButton.Margin = new System.Windows.Forms.Padding(4);
            this.narLocateButton.Name = "narLocateButton";
            this.narLocateButton.Size = new System.Drawing.Size(100, 29);
            this.narLocateButton.TabIndex = 15;
            this.narLocateButton.Text = "参照...";
            this.narLocateButton.UseVisualStyleBackColor = true;
            this.narLocateButton.Click += new System.EventHandler(this.narLocateButton_Click);
            // 
            // fileGroupBox
            // 
            this.fileGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fileGroupBox.Controls.Add(this.escapeNameMemo);
            this.fileGroupBox.Controls.Add(this.escapeNameLabel);
            this.fileGroupBox.Controls.Add(this.processNameMemo);
            this.fileGroupBox.Controls.Add(this.processNameLabel);
            this.fileGroupBox.Controls.Add(this.resetProcessNameButton);
            this.fileGroupBox.Controls.Add(this.resetEscapeNameButton);
            this.fileGroupBox.Location = new System.Drawing.Point(11, 488);
            this.fileGroupBox.Margin = new System.Windows.Forms.Padding(4);
            this.fileGroupBox.Name = "fileGroupBox";
            this.fileGroupBox.Padding = new System.Windows.Forms.Padding(4);
            this.fileGroupBox.Size = new System.Drawing.Size(812, 318);
            this.fileGroupBox.TabIndex = 16;
            this.fileGroupBox.TabStop = false;
            this.fileGroupBox.Text = "対象ファイル";
            // 
            // escapeNameMemo
            // 
            this.escapeNameMemo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.escapeNameMemo.Location = new System.Drawing.Point(405, 50);
            this.escapeNameMemo.Margin = new System.Windows.Forms.Padding(4);
            this.escapeNameMemo.Multiline = true;
            this.escapeNameMemo.Name = "escapeNameMemo";
            this.escapeNameMemo.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.escapeNameMemo.Size = new System.Drawing.Size(395, 257);
            this.escapeNameMemo.TabIndex = 3;
            // 
            // escapeNameLabel
            // 
            this.escapeNameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.escapeNameLabel.AutoSize = true;
            this.escapeNameLabel.Location = new System.Drawing.Point(405, 26);
            this.escapeNameLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.escapeNameLabel.Name = "escapeNameLabel";
            this.escapeNameLabel.Size = new System.Drawing.Size(146, 15);
            this.escapeNameLabel.TabIndex = 2;
            this.escapeNameLabel.Text = "除外ファイル名条件(&R):";
            // 
            // processNameMemo
            // 
            this.processNameMemo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.processNameMemo.Location = new System.Drawing.Point(11, 50);
            this.processNameMemo.Margin = new System.Windows.Forms.Padding(4);
            this.processNameMemo.Multiline = true;
            this.processNameMemo.Name = "processNameMemo";
            this.processNameMemo.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.processNameMemo.Size = new System.Drawing.Size(373, 257);
            this.processNameMemo.TabIndex = 1;
            // 
            // processNameLabel
            // 
            this.processNameLabel.AutoSize = true;
            this.processNameLabel.Location = new System.Drawing.Point(11, 26);
            this.processNameLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.processNameLabel.Name = "processNameLabel";
            this.processNameLabel.Size = new System.Drawing.Size(146, 15);
            this.processNameLabel.TabIndex = 0;
            this.processNameLabel.Text = "処理ファイル名条件(&P):";
            // 
            // resetProcessNameButton
            // 
            this.resetProcessNameButton.Location = new System.Drawing.Point(165, 17);
            this.resetProcessNameButton.Margin = new System.Windows.Forms.Padding(4);
            this.resetProcessNameButton.Name = "resetProcessNameButton";
            this.resetProcessNameButton.Size = new System.Drawing.Size(80, 29);
            this.resetProcessNameButton.TabIndex = 4;
            this.resetProcessNameButton.Text = "リセット";
            this.resetProcessNameButton.UseVisualStyleBackColor = true;
            this.resetProcessNameButton.Click += new System.EventHandler(this.resetProcessNameButton_Click);
            // 
            // resetEscapeNameButton
            // 
            this.resetEscapeNameButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.resetEscapeNameButton.Location = new System.Drawing.Point(559, 17);
            this.resetEscapeNameButton.Margin = new System.Windows.Forms.Padding(4);
            this.resetEscapeNameButton.Name = "resetEscapeNameButton";
            this.resetEscapeNameButton.Size = new System.Drawing.Size(80, 29);
            this.resetEscapeNameButton.TabIndex = 5;
            this.resetEscapeNameButton.Text = "リセット";
            this.resetEscapeNameButton.UseVisualStyleBackColor = true;
            this.resetEscapeNameButton.Click += new System.EventHandler(this.resetEscapeNameButton_Click);
            // 
            // previewButton
            // 
            this.previewButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.previewButton.Location = new System.Drawing.Point(13, 817);
            this.previewButton.Margin = new System.Windows.Forms.Padding(4);
            this.previewButton.Name = "previewButton";
            this.previewButton.Size = new System.Drawing.Size(284, 29);
            this.previewButton.TabIndex = 4;
            this.previewButton.Text = "フィルタリング結果をプレビュー(&K)";
            this.previewButton.UseVisualStyleBackColor = true;
            this.previewButton.Click += new System.EventHandler(this.previewButton_Click);
            // 
            // defaultCheckBox
            // 
            this.defaultCheckBox.AutoSize = true;
            this.defaultCheckBox.Checked = true;
            this.defaultCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.defaultCheckBox.Location = new System.Drawing.Point(587, 17);
            this.defaultCheckBox.Margin = new System.Windows.Forms.Padding(4);
            this.defaultCheckBox.Name = "defaultCheckBox";
            this.defaultCheckBox.Size = new System.Drawing.Size(169, 19);
            this.defaultCheckBox.TabIndex = 2;
            this.defaultCheckBox.Text = "起動時にチェックする(&R)";
            this.defaultCheckBox.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.Location = new System.Drawing.Point(708, 817);
            this.okButton.Margin = new System.Windows.Forms.Padding(4);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(115, 29);
            this.okButton.TabIndex = 17;
            this.okButton.Text = "&OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(585, 817);
            this.cancelButton.Margin = new System.Windows.Forms.Padding(4);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(115, 29);
            this.cancelButton.TabIndex = 18;
            this.cancelButton.Text = "キャンセル(&C)";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // ProjectEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(839, 859);
            this.Controls.Add(this.previewButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.defaultCheckBox);
            this.Controls.Add(this.fileGroupBox);
            this.Controls.Add(this.narLocateButton);
            this.Controls.Add(this.narNameLabel);
            this.Controls.Add(this.narNameEdit);
            this.Controls.Add(this.htmlLocateButton);
            this.Controls.Add(this.htmlLabel);
            this.Controls.Add(this.htmlEdit);
            this.Controls.Add(this.upDirLabel);
            this.Controls.Add(this.ghostIdLabel);
            this.Controls.Add(this.upDirEdit);
            this.Controls.Add(this.ghostIdEdit);
            this.Controls.Add(this.serverGroupBox);
            this.Controls.Add(this.targetFolderLocateButton);
            this.Controls.Add(this.targetFolderLabel);
            this.Controls.Add(this.targetFolderEdit);
            this.Controls.Add(this.nameLabel);
            this.Controls.Add(this.nameEdit);
            this.Controls.Add(this.narCreateRadio);
            this.Controls.Add(this.narUpRadio);
            this.Controls.Add(this.networkRadio);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProjectEditForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "プロジェクト編集";
            this.serverGroupBox.ResumeLayout(false);
            this.serverGroupBox.PerformLayout();
            this.fileGroupBox.ResumeLayout(false);
            this.fileGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
