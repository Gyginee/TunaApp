namespace Client
{
    partial class Host
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Host));
            stopBtn = new Button();
            startBtn = new Button();
            portNumber = new TextBox();
            label1 = new Label();
            logTextBox = new RichTextBox();
            SuspendLayout();
            // 
            // stopBtn
            // 
            stopBtn.Location = new Point(651, 401);
            stopBtn.Name = "stopBtn";
            stopBtn.Size = new Size(137, 37);
            stopBtn.TabIndex = 0;
            stopBtn.Text = "Dừng lại";
            stopBtn.UseVisualStyleBackColor = true;
         
            // 
            // startBtn
            // 
            startBtn.Location = new Point(508, 401);
            startBtn.Name = "startBtn";
            startBtn.Size = new Size(137, 37);
            startBtn.TabIndex = 1;
            startBtn.Text = "Bắt đầu";
            startBtn.UseVisualStyleBackColor = true;
        
            // 
            // portNumber
            // 
            portNumber.Location = new Point(379, 409);
            portNumber.Name = "portNumber";
            portNumber.PlaceholderText = "00000-65535";
            portNumber.Size = new Size(123, 23);
            portNumber.TabIndex = 2;
            portNumber.Text = "8080";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(344, 414);
            label1.Name = "label1";
            label1.Size = new Size(29, 15);
            label1.TabIndex = 3;
            label1.Text = "Port";
            // 
            // logTextBox
            // 
            logTextBox.Location = new Point(8, 9);
            logTextBox.Name = "logTextBox";
            logTextBox.Size = new Size(783, 388);
            logTextBox.TabIndex = 4;
            logTextBox.Text = "";
            // 
            // Host
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(logTextBox);
            Controls.Add(label1);
            Controls.Add(portNumber);
            Controls.Add(startBtn);
            Controls.Add(stopBtn);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Host";
            Text = "Host";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button stopBtn;
        private Button startBtn;
        private TextBox portNumber;
        private Label label1;
        private RichTextBox logTextBox;
    }
}