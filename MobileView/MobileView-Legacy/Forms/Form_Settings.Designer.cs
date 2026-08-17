namespace MobileView.Forms
{
    partial class Form_Settings
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
            button1 = new Button();
            textBox1 = new TextBox();
            label1 = new Label();
            panel1 = new Panel();
            panel2 = new Panel();
            listBox1 = new ListBox();
            panel4 = new Panel();
            textBox3 = new TextBox();
            button4 = new Button();
            button3 = new Button();
            panel3 = new Panel();
            textBox2 = new TextBox();
            button2 = new Button();
            label3 = new Label();
            panel5 = new Panel();
            textBox4 = new TextBox();
            TopBarPanel.SuspendLayout();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            panel4.SuspendLayout();
            panel3.SuspendLayout();
            panel5.SuspendLayout();
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
            TopBarPanel.TabIndex = 3;
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
            // button1
            // 
            button1.Dock = DockStyle.Right;
            button1.Location = new Point(205, 5);
            button1.Name = "button1";
            button1.Size = new Size(107, 24);
            button1.TabIndex = 4;
            button1.Text = "Save Settings";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // textBox1
            // 
            textBox1.Dock = DockStyle.Fill;
            textBox1.Location = new Point(5, 20);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(302, 75);
            textBox1.TabIndex = 5;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Dock = DockStyle.Top;
            label1.Location = new Point(5, 5);
            label1.Name = "label1";
            label1.Size = new Size(62, 15);
            label1.TabIndex = 6;
            label1.Text = "UserAgent";
            // 
            // panel1
            // 
            panel1.Controls.Add(textBox1);
            panel1.Controls.Add(label1);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 32);
            panel1.Name = "panel1";
            panel1.Padding = new Padding(5);
            panel1.Size = new Size(312, 100);
            panel1.TabIndex = 8;
            // 
            // panel2
            // 
            panel2.Controls.Add(listBox1);
            panel2.Controls.Add(panel4);
            panel2.Controls.Add(panel3);
            panel2.Controls.Add(label3);
            panel2.Dock = DockStyle.Top;
            panel2.Location = new Point(0, 132);
            panel2.Name = "panel2";
            panel2.Padding = new Padding(5);
            panel2.Size = new Size(312, 209);
            panel2.TabIndex = 9;
            // 
            // listBox1
            // 
            listBox1.Dock = DockStyle.Fill;
            listBox1.FormattingEnabled = true;
            listBox1.ItemHeight = 15;
            listBox1.Location = new Point(5, 54);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(302, 116);
            listBox1.TabIndex = 9;
            listBox1.SelectedIndexChanged += listBox1_SelectedIndexChanged;
            // 
            // panel4
            // 
            panel4.Controls.Add(textBox3);
            panel4.Controls.Add(button4);
            panel4.Controls.Add(button3);
            panel4.Dock = DockStyle.Bottom;
            panel4.Location = new Point(5, 170);
            panel4.Name = "panel4";
            panel4.Padding = new Padding(0, 5, 0, 5);
            panel4.Size = new Size(302, 34);
            panel4.TabIndex = 10;
            // 
            // textBox3
            // 
            textBox3.Dock = DockStyle.Fill;
            textBox3.Location = new Point(0, 5);
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(163, 23);
            textBox3.TabIndex = 10;
            // 
            // button4
            // 
            button4.Dock = DockStyle.Right;
            button4.Location = new Point(163, 5);
            button4.Margin = new Padding(3, 3, 5, 3);
            button4.Name = "button4";
            button4.Size = new Size(65, 24);
            button4.TabIndex = 9;
            button4.Text = "Update";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // button3
            // 
            button3.Dock = DockStyle.Right;
            button3.Location = new Point(228, 5);
            button3.Margin = new Padding(3, 3, 5, 3);
            button3.Name = "button3";
            button3.Size = new Size(74, 24);
            button3.TabIndex = 8;
            button3.Text = "Remove";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // panel3
            // 
            panel3.Controls.Add(textBox2);
            panel3.Controls.Add(button2);
            panel3.Dock = DockStyle.Top;
            panel3.Location = new Point(5, 20);
            panel3.Name = "panel3";
            panel3.Padding = new Padding(0, 5, 0, 5);
            panel3.Size = new Size(302, 34);
            panel3.TabIndex = 8;
            // 
            // textBox2
            // 
            textBox2.Dock = DockStyle.Fill;
            textBox2.Location = new Point(0, 5);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(252, 23);
            textBox2.TabIndex = 7;
            // 
            // button2
            // 
            button2.Dock = DockStyle.Right;
            button2.Location = new Point(252, 5);
            button2.Name = "button2";
            button2.Size = new Size(50, 24);
            button2.TabIndex = 8;
            button2.Text = "Add";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Dock = DockStyle.Top;
            label3.Location = new Point(5, 5);
            label3.Name = "label3";
            label3.Size = new Size(83, 15);
            label3.TabIndex = 6;
            label3.Text = "Valid Url Suffix";
            // 
            // panel5
            // 
            panel5.Controls.Add(textBox4);
            panel5.Controls.Add(button1);
            panel5.Dock = DockStyle.Bottom;
            panel5.Location = new Point(0, 530);
            panel5.Name = "panel5";
            panel5.Padding = new Padding(0, 5, 0, 5);
            panel5.Size = new Size(312, 34);
            panel5.TabIndex = 10;
            // 
            // textBox4
            // 
            textBox4.Dock = DockStyle.Fill;
            textBox4.Location = new Point(0, 5);
            textBox4.Name = "textBox4";
            textBox4.ReadOnly = true;
            textBox4.Size = new Size(205, 23);
            textBox4.TabIndex = 7;
            // 
            // Form_Settings
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(312, 564);
            Controls.Add(panel5);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Controls.Add(TopBarPanel);
            Name = "Form_Settings";
            Text = "Form_Settings";
            Load += Form_Settings_Load;
            TopBarPanel.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            panel4.ResumeLayout(false);
            panel4.PerformLayout();
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            panel5.ResumeLayout(false);
            panel5.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel TopBarPanel;
        private Button BackButton;
        private Button button1;
        private TextBox textBox1;
        private Label label1;
        private Panel panel1;
        private Panel panel2;
        private Label label3;
        private ListBox listBox1;
        private Panel panel4;
        private TextBox textBox3;
        private Button button4;
        private Button button3;
        private Panel panel3;
        private TextBox textBox2;
        private Button button2;
        private Panel panel5;
        private TextBox textBox4;
    }
}