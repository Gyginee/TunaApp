namespace Client.ChildForm
{
    partial class PingUserDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.ListBox listBoxUsers;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
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
            listBoxUsers = new ListBox();
            btnOK = new Button();
            btnCancel = new Button();
            statusStrip = new StatusStrip();
            statusLabel = new ToolStripStatusLabel();
            statusStrip.SuspendLayout();
            SuspendLayout();
            // 
            // listBoxUsers
            // 
            listBoxUsers.FormattingEnabled = true;
            listBoxUsers.ItemHeight = 15;
            listBoxUsers.Location = new Point(12, 12);
            listBoxUsers.Name = "listBoxUsers";
            listBoxUsers.Size = new Size(260, 154);
            listBoxUsers.TabIndex = 0;
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(197, 180);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(75, 25);
            btnCancel.TabIndex = 2;
            btnCancel.Text = "Hủy";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // statusStrip
            // 
            statusStrip.Items.AddRange(new ToolStripItem[] { statusLabel });
            statusStrip.Location = new Point(0, 219);
            statusStrip.Name = "statusStrip";
            statusStrip.Size = new Size(284, 22);
            statusStrip.TabIndex = 3;
            statusStrip.Text = "";
            // 
            // statusLabel
            // 
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(118, 17);
            statusLabel.Text = "Chọn người dùng để ping...";
            // 
            // PingUserDialog
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(284, 241);
            Controls.Add(statusStrip);
            Controls.Add(btnCancel);
            Controls.Add(listBoxUsers);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Name = "PingUserDialog";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Chọn User để Ping";
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private StatusStrip statusStrip;
        private ToolStripStatusLabel statusLabel;
    }
}