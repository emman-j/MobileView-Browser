namespace MobileView.Forms
{
    partial class Bookmark_Form
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
            TopBarPanel = new Panel();
            BackButton = new Button();
            tableLayoutPanel1 = new TableLayoutPanel();
            TitleTextBox = new TextBox();
            label2 = new Label();
            label3 = new Label();
            label1 = new Label();
            UrlTextBox = new TextBox();
            comboBox1 = new ComboBox();
            AddModifyButton = new Button();
            TopBarPanel.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // TopBarPanel
            // 
            TopBarPanel.BackColor = Color.FromArgb(33, 33, 33);
            TopBarPanel.Controls.Add(BackButton);
            TopBarPanel.Dock = DockStyle.Top;
            TopBarPanel.Location = new Point(0, 0);
            TopBarPanel.Name = "TopBarPanel";
            TopBarPanel.Size = new Size(312, 32);
            TopBarPanel.TabIndex = 4;
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
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.34615F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 83.65385F));
            tableLayoutPanel1.Controls.Add(TitleTextBox, 1, 0);
            tableLayoutPanel1.Controls.Add(label2, 0, 0);
            tableLayoutPanel1.Controls.Add(label3, 0, 2);
            tableLayoutPanel1.Controls.Add(label1, 0, 1);
            tableLayoutPanel1.Controls.Add(UrlTextBox, 1, 1);
            tableLayoutPanel1.Controls.Add(comboBox1, 1, 2);
            tableLayoutPanel1.Dock = DockStyle.Top;
            tableLayoutPanel1.Location = new Point(0, 32);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 3;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel1.Size = new Size(312, 92);
            tableLayoutPanel1.TabIndex = 5;
            // 
            // TitleTextBox
            // 
            TitleTextBox.Dock = DockStyle.Fill;
            TitleTextBox.Location = new Point(53, 3);
            TitleTextBox.Name = "TitleTextBox";
            TitleTextBox.Size = new Size(256, 23);
            TitleTextBox.TabIndex = 7;
            // 
            // label2
            // 
            label2.Dock = DockStyle.Fill;
            label2.Location = new Point(3, 0);
            label2.Name = "label2";
            label2.Size = new Size(44, 30);
            label2.TabIndex = 6;
            label2.Text = "Title";
            label2.TextAlign = ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            label3.Dock = DockStyle.Fill;
            label3.Location = new Point(3, 60);
            label3.Name = "label3";
            label3.Size = new Size(44, 32);
            label3.TabIndex = 5;
            label3.Text = "Folder";
            label3.TextAlign = ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            label1.Dock = DockStyle.Fill;
            label1.Location = new Point(3, 30);
            label1.Name = "label1";
            label1.Size = new Size(44, 30);
            label1.TabIndex = 0;
            label1.Text = "URL";
            label1.TextAlign = ContentAlignment.MiddleRight;
            // 
            // UrlTextBox
            // 
            UrlTextBox.Dock = DockStyle.Fill;
            UrlTextBox.Location = new Point(53, 33);
            UrlTextBox.Name = "UrlTextBox";
            UrlTextBox.Size = new Size(256, 23);
            UrlTextBox.TabIndex = 3;
            // 
            // comboBox1
            // 
            comboBox1.Dock = DockStyle.Fill;
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(53, 63);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(256, 23);
            comboBox1.TabIndex = 4;
            // 
            // AddModifyButton
            // 
            AddModifyButton.Location = new Point(234, 130);
            AddModifyButton.Name = "AddModifyButton";
            AddModifyButton.Size = new Size(75, 23);
            AddModifyButton.TabIndex = 6;
            AddModifyButton.Text = "Add";
            AddModifyButton.UseVisualStyleBackColor = true;
            // 
            // Bookmark_Form
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(312, 180);
            Controls.Add(AddModifyButton);
            Controls.Add(tableLayoutPanel1);
            Controls.Add(TopBarPanel);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            MaximumSize = new Size(328, 219);
            MinimumSize = new Size(328, 219);
            Name = "Bookmark_Form";
            Text = "Bookmark_Form";
            TopBarPanel.ResumeLayout(false);
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel TopBarPanel;
        private Button BackButton;
        private TableLayoutPanel tableLayoutPanel1;
        private Label label1;
        private TextBox UrlTextBox;
        private ComboBox comboBox1;
        private Button AddModifyButton;
        private TextBox TitleTextBox;
        private Label label2;
        private Label label3;
    }
}