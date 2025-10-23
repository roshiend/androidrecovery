namespace AndroidFileRecovery;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();
        this.menuStrip1 = new MenuStrip();
        this.fileToolStripMenuItem = new ToolStripMenuItem();
        this.exitToolStripMenuItem = new ToolStripMenuItem();
        this.toolsToolStripMenuItem = new ToolStripMenuItem();
        this.viewStatisticsToolStripMenuItem = new ToolStripMenuItem();
        this.viewLogToolStripMenuItem = new ToolStripMenuItem();
        this.helpToolStripMenuItem = new ToolStripMenuItem();
        this.aboutToolStripMenuItem = new ToolStripMenuItem();
        this.statusStrip1 = new StatusStrip();
        this.statusLabel = new ToolStripStatusLabel();
        this.progressBar = new ToolStripProgressBar();
        this.splitContainer1 = new SplitContainer();
        this.groupBox1 = new GroupBox();
        this.btnRefreshDevices = new Button();
        this.btnTestDetection = new Button();
        this.btnToggleMonitoring = new Button();
        this.btnConnectDevice = new Button();
        this.lstDevices = new ListBox();
        this.lblDeviceStatus = new Label();
        this.groupBox2 = new GroupBox();
        this.btnStopRecovery = new Button();
        this.btnStartRecovery = new Button();
        this.btnCopyAll = new Button();
        this.btnCopySelected = new Button();
        this.btnClearList = new Button();
        this.chkIncludeVideos = new CheckBox();
        this.chkIncludeImages = new CheckBox();
        this.txtRecoveryPath = new TextBox();
        this.btnBrowsePath = new Button();
        this.lblRecoveryPath = new Label();
        this.groupBox3 = new GroupBox();
        this.lstRecoveredFiles = new ListView();
        this.colFileName = new ColumnHeader();
        this.colFileSize = new ColumnHeader();
        this.colFileType = new ColumnHeader();
        this.colRecoveryDate = new ColumnHeader();
        this.colStatus = new ColumnHeader();
        this.contextMenuStrip1 = new ContextMenuStrip();
        this.previewToolStripMenuItem = new ToolStripMenuItem();
        this.recoverToolStripMenuItem = new ToolStripMenuItem();
        this.toolStripSeparator1 = new ToolStripSeparator();
        this.deleteToolStripMenuItem = new ToolStripMenuItem();
        this.pictureBox1 = new PictureBox();
        this.lblPreview = new Label();
        this.timer1 = new System.Windows.Forms.Timer(this.components);
        this.menuStrip1.SuspendLayout();
        this.statusStrip1.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
        this.splitContainer1.Panel1.SuspendLayout();
        this.splitContainer1.Panel2.SuspendLayout();
        this.splitContainer1.SuspendLayout();
        this.groupBox1.SuspendLayout();
        this.groupBox2.SuspendLayout();
        this.groupBox3.SuspendLayout();
        this.contextMenuStrip1.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
        this.SuspendLayout();
        // 
        // menuStrip1
        // 
        this.menuStrip1.Items.AddRange(new ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolsToolStripMenuItem,
            this.helpToolStripMenuItem});
        this.menuStrip1.Location = new Point(0, 0);
        this.menuStrip1.Name = "menuStrip1";
        this.menuStrip1.Size = new Size(1200, 24);
        this.menuStrip1.TabIndex = 0;
        this.menuStrip1.Text = "menuStrip1";
        // 
        // fileToolStripMenuItem
        // 
        this.fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
            this.exitToolStripMenuItem});
        this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
        this.fileToolStripMenuItem.Size = new Size(37, 20);
        this.fileToolStripMenuItem.Text = "&File";
        // 
        // exitToolStripMenuItem
        // 
        this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
        this.exitToolStripMenuItem.Size = new Size(93, 22);
        this.exitToolStripMenuItem.Text = "E&xit";
        this.exitToolStripMenuItem.Click += new EventHandler(this.exitToolStripMenuItem_Click);
        // 
        // toolsToolStripMenuItem
        // 
        this.toolsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
            this.viewStatisticsToolStripMenuItem,
            this.viewLogToolStripMenuItem});
        this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
        this.toolsToolStripMenuItem.Size = new Size(46, 20);
        this.toolsToolStripMenuItem.Text = "&Tools";
        // 
        // viewStatisticsToolStripMenuItem
        // 
        this.viewStatisticsToolStripMenuItem.Name = "viewStatisticsToolStripMenuItem";
        this.viewStatisticsToolStripMenuItem.Size = new Size(180, 22);
        this.viewStatisticsToolStripMenuItem.Text = "View &Statistics";
        this.viewStatisticsToolStripMenuItem.Click += new EventHandler(this.viewStatisticsToolStripMenuItem_Click);
        // 
        // viewLogToolStripMenuItem
        // 
        this.viewLogToolStripMenuItem.Name = "viewLogToolStripMenuItem";
        this.viewLogToolStripMenuItem.Size = new Size(180, 22);
        this.viewLogToolStripMenuItem.Text = "View &Log";
        this.viewLogToolStripMenuItem.Click += new EventHandler(this.viewLogToolStripMenuItem_Click);
        // 
        // helpToolStripMenuItem
        // 
        this.helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
            this.aboutToolStripMenuItem});
        this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
        this.helpToolStripMenuItem.Size = new Size(44, 20);
        this.helpToolStripMenuItem.Text = "&Help";
        // 
        // aboutToolStripMenuItem
        // 
        this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
        this.aboutToolStripMenuItem.Size = new Size(107, 22);
        this.aboutToolStripMenuItem.Text = "&About";
        this.aboutToolStripMenuItem.Click += new EventHandler(this.aboutToolStripMenuItem_Click);
        // 
        // statusStrip1
        // 
        this.statusStrip1.Items.AddRange(new ToolStripItem[] {
            this.statusLabel,
            this.progressBar});
        this.statusStrip1.Location = new Point(0, 728);
        this.statusStrip1.Name = "statusStrip1";
        this.statusStrip1.Size = new Size(1200, 22);
        this.statusStrip1.TabIndex = 1;
        this.statusStrip1.Text = "statusStrip1";
        // 
        // statusLabel
        // 
        this.statusLabel.Name = "statusLabel";
        this.statusLabel.Size = new Size(39, 17);
        this.statusLabel.Text = "Ready";
        // 
        // progressBar
        // 
        this.progressBar.Name = "progressBar";
        this.progressBar.Size = new Size(200, 16);
        this.progressBar.Visible = false;
        // 
        // splitContainer1
        // 
        this.splitContainer1.Dock = DockStyle.Fill;
        this.splitContainer1.Location = new Point(0, 24);
        this.splitContainer1.Name = "splitContainer1";
        // 
        // splitContainer1.Panel1
        // 
        this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
        this.splitContainer1.Panel1.Controls.Add(this.groupBox2);
        this.splitContainer1.Panel1.Controls.Add(this.groupBox3);
        // 
        // splitContainer1.Panel2
        // 
        this.splitContainer1.Panel2.Controls.Add(this.pictureBox1);
        this.splitContainer1.Panel2.Controls.Add(this.lblPreview);
        this.splitContainer1.Size = new Size(1200, 704);
        this.splitContainer1.SplitterDistance = 800;
        this.splitContainer1.TabIndex = 2;
        // 
        // groupBox1
        // 
        this.groupBox1.Controls.Add(this.btnRefreshDevices);
        this.groupBox1.Controls.Add(this.btnTestDetection);
        this.groupBox1.Controls.Add(this.btnToggleMonitoring);
        this.groupBox1.Controls.Add(this.btnConnectDevice);
        this.groupBox1.Controls.Add(this.lstDevices);
        this.groupBox1.Controls.Add(this.lblDeviceStatus);
        this.groupBox1.Dock = DockStyle.Top;
        this.groupBox1.Location = new Point(0, 0);
        this.groupBox1.Name = "groupBox1";
        this.groupBox1.Size = new Size(800, 200);
        this.groupBox1.TabIndex = 0;
        this.groupBox1.TabStop = false;
        this.groupBox1.Text = "Android Device Connection";
        // 
        // btnRefreshDevices
        // 
        this.btnRefreshDevices.Anchor = ((AnchorStyles)((AnchorStyles.Bottom | AnchorStyles.Left)));
        this.btnRefreshDevices.Location = new Point(12, 160);
        this.btnRefreshDevices.Name = "btnRefreshDevices";
        this.btnRefreshDevices.Size = new Size(100, 30);
        this.btnRefreshDevices.TabIndex = 3;
        this.btnRefreshDevices.Text = "Refresh Devices";
        this.btnRefreshDevices.UseVisualStyleBackColor = true;
        this.btnRefreshDevices.Click += new EventHandler(this.btnRefreshDevices_Click);
        // 
        // btnTestDetection
        // 
        this.btnTestDetection.Anchor = ((AnchorStyles)((AnchorStyles.Bottom | AnchorStyles.Left)));
        this.btnTestDetection.Location = new Point(130, 160);
        this.btnTestDetection.Name = "btnTestDetection";
        this.btnTestDetection.Size = new Size(100, 30);
        this.btnTestDetection.TabIndex = 4;
        this.btnTestDetection.Text = "Test Detection";
        this.btnTestDetection.UseVisualStyleBackColor = true;
        this.btnTestDetection.Click += new EventHandler(this.btnTestDetection_Click);
        // 
        // btnToggleMonitoring
        // 
        this.btnToggleMonitoring.Anchor = ((AnchorStyles)((AnchorStyles.Bottom | AnchorStyles.Left)));
        this.btnToggleMonitoring.Location = new Point(240, 160);
        this.btnToggleMonitoring.Name = "btnToggleMonitoring";
        this.btnToggleMonitoring.Size = new Size(120, 30);
        this.btnToggleMonitoring.TabIndex = 5;
        this.btnToggleMonitoring.Text = "Auto-Detect: ON";
        this.btnToggleMonitoring.UseVisualStyleBackColor = true;
        this.btnToggleMonitoring.Click += new EventHandler(this.btnToggleMonitoring_Click);
        // 
        // btnConnectDevice
        // 
        this.btnConnectDevice.Anchor = ((AnchorStyles)((AnchorStyles.Bottom | AnchorStyles.Left)));
        this.btnConnectDevice.Enabled = false;
        this.btnConnectDevice.Location = new Point(370, 160);
        this.btnConnectDevice.Name = "btnConnectDevice";
        this.btnConnectDevice.Size = new Size(100, 30);
        this.btnConnectDevice.TabIndex = 2;
        this.btnConnectDevice.Text = "Connect Device";
        this.btnConnectDevice.UseVisualStyleBackColor = true;
        this.btnConnectDevice.Click += new EventHandler(this.btnConnectDevice_Click);
        // 
        // lstDevices
        // 
        this.lstDevices.Anchor = ((AnchorStyles)((((AnchorStyles.Top | AnchorStyles.Bottom) | AnchorStyles.Left) | AnchorStyles.Right)));
        this.lstDevices.FormattingEnabled = true;
        this.lstDevices.ItemHeight = 15;
        this.lstDevices.Location = new Point(12, 50);
        this.lstDevices.Name = "lstDevices";
        this.lstDevices.Size = new Size(776, 94);
        this.lstDevices.TabIndex = 1;
        this.lstDevices.SelectedIndexChanged += new EventHandler(this.lstDevices_SelectedIndexChanged);
        // 
        // lblDeviceStatus
        // 
        this.lblDeviceStatus.Anchor = ((AnchorStyles)((AnchorStyles.Top | AnchorStyles.Left)));
        this.lblDeviceStatus.AutoSize = true;
        this.lblDeviceStatus.Location = new Point(12, 25);
        this.lblDeviceStatus.Name = "lblDeviceStatus";
        this.lblDeviceStatus.Size = new Size(78, 15);
        this.lblDeviceStatus.TabIndex = 0;
        this.lblDeviceStatus.Text = "No devices found";
        // 
        // groupBox2
        // 
        this.groupBox2.Controls.Add(this.btnStopRecovery);
        this.groupBox2.Controls.Add(this.btnStartRecovery);
        this.groupBox2.Controls.Add(this.btnCopyAll);
        this.groupBox2.Controls.Add(this.btnCopySelected);
        this.groupBox2.Controls.Add(this.btnClearList);
        this.groupBox2.Controls.Add(this.chkIncludeVideos);
        this.groupBox2.Controls.Add(this.chkIncludeImages);
        this.groupBox2.Controls.Add(this.txtRecoveryPath);
        this.groupBox2.Controls.Add(this.btnBrowsePath);
        this.groupBox2.Controls.Add(this.lblRecoveryPath);
        this.groupBox2.Dock = DockStyle.Top;
        this.groupBox2.Location = new Point(0, 200);
        this.groupBox2.Name = "groupBox2";
        this.groupBox2.Size = new Size(800, 190);
        this.groupBox2.TabIndex = 1;
        this.groupBox2.TabStop = false;
        this.groupBox2.Text = "Recovery Settings";
        // 
        // btnStopRecovery
        // 
        this.btnStopRecovery.Enabled = false;
        this.btnStopRecovery.Location = new Point(130, 110);
        this.btnStopRecovery.Name = "btnStopRecovery";
        this.btnStopRecovery.Size = new Size(100, 30);
        this.btnStopRecovery.TabIndex = 6;
        this.btnStopRecovery.Text = "Stop Recovery";
        this.btnStopRecovery.UseVisualStyleBackColor = true;
        this.btnStopRecovery.Click += new EventHandler(this.btnStopRecovery_Click);
        // 
        // btnStartRecovery
        // 
        this.btnStartRecovery.Enabled = false;
        this.btnStartRecovery.Location = new Point(12, 110);
        this.btnStartRecovery.Name = "btnStartRecovery";
        this.btnStartRecovery.Size = new Size(100, 30);
        this.btnStartRecovery.TabIndex = 5;
        this.btnStartRecovery.Text = "Start Scan";
        this.btnStartRecovery.UseVisualStyleBackColor = true;
        this.btnStartRecovery.Click += new EventHandler(this.btnStartRecovery_Click);
        // 
        // btnCopyAll
        // 
        this.btnCopyAll.Enabled = false;
        this.btnCopyAll.Location = new Point(12, 150);
        this.btnCopyAll.Name = "btnCopyAll";
        this.btnCopyAll.Size = new Size(100, 30);
        this.btnCopyAll.TabIndex = 7;
        this.btnCopyAll.Text = "Copy All";
        this.btnCopyAll.UseVisualStyleBackColor = true;
        this.btnCopyAll.Click += new EventHandler(this.btnCopyAll_Click);
        // 
        // btnCopySelected
        // 
        this.btnCopySelected.Enabled = false;
        this.btnCopySelected.Location = new Point(118, 150);
        this.btnCopySelected.Name = "btnCopySelected";
        this.btnCopySelected.Size = new Size(100, 30);
        this.btnCopySelected.TabIndex = 8;
        this.btnCopySelected.Text = "Copy Selected";
        this.btnCopySelected.UseVisualStyleBackColor = true;
        this.btnCopySelected.Click += new EventHandler(this.btnCopySelected_Click);
        // 
        // btnClearList
        // 
        this.btnClearList.Enabled = false;
        this.btnClearList.Location = new Point(224, 150);
        this.btnClearList.Name = "btnClearList";
        this.btnClearList.Size = new Size(100, 30);
        this.btnClearList.TabIndex = 9;
        this.btnClearList.Text = "Clear List";
        this.btnClearList.UseVisualStyleBackColor = true;
        this.btnClearList.Click += new EventHandler(this.btnClearList_Click);
        // 
        // chkIncludeVideos
        // 
        this.chkIncludeVideos.AutoSize = true;
        this.chkIncludeVideos.Checked = true;
        this.chkIncludeVideos.CheckState = CheckState.Checked;
        this.chkIncludeVideos.Location = new Point(150, 80);
        this.chkIncludeVideos.Name = "chkIncludeVideos";
        this.chkIncludeVideos.Size = new Size(100, 19);
        this.chkIncludeVideos.TabIndex = 4;
        this.chkIncludeVideos.Text = "Include Videos";
        this.chkIncludeVideos.UseVisualStyleBackColor = true;
        // 
        // chkIncludeImages
        // 
        this.chkIncludeImages.AutoSize = true;
        this.chkIncludeImages.Checked = true;
        this.chkIncludeImages.CheckState = CheckState.Checked;
        this.chkIncludeImages.Location = new Point(12, 80);
        this.chkIncludeImages.Name = "chkIncludeImages";
        this.chkIncludeImages.Size = new Size(100, 19);
        this.chkIncludeImages.TabIndex = 3;
        this.chkIncludeImages.Text = "Include Images";
        this.chkIncludeImages.UseVisualStyleBackColor = true;
        // 
        // txtRecoveryPath
        // 
        this.txtRecoveryPath.Location = new Point(12, 50);
        this.txtRecoveryPath.Name = "txtRecoveryPath";
        this.txtRecoveryPath.ReadOnly = true;
        this.txtRecoveryPath.Size = new Size(650, 23);
        this.txtRecoveryPath.TabIndex = 2;
        this.txtRecoveryPath.Text = "Current scanning path will appear here during recovery...";
        // 
        // btnBrowsePath
        // 
        this.btnBrowsePath.Location = new Point(680, 50);
        this.btnBrowsePath.Name = "btnBrowsePath";
        this.btnBrowsePath.Size = new Size(75, 23);
        this.btnBrowsePath.TabIndex = 1;
        this.btnBrowsePath.Text = "Browse...";
        this.btnBrowsePath.UseVisualStyleBackColor = true;
        this.btnBrowsePath.Click += new EventHandler(this.btnBrowsePath_Click);
        // 
        // lblRecoveryPath
        // 
        this.lblRecoveryPath.AutoSize = true;
        this.lblRecoveryPath.Location = new Point(12, 25);
        this.lblRecoveryPath.Name = "lblRecoveryPath";
        this.lblRecoveryPath.Size = new Size(100, 15);
        this.lblRecoveryPath.TabIndex = 0;
        this.lblRecoveryPath.Text = "Current Scanning Path:";
        // 
        // groupBox3
        // 
        this.groupBox3.Controls.Add(this.lstRecoveredFiles);
        this.groupBox3.Dock = DockStyle.Fill;
        this.groupBox3.Location = new Point(0, 390);
        this.groupBox3.Name = "groupBox3";
        this.groupBox3.Size = new Size(800, 314);
        this.groupBox3.TabIndex = 2;
        this.groupBox3.TabStop = false;
        this.groupBox3.Text = "Recovered Files";
        // 
        // lstRecoveredFiles
        // 
        this.lstRecoveredFiles.Columns.AddRange(new ColumnHeader[] {
            this.colFileName,
            this.colFileSize,
            this.colFileType,
            this.colRecoveryDate,
            this.colStatus});
        this.lstRecoveredFiles.ContextMenuStrip = this.contextMenuStrip1;
        this.lstRecoveredFiles.Dock = DockStyle.Fill;
        this.lstRecoveredFiles.FullRowSelect = true;
        this.lstRecoveredFiles.GridLines = true;
        this.lstRecoveredFiles.Location = new Point(3, 19);
        this.lstRecoveredFiles.Name = "lstRecoveredFiles";
        this.lstRecoveredFiles.Size = new Size(794, 332);
        this.lstRecoveredFiles.TabIndex = 0;
        this.lstRecoveredFiles.UseCompatibleStateImageBehavior = false;
        this.lstRecoveredFiles.View = View.Details;
        this.lstRecoveredFiles.SelectedIndexChanged += new EventHandler(this.lstRecoveredFiles_SelectedIndexChanged);
        // 
        // colFileName
        // 
        this.colFileName.Text = "File Name";
        this.colFileName.Width = 200;
        // 
        // colFileSize
        // 
        this.colFileSize.Text = "Size";
        this.colFileSize.Width = 100;
        // 
        // colFileType
        // 
        this.colFileType.Text = "Type";
        this.colFileType.Width = 100;
        // 
        // colRecoveryDate
        // 
        this.colRecoveryDate.Text = "Recovery Date";
        this.colRecoveryDate.Width = 150;
        // 
        // colStatus
        // 
        this.colStatus.Text = "Status";
        this.colStatus.Width = 100;
        // 
        // contextMenuStrip1
        // 
        this.contextMenuStrip1.Items.AddRange(new ToolStripItem[] {
            this.previewToolStripMenuItem,
            this.recoverToolStripMenuItem,
            this.toolStripSeparator1,
            this.deleteToolStripMenuItem});
        this.contextMenuStrip1.Name = "contextMenuStrip1";
        this.contextMenuStrip1.Size = new Size(153, 76);
        // 
        // previewToolStripMenuItem
        // 
        this.previewToolStripMenuItem.Name = "previewToolStripMenuItem";
        this.previewToolStripMenuItem.Size = new Size(152, 22);
        this.previewToolStripMenuItem.Text = "Preview";
        this.previewToolStripMenuItem.Click += new EventHandler(this.previewToolStripMenuItem_Click);
        // 
        // recoverToolStripMenuItem
        // 
        this.recoverToolStripMenuItem.Name = "recoverToolStripMenuItem";
        this.recoverToolStripMenuItem.Size = new Size(152, 22);
        this.recoverToolStripMenuItem.Text = "Recover File";
        this.recoverToolStripMenuItem.Click += new EventHandler(this.recoverToolStripMenuItem_Click);
        // 
        // toolStripSeparator1
        // 
        this.toolStripSeparator1.Name = "toolStripSeparator1";
        this.toolStripSeparator1.Size = new Size(149, 6);
        // 
        // deleteToolStripMenuItem
        // 
        this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
        this.deleteToolStripMenuItem.Size = new Size(152, 22);
        this.deleteToolStripMenuItem.Text = "Remove from List";
        this.deleteToolStripMenuItem.Click += new EventHandler(this.deleteToolStripMenuItem_Click);
        // 
        // pictureBox1
        // 
        this.pictureBox1.Anchor = ((AnchorStyles)((((AnchorStyles.Top | AnchorStyles.Bottom) | AnchorStyles.Left) | AnchorStyles.Right)));
        this.pictureBox1.BackColor = Color.LightGray;
        this.pictureBox1.BorderStyle = BorderStyle.FixedSingle;
        this.pictureBox1.Location = new Point(0, 0);
        this.pictureBox1.Name = "pictureBox1";
        this.pictureBox1.Size = new Size(396, 550);
        this.pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
        this.pictureBox1.TabIndex = 0;
        this.pictureBox1.TabStop = false;
        // 
        // lblPreview
        // 
        this.lblPreview.Anchor = ((AnchorStyles)((AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right)));
        this.lblPreview.BackColor = Color.LightBlue;
        this.lblPreview.BorderStyle = BorderStyle.FixedSingle;
        this.lblPreview.Font = new Font("Microsoft Sans Serif", 8F, FontStyle.Regular);
        this.lblPreview.ForeColor = Color.Black;
        this.lblPreview.Location = new Point(0, 550);
        this.lblPreview.Name = "lblPreview";
        this.lblPreview.Size = new Size(396, 154);
        this.lblPreview.TabIndex = 1;
        this.lblPreview.Text = "Scanning Information:\r\n\r\nReady to scan device storage...\r\n\r\nSelect a file to preview";
        this.lblPreview.TextAlign = ContentAlignment.TopLeft;
        // 
        // timer1
        // 
        this.timer1.Interval = 1000;
        this.timer1.Tick += new EventHandler(this.timer1_Tick);
        // 
        // Form1
        // 
        this.AutoScaleDimensions = new SizeF(7F, 15F);
        this.AutoScaleMode = AutoScaleMode.Font;
        this.ClientSize = new Size(1200, 750);
        this.Controls.Add(this.splitContainer1);
        this.Controls.Add(this.statusStrip1);
        this.Controls.Add(this.menuStrip1);
        this.MainMenuStrip = this.menuStrip1;
        this.MinimumSize = new Size(1000, 600);
        this.Name = "Form1";
        this.StartPosition = FormStartPosition.CenterScreen;
        this.Text = "Android File Recovery Tool v1.0";
        this.Load += new EventHandler(this.Form1_Load);
        this.menuStrip1.ResumeLayout(false);
        this.menuStrip1.PerformLayout();
        this.statusStrip1.ResumeLayout(false);
        this.statusStrip1.PerformLayout();
        this.splitContainer1.Panel1.ResumeLayout(false);
        this.splitContainer1.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
        this.splitContainer1.ResumeLayout(false);
        this.groupBox1.ResumeLayout(false);
        this.groupBox1.PerformLayout();
        this.groupBox2.ResumeLayout(false);
        this.groupBox2.PerformLayout();
        this.groupBox3.ResumeLayout(false);
        this.contextMenuStrip1.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    #endregion

    private MenuStrip menuStrip1;
    private ToolStripMenuItem fileToolStripMenuItem;
    private ToolStripMenuItem exitToolStripMenuItem;
    private ToolStripMenuItem toolsToolStripMenuItem;
    private ToolStripMenuItem viewStatisticsToolStripMenuItem;
    private ToolStripMenuItem viewLogToolStripMenuItem;
    private ToolStripMenuItem helpToolStripMenuItem;
    private ToolStripMenuItem aboutToolStripMenuItem;
    private StatusStrip statusStrip1;
    private ToolStripStatusLabel statusLabel;
    private ToolStripProgressBar progressBar;
    private SplitContainer splitContainer1;
    private GroupBox groupBox1;
    private Button btnRefreshDevices;
    private Button btnTestDetection;
    private Button btnToggleMonitoring;
    private Button btnConnectDevice;
    private ListBox lstDevices;
    private Label lblDeviceStatus;
    private GroupBox groupBox2;
    private Button btnStopRecovery;
    private Button btnStartRecovery;
    private Button btnCopyAll;
    private Button btnCopySelected;
    private Button btnClearList;
    private CheckBox chkIncludeVideos;
    private CheckBox chkIncludeImages;
    private TextBox txtRecoveryPath;
    private Button btnBrowsePath;
    private Label lblRecoveryPath;
    private GroupBox groupBox3;
    private ListView lstRecoveredFiles;
    private ColumnHeader colFileName;
    private ColumnHeader colFileSize;
    private ColumnHeader colFileType;
    private ColumnHeader colRecoveryDate;
    private ColumnHeader colStatus;
    private ContextMenuStrip contextMenuStrip1;
    private ToolStripMenuItem previewToolStripMenuItem;
    private ToolStripMenuItem recoverToolStripMenuItem;
    private ToolStripSeparator toolStripSeparator1;
    private ToolStripMenuItem deleteToolStripMenuItem;
    private PictureBox pictureBox1;
    private Label lblPreview;
    private System.Windows.Forms.Timer timer1;
}
