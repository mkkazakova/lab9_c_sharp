namespace lab9_2
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private ListBox listBox1;

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
            listBox1 = new ListBox();
            button1 = new Button();
            label1 = new Label();
            SuspendLayout();
            // 
            // listBox1
            // 
            listBox1.BackColor = Color.BurlyWood;
            listBox1.FormattingEnabled = true;
            listBox1.ItemHeight = 25;
            listBox1.Location = new Point(68, 25);
            listBox1.Margin = new Padding(4, 5, 4, 5);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(348, 554);
            listBox1.TabIndex = 1;
            // 
            // button1
            // 
            button1.BackColor = Color.Tan;
            button1.Location = new Point(451, 484);
            button1.Margin = new Padding(4, 5, 4, 5);
            button1.Name = "button1";
            button1.Size = new Size(218, 95);
            button1.TabIndex = 0;
            button1.Text = "Пуск";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(513, 108);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(42, 25);
            label1.TabIndex = 2;
            label1.Text = "Text";
            // 
            // Form1
            // 
            BackColor = Color.AntiqueWhite;
            ClientSize = new Size(903, 619);
            Controls.Add(label1);
            Controls.Add(button1);
            Controls.Add(listBox1);
            Name = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private Label label1;
    }
}