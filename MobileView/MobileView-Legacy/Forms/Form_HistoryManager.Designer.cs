
namespace MobileView
{
    partial class Form_HistoryManager
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_HistoryManager));
            TopBarPanel = new Panel();
            MenuButton = new Button();
            BackButton = new Button();
            ReloadButton = new Button();
            URLTextBox = new TextBox();
            dataGridView1 = new DataGridView();
            MenuPanel = new Panel();
            MenuStrip = new MenuStrip();
            clearAllBrowsingHistoryToolStripMenuItem = new ToolStripMenuItem();
            deleteByDateRangeToolStripMenuItem = new ToolStripMenuItem();
            TopBarPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            MenuPanel.SuspendLayout();
            MenuStrip.SuspendLayout();
            SuspendLayout();
            // 
            // TopBarPanel
            // 
            TopBarPanel.BackColor = Color.FromArgb(33, 33, 33);
            TopBarPanel.Controls.Add(MenuButton);
            TopBarPanel.Controls.Add(BackButton);
            TopBarPanel.Controls.Add(ReloadButton);
            TopBarPanel.Controls.Add(URLTextBox);
            TopBarPanel.Dock = DockStyle.Top;
            TopBarPanel.Location = new Point(0, 24);
            TopBarPanel.Name = "TopBarPanel";
            TopBarPanel.Size = new Size(312, 32);
            TopBarPanel.TabIndex = 2;
            // 
            // MenuButton
            // 
            MenuButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            MenuButton.BackgroundImage = Properties.Resources.menu_25dp_FFFFFF;
            MenuButton.BackgroundImageLayout = ImageLayout.Stretch;
            MenuButton.FlatAppearance.BorderColor = Color.FromArgb(33, 33, 33);
            MenuButton.FlatStyle = FlatStyle.Flat;
            MenuButton.Location = new Point(282, 3);
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
            URLTextBox.Size = new Size(211, 23);
            URLTextBox.TabIndex = 0;
            // 
            // dataGridView1
            // 
            dataGridViewCellStyle1.ForeColor = Color.Black;
            dataGridViewCellStyle1.SelectionForeColor = Color.White;
            dataGridView1.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Dock = DockStyle.Fill;
            dataGridView1.Location = new Point(0, 56);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.RowHeadersVisible = false;
            dataGridViewCellStyle2.ForeColor = Color.Black;
            dataGridViewCellStyle2.SelectionForeColor = Color.White;
            dataGridView1.RowsDefaultCellStyle = dataGridViewCellStyle2;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.Size = new Size(312, 508);
            dataGridView1.TabIndex = 3;
            dataGridView1.CellDoubleClick += dataGridView1_CellDoubleClick;
            // 
            // MenuPanel
            // 
            MenuPanel.AutoSize = true;
            MenuPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            MenuPanel.Controls.Add(MenuStrip);
            MenuPanel.Dock = DockStyle.Top;
            MenuPanel.Location = new Point(0, 0);
            MenuPanel.Name = "MenuPanel";
            MenuPanel.Size = new Size(312, 24);
            MenuPanel.TabIndex = 4;
            MenuPanel.Visible = false;
            // 
            // MenuStrip
            // 
            MenuStrip.BackColor = Color.Transparent;
            MenuStrip.Items.AddRange(new ToolStripItem[] { clearAllBrowsingHistoryToolStripMenuItem, deleteByDateRangeToolStripMenuItem });
            MenuStrip.Location = new Point(0, 0);
            MenuStrip.Name = "MenuStrip";
            MenuStrip.Size = new Size(312, 24);
            MenuStrip.TabIndex = 0;
            // 
            // clearAllBrowsingHistoryToolStripMenuItem
            // 
            clearAllBrowsingHistoryToolStripMenuItem.ForeColor = Color.White;
            clearAllBrowsingHistoryToolStripMenuItem.Name = "clearAllBrowsingHistoryToolStripMenuItem";
            clearAllBrowsingHistoryToolStripMenuItem.Size = new Size(156, 20);
            clearAllBrowsingHistoryToolStripMenuItem.Text = "&Clear All Browsing History";
            clearAllBrowsingHistoryToolStripMenuItem.Click += clearAllBrowsingHistoryToolStripMenuItem_Click;
            // 
            // deleteByDateRangeToolStripMenuItem
            // 
            deleteByDateRangeToolStripMenuItem.ForeColor = Color.White;
            deleteByDateRangeToolStripMenuItem.Name = "deleteByDateRangeToolStripMenuItem";
            deleteByDateRangeToolStripMenuItem.Size = new Size(131, 20);
            deleteByDateRangeToolStripMenuItem.Text = "&Delete by Date Range";
            // 
            // Form_HistoryManager
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(33, 33, 33);
            ClientSize = new Size(312, 564);
            Controls.Add(dataGridView1);
            Controls.Add(TopBarPanel);
            Controls.Add(MenuPanel);
            ForeColor = SystemColors.HighlightText;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form_HistoryManager";
            Shown += Form_HistoryManager_Shown;
            TopBarPanel.ResumeLayout(false);
            TopBarPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            MenuPanel.ResumeLayout(false);
            MenuPanel.PerformLayout();
            MenuStrip.ResumeLayout(false);
            MenuStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }


        #endregion

        private Panel TopBarPanel;
        private Button MenuButton;
        private Button BackButton;
        private Button ReloadButton;
        private DataGridView dataGridView1;
        private TextBox URLTextBox;
        private Panel MenuPanel;
        private MenuStrip MenuStrip;
        private ToolStripMenuItem clearAllBrowsingHistoryToolStripMenuItem;
        private ToolStripMenuItem deleteByDateRangeToolStripMenuItem;
    }
}