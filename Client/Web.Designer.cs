namespace Client
{
    partial class Web
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Web));
            webPanel = new Panel();
            goBtn = new Button();
            addressBox = new TextBox();
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            SuspendLayout();
            // 
            // webPanel
            // 
            webPanel.Location = new Point(-1, 54);
            webPanel.Margin = new Padding(4);
            webPanel.Name = "webPanel";
            webPanel.Size = new Size(1039, 619);
            webPanel.TabIndex = 0;
            // 
            // goBtn
            // 
            goBtn.Location = new Point(922, 13);
            goBtn.Margin = new Padding(4);
            goBtn.Name = "goBtn";
            goBtn.Size = new Size(96, 28);
            goBtn.TabIndex = 1;
            goBtn.Text = "Go";
            goBtn.UseVisualStyleBackColor = true;
            goBtn.Click += goBtn_Click;
            // 
            // addressBox
            // 
            addressBox.Location = new Point(15, 12);
            addressBox.Margin = new Padding(4);
            addressBox.Name = "addressBox";
            addressBox.PlaceholderText = " Nhập địa chỉ trang web...";
            addressBox.Size = new Size(898, 29);
            addressBox.TabIndex = 2;
            // 
            // Web
            // 
            AutoScaleDimensions = new SizeF(9F, 22F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1034, 666);
            Controls.Add(addressBox);
            Controls.Add(goBtn);
            Controls.Add(webPanel);
            Font = new Font("UTM Avo", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4);
            Name = "Web";
            Text = "Web";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel webPanel;
        private Button goBtn;
        private TextBox addressBox;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
    }
}