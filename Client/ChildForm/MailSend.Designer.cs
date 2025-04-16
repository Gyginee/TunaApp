namespace Client
{
    partial class MailSend
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblRecipient;
        private System.Windows.Forms.TextBox txtRecipient;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.TextBox txtTitle;
        private System.Windows.Forms.Label lblCC;
        private System.Windows.Forms.TextBox txtCC;
        private System.Windows.Forms.Label lblBCC;
        private System.Windows.Forms.TextBox txtBCC;
        private System.Windows.Forms.Label lblBody;
        private System.Windows.Forms.TextBox txtBody;
        private System.Windows.Forms.Button btnSend;
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
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Text = "Mail";

            this.lblRecipient = new System.Windows.Forms.Label();
            this.txtRecipient = new System.Windows.Forms.TextBox();
            this.lblTitle = new System.Windows.Forms.Label();
            this.txtTitle = new System.Windows.Forms.TextBox();
            this.lblCC = new System.Windows.Forms.Label();
            this.txtCC = new System.Windows.Forms.TextBox();
            this.lblBCC = new System.Windows.Forms.Label();
            this.txtBCC = new System.Windows.Forms.TextBox();
            this.lblBody = new System.Windows.Forms.Label();
            this.txtBody = new System.Windows.Forms.TextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblRecipient
            // 
            this.lblRecipient.AutoSize = true;
            this.lblRecipient.Location = new System.Drawing.Point(20, 20);
            this.lblRecipient.Name = "lblRecipient";
            this.lblRecipient.Size = new System.Drawing.Size(74, 15);
            this.lblRecipient.TabIndex = 0;
            this.lblRecipient.Text = "Người nhận:";
            // 
            // txtRecipient
            // 
            this.txtRecipient.Location = new System.Drawing.Point(100, 17);
            this.txtRecipient.Name = "txtRecipient";
            this.txtRecipient.Size = new System.Drawing.Size(250, 23);
            this.txtRecipient.TabIndex = 1;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Location = new System.Drawing.Point(20, 55);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(48, 15);
            this.lblTitle.TabIndex = 2;
            this.lblTitle.Text = "Tiêu đề:";
            // 
            // txtTitle
            // 
            this.txtTitle.Location = new System.Drawing.Point(100, 52);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Size = new System.Drawing.Size(250, 23);
            this.txtTitle.TabIndex = 3;
            // 
            // lblCC
            // 
            this.lblCC.AutoSize = true;
            this.lblCC.Location = new System.Drawing.Point(20, 90);
            this.lblCC.Name = "lblCC";
            this.lblCC.Size = new System.Drawing.Size(27, 15);
            this.lblCC.TabIndex = 4;
            this.lblCC.Text = "CC:";
            // 
            // txtCC
            // 
            this.txtCC.Location = new System.Drawing.Point(100, 87);
            this.txtCC.Name = "txtCC";
            this.txtCC.Size = new System.Drawing.Size(250, 23);
            this.txtCC.TabIndex = 5;
            // 
            // lblBCC
            // 
            this.lblBCC.AutoSize = true;
            this.lblBCC.Location = new System.Drawing.Point(20, 125);
            this.lblBCC.Name = "lblBCC";
            this.lblBCC.Size = new System.Drawing.Size(34, 15);
            this.lblBCC.TabIndex = 6;
            this.lblBCC.Text = "BCC:";
            // 
            // txtBCC
            // 
            this.txtBCC.Location = new System.Drawing.Point(100, 122);
            this.txtBCC.Name = "txtBCC";
            this.txtBCC.Size = new System.Drawing.Size(250, 23);
            this.txtBCC.TabIndex = 7;
            // 
            // lblBody
            // 
            this.lblBody.AutoSize = true;
            this.lblBody.Location = new System.Drawing.Point(20, 160);
            this.lblBody.Name = "lblBody";
            this.lblBody.Size = new System.Drawing.Size(62, 15);
            this.lblBody.TabIndex = 8;
            this.lblBody.Text = "Nội dung:";
            // 
            // txtBody
            // 
            this.txtBody.Location = new System.Drawing.Point(100, 157);
            this.txtBody.Multiline = true;
            this.txtBody.Name = "txtBody";
            this.txtBody.Size = new System.Drawing.Size(250, 100);
            this.txtBody.TabIndex = 9;
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(100, 280);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(75, 25);
            this.btnSend.TabIndex = 10;
            this.btnSend.Text = "Gửi";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(200, 280);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 25);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.Text = "Hủy";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // Mail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(380, 330);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.txtBody);
            this.Controls.Add(this.lblBody);
            this.Controls.Add(this.txtBCC);
            this.Controls.Add(this.lblBCC);
            this.Controls.Add(this.txtCC);
            this.Controls.Add(this.lblCC);
            this.Controls.Add(this.txtTitle);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.txtRecipient);
            this.Controls.Add(this.lblRecipient);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "Mail";
            this.Text = "Gửi Mail";
            this.ResumeLayout(false);
            this.PerformLayout();
        
    }

        #endregion
    }
}