namespace Client
{
    partial class Menu
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Menu));
            chatFormBtn = new Button();
            mailFormBtn = new Button();
            webFormBtn = new Button();
            goldFormBtn = new Button();
            pingBtn = new Button();
            hostFormBtn = new Button();
            userLabel = new Label();
            verLabel = new Label();
            SuspendLayout();
            // 
            // chatFormBtn
            // 
            chatFormBtn.Location = new Point(19, 56);
            chatFormBtn.Margin = new Padding(4);
            chatFormBtn.Name = "chatFormBtn";
            chatFormBtn.Size = new Size(195, 97);
            chatFormBtn.TabIndex = 0;
            chatFormBtn.Text = "Chat";
            chatFormBtn.UseVisualStyleBackColor = true;
            chatFormBtn.Click += chatFormBtn_Click;
            // 
            // mailFormBtn
            // 
            mailFormBtn.Location = new Point(223, 56);
            mailFormBtn.Margin = new Padding(4);
            mailFormBtn.Name = "mailFormBtn";
            mailFormBtn.Size = new Size(195, 97);
            mailFormBtn.TabIndex = 1;
            mailFormBtn.Text = "Mail";
            mailFormBtn.UseVisualStyleBackColor = true;
            mailFormBtn.Click += mailFormBtn_Click;
            // 
            // webFormBtn
            // 
            webFormBtn.Location = new Point(223, 161);
            webFormBtn.Margin = new Padding(4);
            webFormBtn.Name = "webFormBtn";
            webFormBtn.Size = new Size(195, 97);
            webFormBtn.TabIndex = 3;
            webFormBtn.Text = "WebBrowser";
            webFormBtn.UseVisualStyleBackColor = true;
            webFormBtn.Click += webFormBtn_Click;
            // 
            // goldFormBtn
            // 
            goldFormBtn.Location = new Point(19, 161);
            goldFormBtn.Margin = new Padding(4);
            goldFormBtn.Name = "goldFormBtn";
            goldFormBtn.Size = new Size(195, 97);
            goldFormBtn.TabIndex = 2;
            goldFormBtn.Text = "Giá Vàng";
            goldFormBtn.UseVisualStyleBackColor = true;
            goldFormBtn.Click += goldFormBtn_Click;
            // 
            // pingBtn
            // 
            pingBtn.Location = new Point(223, 267);
            pingBtn.Margin = new Padding(4);
            pingBtn.Name = "pingBtn";
            pingBtn.Size = new Size(195, 97);
            pingBtn.TabIndex = 5;
            pingBtn.Text = "Ping";
            pingBtn.UseVisualStyleBackColor = true;
            pingBtn.Click += pingBtn_Click;
            // 
            // hostFormBtn
            // 
            hostFormBtn.Location = new Point(19, 267);
            hostFormBtn.Margin = new Padding(4);
            hostFormBtn.Name = "hostFormBtn";
            hostFormBtn.Size = new Size(195, 97);
            hostFormBtn.TabIndex = 4;
            hostFormBtn.Text = "WebHosting";
            hostFormBtn.UseVisualStyleBackColor = true;
            hostFormBtn.Click += hostFormBtn_Click;
            // 
            // userLabel
            // 
            userLabel.AutoSize = true;
            userLabel.Font = new Font("Microsoft Sans Serif", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            userLabel.Location = new Point(19, 9);
            userLabel.Name = "userLabel";
            userLabel.Size = new Size(118, 24);
            userLabel.TabIndex = 6;
            userLabel.Text = "anonymous";
            // 
            // verLabel
            // 
            verLabel.AutoSize = true;
            verLabel.Font = new Font("Microsoft Sans Serif", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            verLabel.Location = new Point(313, 385);
            verLabel.Name = "verLabel";
            verLabel.RightToLeft = RightToLeft.Yes;
            verLabel.Size = new Size(100, 16);
            verLabel.TabIndex = 7;
            verLabel.Text = "Version 1.0.0.10";
            verLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Menu
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(431, 412);
            Controls.Add(verLabel);
            Controls.Add(userLabel);
            Controls.Add(pingBtn);
            Controls.Add(hostFormBtn);
            Controls.Add(webFormBtn);
            Controls.Add(goldFormBtn);
            Controls.Add(mailFormBtn);
            Controls.Add(chatFormBtn);
            Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "Menu";
            Text = "Menu";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button chatFormBtn;
        private Button mailFormBtn;
        private Button webFormBtn;
        private Button goldFormBtn;
        private Button pingBtn;
        private Button hostFormBtn;
        private Label verLabel;
        private Label userLabel;
    }
}