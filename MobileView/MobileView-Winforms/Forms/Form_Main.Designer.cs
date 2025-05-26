namespace MobileView
{
    partial class Form_Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Main));
            WebView21 = new Microsoft.Web.WebView2.WinForms.WebView2();
            TopBarPanel = new Panel();
            MenuButton = new Button();
            BackButton = new Button();
            ReloadButton = new Button();
            URLTextBox = new TextBox();
            MenuPanel = new Panel();
            MenuStrip = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            incognitoToolStripMenuItem = new ToolStripMenuItem();
            exitToolStripMenuItem = new ToolStripMenuItem();
            BrowserToolStripMenuItem = new ToolStripMenuItem();
            historyToolStripMenuItem1 = new ToolStripMenuItem();
            clearAllBrowsingDataToolStripMenuItem1 = new ToolStripMenuItem();
            extensionsToolStripMenuItem1 = new ToolStripMenuItem();
            viewExtensionsToolStripMenuItem = new ToolStripMenuItem();
            addToolStripMenuItem = new ToolStripMenuItem();
            removeToolStripMenuItem = new ToolStripMenuItem();
            clearAllBrowserDataToolStripMenuItem = new ToolStripMenuItem();
            IncognitoToolStripMenuItem1 = new ToolStripMenuItem();
            TitleBarPanel = new Panel();
            FormTextLabel = new Label();
            pictureBox1 = new PictureBox();
            MinimizeButton = new Button();
            CloseButton = new Button();
            favoritesToolStripMenuItem = new ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)WebView21).BeginInit();
            TopBarPanel.SuspendLayout();
            MenuPanel.SuspendLayout();
            MenuStrip.SuspendLayout();
            TitleBarPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // WebView21
            // 
            WebView21.AllowExternalDrop = true;
            WebView21.CreationProperties = null;
            WebView21.DefaultBackgroundColor = Color.White;
            WebView21.Dock = DockStyle.Fill;
            WebView21.Location = new Point(0, 83);
            WebView21.Name = "WebView21";
            WebView21.Size = new Size(321, 510);
            WebView21.TabIndex = 0;
            WebView21.ZoomFactor = 1D;
            // 
            // TopBarPanel
            // 
            TopBarPanel.BackColor = Color.FromArgb(33, 33, 33);
            TopBarPanel.Controls.Add(MenuButton);
            TopBarPanel.Controls.Add(BackButton);
            TopBarPanel.Controls.Add(ReloadButton);
            TopBarPanel.Controls.Add(URLTextBox);
            TopBarPanel.Dock = DockStyle.Top;
            TopBarPanel.Location = new Point(0, 52);
            TopBarPanel.Name = "TopBarPanel";
            TopBarPanel.Size = new Size(321, 31);
            TopBarPanel.TabIndex = 1;
            // 
            // MenuButton
            // 
            MenuButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            MenuButton.BackgroundImage = Properties.Resources.menu_25dp_FFFFFF;
            MenuButton.BackgroundImageLayout = ImageLayout.Stretch;
            MenuButton.FlatAppearance.BorderColor = Color.FromArgb(33, 33, 33);
            MenuButton.FlatStyle = FlatStyle.Flat;
            MenuButton.Location = new Point(292, 3);
            MenuButton.Name = "MenuButton";
            MenuButton.Size = new Size(25, 26);
            MenuButton.TabIndex = 3;
            MenuButton.UseVisualStyleBackColor = true;
            MenuButton.Click += MenuButton_Click;
            // 
            // BackButton
            // 
            BackButton.BackgroundImage = Properties.Resources.arrow_back_25dp_FFFFFF;
            BackButton.BackgroundImageLayout = ImageLayout.Stretch;
            BackButton.FlatAppearance.BorderColor = Color.FromArgb(33, 33, 33);
            BackButton.FlatStyle = FlatStyle.Flat;
            BackButton.Location = new Point(5, 2);
            BackButton.Name = "BackButton";
            BackButton.Size = new Size(27, 26);
            BackButton.TabIndex = 2;
            BackButton.UseVisualStyleBackColor = true;
            BackButton.Click += BackButton_Click;
            // 
            // ReloadButton
            // 
            ReloadButton.BackgroundImage = Properties.Resources.refresh_35dp;
            ReloadButton.BackgroundImageLayout = ImageLayout.Stretch;
            ReloadButton.FlatAppearance.BorderColor = Color.FromArgb(33, 33, 33);
            ReloadButton.FlatStyle = FlatStyle.Flat;
            ReloadButton.Location = new Point(36, 2);
            ReloadButton.Name = "ReloadButton";
            ReloadButton.Size = new Size(25, 26);
            ReloadButton.TabIndex = 1;
            ReloadButton.UseVisualStyleBackColor = true;
            ReloadButton.Click += ReloadButton_Click;
            // 
            // URLTextBox
            // 
            URLTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            URLTextBox.BackColor = Color.FromArgb(23, 23, 23);
            URLTextBox.BorderStyle = BorderStyle.FixedSingle;
            URLTextBox.ForeColor = Color.White;
            URLTextBox.Location = new Point(65, 4);
            URLTextBox.Name = "URLTextBox";
            URLTextBox.Size = new Size(223, 23);
            URLTextBox.TabIndex = 0;
            URLTextBox.DoubleClick += URLTextBox_DoubleClick;
            URLTextBox.KeyDown += URLTextBox_KeyDown;
            // 
            // MenuPanel
            // 
            MenuPanel.AutoSize = true;
            MenuPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            MenuPanel.Controls.Add(MenuStrip);
            MenuPanel.Dock = DockStyle.Top;
            MenuPanel.Location = new Point(0, 28);
            MenuPanel.Name = "MenuPanel";
            MenuPanel.Size = new Size(321, 24);
            MenuPanel.TabIndex = 2;
            MenuPanel.Visible = false;
            // 
            // MenuStrip
            // 
            MenuStrip.BackColor = Color.FromArgb(33, 33, 33);
            MenuStrip.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, BrowserToolStripMenuItem, IncognitoToolStripMenuItem1 });
            MenuStrip.Location = new Point(0, 0);
            MenuStrip.Name = "MenuStrip";
            MenuStrip.Size = new Size(321, 24);
            MenuStrip.TabIndex = 0;
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { incognitoToolStripMenuItem, exitToolStripMenuItem });
            fileToolStripMenuItem.ForeColor = SystemColors.HighlightText;
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "&File";
            // 
            // incognitoToolStripMenuItem
            // 
            incognitoToolStripMenuItem.Name = "incognitoToolStripMenuItem";
            incognitoToolStripMenuItem.Size = new Size(125, 22);
            incognitoToolStripMenuItem.Text = "&Incognito";
            incognitoToolStripMenuItem.Click += IncognitoToolStripMenuItem_Click;
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(125, 22);
            exitToolStripMenuItem.Text = "&Exit";
            exitToolStripMenuItem.Click += ExitToolStripMenuItem_Click;
            // 
            // BrowserToolStripMenuItem
            // 
            BrowserToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { historyToolStripMenuItem1, favoritesToolStripMenuItem, extensionsToolStripMenuItem1, clearAllBrowserDataToolStripMenuItem });
            BrowserToolStripMenuItem.ForeColor = SystemColors.HighlightText;
            BrowserToolStripMenuItem.Name = "BrowserToolStripMenuItem";
            BrowserToolStripMenuItem.Size = new Size(61, 20);
            BrowserToolStripMenuItem.Text = "&Browser";
            // 
            // historyToolStripMenuItem1
            // 
            historyToolStripMenuItem1.DropDownItems.AddRange(new ToolStripItem[] { clearAllBrowsingDataToolStripMenuItem1 });
            historyToolStripMenuItem1.Name = "historyToolStripMenuItem1";
            historyToolStripMenuItem1.Size = new Size(190, 22);
            historyToolStripMenuItem1.Text = "&History";
            historyToolStripMenuItem1.Click += ViewHistoryToolStripMenuItem_Click;
            // 
            // clearAllBrowsingDataToolStripMenuItem1
            // 
            clearAllBrowsingDataToolStripMenuItem1.Name = "clearAllBrowsingDataToolStripMenuItem1";
            clearAllBrowsingDataToolStripMenuItem1.Size = new Size(197, 22);
            clearAllBrowsingDataToolStripMenuItem1.Text = "&Clear All Browsing Data";
            clearAllBrowsingDataToolStripMenuItem1.Click += ClearAllBrowsingDataToolStripMenuItem1_Click;
            // 
            // extensionsToolStripMenuItem1
            // 
            extensionsToolStripMenuItem1.DropDownItems.AddRange(new ToolStripItem[] { viewExtensionsToolStripMenuItem, addToolStripMenuItem, removeToolStripMenuItem });
            extensionsToolStripMenuItem1.Name = "extensionsToolStripMenuItem1";
            extensionsToolStripMenuItem1.Size = new Size(190, 22);
            extensionsToolStripMenuItem1.Text = "&Extensions";
            // 
            // viewExtensionsToolStripMenuItem
            // 
            viewExtensionsToolStripMenuItem.Name = "viewExtensionsToolStripMenuItem";
            viewExtensionsToolStripMenuItem.Size = new Size(117, 22);
            viewExtensionsToolStripMenuItem.Text = "&View";
            viewExtensionsToolStripMenuItem.Click += ViewExtensionsToolStripMenuItem_Click;
            // 
            // addToolStripMenuItem
            // 
            addToolStripMenuItem.Name = "addToolStripMenuItem";
            addToolStripMenuItem.Size = new Size(117, 22);
            addToolStripMenuItem.Text = "&Add ";
            addToolStripMenuItem.Click += AddExtensionToolStripMenuItem_Click;
            // 
            // removeToolStripMenuItem
            // 
            removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            removeToolStripMenuItem.Size = new Size(117, 22);
            removeToolStripMenuItem.Text = "&Remove";
            removeToolStripMenuItem.Click += RemoveExtensionToolStripMenuItem_Click;
            // 
            // clearAllBrowserDataToolStripMenuItem
            // 
            clearAllBrowserDataToolStripMenuItem.Name = "clearAllBrowserDataToolStripMenuItem";
            clearAllBrowserDataToolStripMenuItem.Size = new Size(190, 22);
            clearAllBrowserDataToolStripMenuItem.Text = "&Clear All Browser Data";
            clearAllBrowserDataToolStripMenuItem.Click += ClearAllBrowserDataToolStripMenuItem_Click;
            // 
            // IncognitoToolStripMenuItem1
            // 
            IncognitoToolStripMenuItem1.ForeColor = SystemColors.HighlightText;
            IncognitoToolStripMenuItem1.Name = "IncognitoToolStripMenuItem1";
            IncognitoToolStripMenuItem1.Size = new Size(70, 20);
            IncognitoToolStripMenuItem1.Text = "&Incognito";
            IncognitoToolStripMenuItem1.Click += IncognitoToolStripMenuItem_Click;
            // 
            // TitleBarPanel
            // 
            TitleBarPanel.BackColor = Color.FromArgb(23, 23, 23);
            TitleBarPanel.Controls.Add(FormTextLabel);
            TitleBarPanel.Controls.Add(pictureBox1);
            TitleBarPanel.Controls.Add(MinimizeButton);
            TitleBarPanel.Controls.Add(CloseButton);
            TitleBarPanel.Dock = DockStyle.Top;
            TitleBarPanel.Location = new Point(0, 0);
            TitleBarPanel.Name = "TitleBarPanel";
            TitleBarPanel.Size = new Size(321, 28);
            TitleBarPanel.TabIndex = 3;
            // 
            // FormTextLabel
            // 
            FormTextLabel.AutoSize = true;
            FormTextLabel.Location = new Point(26, 6);
            FormTextLabel.MaximumSize = new Size(240, 15);
            FormTextLabel.Name = "FormTextLabel";
            FormTextLabel.Size = new Size(58, 15);
            FormTextLabel.TabIndex = 7;
            FormTextLabel.Text = "Page Title";
            // 
            // pictureBox1
            // 
            pictureBox1.BackgroundImage = Properties.Resources.public_30dp_FFFFFF;
            pictureBox1.BackgroundImageLayout = ImageLayout.Stretch;
            pictureBox1.Location = new Point(0, 1);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(25, 25);
            pictureBox1.TabIndex = 6;
            pictureBox1.TabStop = false;
            // 
            // MinimizeButton
            // 
            MinimizeButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            MinimizeButton.BackgroundImage = Properties.Resources.minimize_30dp_FFFFFF;
            MinimizeButton.BackgroundImageLayout = ImageLayout.Zoom;
            MinimizeButton.FlatAppearance.BorderColor = Color.FromArgb(23, 23, 23);
            MinimizeButton.FlatStyle = FlatStyle.Flat;
            MinimizeButton.Location = new Point(263, 1);
            MinimizeButton.Name = "MinimizeButton";
            MinimizeButton.Size = new Size(25, 26);
            MinimizeButton.TabIndex = 5;
            MinimizeButton.UseVisualStyleBackColor = true;
            // 
            // CloseButton
            // 
            CloseButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            CloseButton.BackgroundImage = Properties.Resources.close_30dp_FFFFFF;
            CloseButton.BackgroundImageLayout = ImageLayout.Zoom;
            CloseButton.FlatAppearance.BorderColor = Color.FromArgb(23, 23, 23);
            CloseButton.FlatStyle = FlatStyle.Flat;
            CloseButton.Location = new Point(292, 1);
            CloseButton.Name = "CloseButton";
            CloseButton.Size = new Size(25, 26);
            CloseButton.TabIndex = 4;
            CloseButton.UseVisualStyleBackColor = true;
            // 
            // favoritesToolStripMenuItem
            // 
            favoritesToolStripMenuItem.Name = "favoritesToolStripMenuItem";
            favoritesToolStripMenuItem.Size = new Size(190, 22);
            favoritesToolStripMenuItem.Text = "&Favorites";
            // 
            // Form_Main
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(33, 33, 33);
            ClientSize = new Size(321, 593);
            Controls.Add(WebView21);
            Controls.Add(TopBarPanel);
            Controls.Add(MenuPanel);
            Controls.Add(TitleBarPanel);
            ForeColor = SystemColors.HighlightText;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = MenuStrip;
            Name = "Form_Main";
            StartPosition = FormStartPosition.CenterScreen;
            FormClosing += Form_Main_FormClosing;
            Load += Form_Main_Load;
            ((System.ComponentModel.ISupportInitialize)WebView21).EndInit();
            TopBarPanel.ResumeLayout(false);
            TopBarPanel.PerformLayout();
            MenuPanel.ResumeLayout(false);
            MenuPanel.PerformLayout();
            MenuStrip.ResumeLayout(false);
            MenuStrip.PerformLayout();
            TitleBarPanel.ResumeLayout(false);
            TitleBarPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Microsoft.Web.WebView2.WinForms.WebView2 WebView21;
        private Panel TopBarPanel;
        private TextBox URLTextBox;
        private Button ReloadButton;
        private Button BackButton;
        private Panel MenuPanel;
        private MenuStrip MenuStrip;
        private Button MenuButton;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem BrowserToolStripMenuItem;
        private ToolStripMenuItem incognitoToolStripMenuItem;
        private ToolStripMenuItem extensionsToolStripMenuItem1;
        private ToolStripMenuItem IncognitoToolStripMenuItem1;
        private ToolStripMenuItem clearAllBrowserDataToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem viewExtensionsToolStripMenuItem;
        private ToolStripMenuItem addToolStripMenuItem;
        private ToolStripMenuItem removeToolStripMenuItem;
        private Panel TitleBarPanel;
        private Button CloseButton;
        private Button MinimizeButton;
        private Label FormTextLabel;
        private PictureBox pictureBox1;
        private ToolStripMenuItem historyToolStripMenuItem1;
        private ToolStripMenuItem clearAllBrowsingDataToolStripMenuItem1;
        private ToolStripMenuItem favoritesToolStripMenuItem;
    }
}
