namespace Client
{
    partial class Login
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
            //this.Load += new System.EventHandler(this.Login_Load);

            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Login));
            pictureBox1 = new PictureBox();
            userBox = new TextBox();
            passBox = new TextBox();
            pictureBox2 = new PictureBox();
            pictureBox3 = new PictureBox();
            loginBox = new Button();
            signBox = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.InitialImage = (Image)resources.GetObject("pictureBox1.InitialImage");
            pictureBox1.Location = new Point(12, 12);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(266, 232);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // userBox
            // 
            userBox.BorderStyle = BorderStyle.FixedSingle;
            userBox.Font = new Font("UTM Avo", 12F);
            userBox.Location = new Point(51, 287);
            userBox.Margin = new Padding(6);
            userBox.Name = "userBox";
            userBox.PlaceholderText = " Username";
            userBox.Size = new Size(227, 29);
            userBox.TabIndex = 1;
            // 
            // passBox
            // 
            passBox.BorderStyle = BorderStyle.FixedSingle;
            passBox.Font = new Font("UTM Avo", 12F);
            passBox.Location = new Point(51, 339);
            passBox.Margin = new Padding(6);
            passBox.Name = "passBox";
            passBox.PasswordChar = '*';
            passBox.PlaceholderText = " Password";
            passBox.Size = new Size(227, 29);
            passBox.TabIndex = 2;
            // 
            // pictureBox2
            // 
            pictureBox2.Image = (Image)resources.GetObject("pictureBox2.Image");
            pictureBox2.Location = new Point(12, 287);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(33, 30);
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox2.TabIndex = 3;
            pictureBox2.TabStop = false;
            // 
            // pictureBox3
            // 
            pictureBox3.Image = (Image)resources.GetObject("pictureBox3.Image");
            pictureBox3.Location = new Point(12, 339);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(33, 29);
            pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox3.TabIndex = 4;
            pictureBox3.TabStop = false;
            // 
            // loginBox
            // 
            loginBox.Font = new Font("UTM Avo", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            loginBox.Location = new Point(161, 405);
            loginBox.Name = "loginBox";
            loginBox.Size = new Size(117, 33);
            loginBox.TabIndex = 5;
            loginBox.Text = "Đăng nhập";
            loginBox.UseVisualStyleBackColor = true;
            loginBox.Click += loginBox_Click;
            // 
            // signBox
            // 
            signBox.Font = new Font("UTM Avo", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            signBox.Location = new Point(13, 405);
            signBox.Name = "signBox";
            signBox.Size = new Size(117, 33);
            signBox.TabIndex = 6;
            signBox.Text = "Đăng ký";
            signBox.UseVisualStyleBackColor = true;
            signBox.Click += signBox_Click;
            // 
            // Login
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(290, 450);
            Controls.Add(signBox);
            Controls.Add(loginBox);
            Controls.Add(pictureBox3);
            Controls.Add(pictureBox2);
            Controls.Add(passBox);
            Controls.Add(userBox);
            Controls.Add(pictureBox1);
            Font = new Font("UTM Avo", 12F);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MdiChildrenMinimizedAnchorBottom = false;
            Name = "Login";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Login";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox1;
        private TextBox userBox;
        private TextBox passBox;
        private PictureBox pictureBox2;
        private PictureBox pictureBox3;
        private Button loginBox;
        private Button signBox;
    }
}