namespace Client.ChildForm // Hoặc namespace phù hợp của bạn
{
    partial class EmailDetailForm
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
            pnHeader = new Panel();
            tableLayoutPanelHeader = new TableLayoutPanel();
            lblFromLabel = new Label();
            txtFrom = new TextBox();
            lblToLabel = new Label();
            txtTo = new TextBox();
            lblCcLabel = new Label();
            txtCc = new TextBox();
            lblSubjectLabel = new Label();
            txtSubject = new TextBox();
            lblDateLabel = new Label();
            txtDate = new TextBox();
            pnAttachments = new Panel();
            lstAttachments = new ListBox();
            btnSaveAttachment = new Button();
            lblAttachmentsLabel = new Label();
            pnBody = new Panel();
            rtbBody = new RichTextBox();
            pnHeader.SuspendLayout();
            tableLayoutPanelHeader.SuspendLayout();
            pnAttachments.SuspendLayout();
            pnBody.SuspendLayout();
            SuspendLayout();
            // 
            // pnHeader
            // 
            pnHeader.BorderStyle = BorderStyle.FixedSingle;
            pnHeader.Controls.Add(tableLayoutPanelHeader);
            pnHeader.Dock = DockStyle.Top;
            pnHeader.Location = new Point(0, 0);
            pnHeader.Name = "pnHeader";
            pnHeader.Padding = new Padding(4, 5, 4, 5);
            pnHeader.Size = new Size(684, 127);
            pnHeader.TabIndex = 0;
            // 
            // tableLayoutPanelHeader
            // 
            tableLayoutPanelHeader.ColumnCount = 2;
            tableLayoutPanelHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70F));
            tableLayoutPanelHeader.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanelHeader.Controls.Add(lblFromLabel, 0, 0);
            tableLayoutPanelHeader.Controls.Add(txtFrom, 1, 0);
            tableLayoutPanelHeader.Controls.Add(lblToLabel, 0, 1);
            tableLayoutPanelHeader.Controls.Add(txtTo, 1, 1);
            tableLayoutPanelHeader.Controls.Add(lblCcLabel, 0, 2);
            tableLayoutPanelHeader.Controls.Add(txtCc, 1, 2);
            tableLayoutPanelHeader.Controls.Add(lblSubjectLabel, 0, 3);
            tableLayoutPanelHeader.Controls.Add(txtSubject, 1, 3);
            tableLayoutPanelHeader.Controls.Add(lblDateLabel, 0, 4);
            tableLayoutPanelHeader.Controls.Add(txtDate, 1, 4);
            tableLayoutPanelHeader.Dock = DockStyle.Fill;
            tableLayoutPanelHeader.Location = new Point(4, 5);
            tableLayoutPanelHeader.Name = "tableLayoutPanelHeader";
            tableLayoutPanelHeader.RowCount = 5;
            tableLayoutPanelHeader.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanelHeader.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanelHeader.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanelHeader.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanelHeader.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            tableLayoutPanelHeader.Size = new Size(674, 115);
            tableLayoutPanelHeader.TabIndex = 0;
            // 
            // lblFromLabel
            // 
            lblFromLabel.Anchor = AnchorStyles.Left;
            lblFromLabel.AutoSize = true;
            lblFromLabel.Location = new Point(3, 4);
            lblFromLabel.Name = "lblFromLabel";
            lblFromLabel.Size = new Size(24, 15);
            lblFromLabel.TabIndex = 0;
            lblFromLabel.Text = "Từ:";
            // 
            // txtFrom
            // 
            txtFrom.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtFrom.BackColor = SystemColors.ControlLightLight;
            txtFrom.BorderStyle = BorderStyle.None;
            txtFrom.Location = new Point(73, 3);
            txtFrom.Name = "txtFrom";
            txtFrom.ReadOnly = true;
            txtFrom.Size = new Size(598, 16);
            txtFrom.TabIndex = 1;
            // 
            // lblToLabel
            // 
            lblToLabel.Anchor = AnchorStyles.Left;
            lblToLabel.AutoSize = true;
            lblToLabel.Location = new Point(3, 27);
            lblToLabel.Name = "lblToLabel";
            lblToLabel.Size = new Size(31, 15);
            lblToLabel.TabIndex = 2;
            lblToLabel.Text = "Đến:";
            // 
            // txtTo
            // 
            txtTo.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtTo.BackColor = SystemColors.ControlLightLight;
            txtTo.BorderStyle = BorderStyle.None;
            txtTo.Location = new Point(73, 26);
            txtTo.Name = "txtTo";
            txtTo.ReadOnly = true;
            txtTo.Size = new Size(598, 16);
            txtTo.TabIndex = 3;
            // 
            // lblCcLabel
            // 
            lblCcLabel.Anchor = AnchorStyles.Left;
            lblCcLabel.AutoSize = true;
            lblCcLabel.Location = new Point(3, 50);
            lblCcLabel.Name = "lblCcLabel";
            lblCcLabel.Size = new Size(26, 15);
            lblCcLabel.TabIndex = 4;
            lblCcLabel.Text = "CC:";
            // 
            // txtCc
            // 
            txtCc.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtCc.BackColor = SystemColors.ControlLightLight;
            txtCc.BorderStyle = BorderStyle.None;
            txtCc.Location = new Point(73, 49);
            txtCc.Name = "txtCc";
            txtCc.ReadOnly = true;
            txtCc.Size = new Size(598, 16);
            txtCc.TabIndex = 5;
            // 
            // lblSubjectLabel
            // 
            lblSubjectLabel.Anchor = AnchorStyles.Left;
            lblSubjectLabel.AutoSize = true;
            lblSubjectLabel.Location = new Point(3, 73);
            lblSubjectLabel.Name = "lblSubjectLabel";
            lblSubjectLabel.Size = new Size(48, 15);
            lblSubjectLabel.TabIndex = 6;
            lblSubjectLabel.Text = "Chủ đề:";
            // 
            // txtSubject
            // 
            txtSubject.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtSubject.BackColor = SystemColors.ControlLightLight;
            txtSubject.BorderStyle = BorderStyle.None;
            txtSubject.Font = new Font("Microsoft Sans Serif", 7.8F, FontStyle.Bold, GraphicsUnit.Point, 0);
            txtSubject.Location = new Point(73, 74);
            txtSubject.Name = "txtSubject";
            txtSubject.ReadOnly = true;
            txtSubject.Size = new Size(598, 12);
            txtSubject.TabIndex = 7;
            // 
            // lblDateLabel
            // 
            lblDateLabel.Anchor = AnchorStyles.Left;
            lblDateLabel.AutoSize = true;
            lblDateLabel.Location = new Point(3, 92);
            lblDateLabel.Name = "lblDateLabel";
            lblDateLabel.Size = new Size(38, 23);
            lblDateLabel.TabIndex = 8;
            lblDateLabel.Text = "Ngày nhận:";
            // 
            // txtDate
            // 
            txtDate.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtDate.BackColor = SystemColors.ControlLightLight;
            txtDate.BorderStyle = BorderStyle.None;
            txtDate.Location = new Point(73, 95);
            txtDate.Name = "txtDate";
            txtDate.ReadOnly = true;
            txtDate.Size = new Size(598, 16);
            txtDate.TabIndex = 9;
            // 
            // pnAttachments
            // 
            pnAttachments.BorderStyle = BorderStyle.FixedSingle;
            pnAttachments.Controls.Add(lstAttachments);
            pnAttachments.Controls.Add(btnSaveAttachment);
            pnAttachments.Controls.Add(lblAttachmentsLabel);
            pnAttachments.Dock = DockStyle.Top;
            pnAttachments.Location = new Point(0, 127);
            pnAttachments.Name = "pnAttachments";
            pnAttachments.Padding = new Padding(4, 5, 4, 5);
            pnAttachments.Size = new Size(684, 94);
            pnAttachments.TabIndex = 1;
            pnAttachments.Visible = false;
            // 
            // lstAttachments
            // 
            lstAttachments.Dock = DockStyle.Fill;
            lstAttachments.FormattingEnabled = true;
            lstAttachments.IntegralHeight = false;
            lstAttachments.ItemHeight = 15;
            lstAttachments.Location = new Point(4, 25);
            lstAttachments.Name = "lstAttachments";
            lstAttachments.Size = new Size(674, 32);
            lstAttachments.TabIndex = 1;
            // 
            // btnSaveAttachment
            // 
            btnSaveAttachment.Dock = DockStyle.Bottom;
            btnSaveAttachment.Enabled = false;
            btnSaveAttachment.Location = new Point(4, 57);
            btnSaveAttachment.Name = "btnSaveAttachment";
            btnSaveAttachment.Size = new Size(674, 30);
            btnSaveAttachment.TabIndex = 2;
            btnSaveAttachment.Text = "Lưu file đính kèm đã chọn...";
            btnSaveAttachment.UseVisualStyleBackColor = true;
            // 
            // lblAttachmentsLabel
            // 
            lblAttachmentsLabel.AutoSize = true;
            lblAttachmentsLabel.Dock = DockStyle.Top;
            lblAttachmentsLabel.Location = new Point(4, 5);
            lblAttachmentsLabel.Margin = new Padding(3, 0, 3, 5);
            lblAttachmentsLabel.Name = "lblAttachmentsLabel";
            lblAttachmentsLabel.Padding = new Padding(0, 0, 0, 5);
            lblAttachmentsLabel.Size = new Size(81, 20);
            lblAttachmentsLabel.TabIndex = 0;
            lblAttachmentsLabel.Text = "File đính kèm:";
            // 
            // pnBody
            // 
            pnBody.Controls.Add(rtbBody);
            pnBody.Dock = DockStyle.Fill;
            pnBody.Location = new Point(0, 221);
            pnBody.Name = "pnBody";
            pnBody.Padding = new Padding(7, 8, 7, 8);
            pnBody.Size = new Size(684, 297);
            pnBody.TabIndex = 2;
            // 
            // rtbBody
            // 
            rtbBody.BackColor = SystemColors.Window;
            rtbBody.BorderStyle = BorderStyle.FixedSingle;
            rtbBody.Dock = DockStyle.Fill;
            rtbBody.Location = new Point(7, 8);
            rtbBody.Name = "rtbBody";
            rtbBody.ReadOnly = true;
            rtbBody.Size = new Size(670, 281);
            rtbBody.TabIndex = 0;
            rtbBody.Text = "";
            // 
            // EmailDetailForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(684, 518);
            Controls.Add(pnBody);
            Controls.Add(pnAttachments);
            Controls.Add(pnHeader);
            MinimumSize = new Size(527, 377);
            Name = "EmailDetailForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Chi tiết Email";
            pnHeader.ResumeLayout(false);
            tableLayoutPanelHeader.ResumeLayout(false);
            tableLayoutPanelHeader.PerformLayout();
            pnAttachments.ResumeLayout(false);
            pnAttachments.PerformLayout();
            pnBody.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnHeader;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelHeader;
        private System.Windows.Forms.Label lblFromLabel;
        private System.Windows.Forms.TextBox txtFrom;
        private System.Windows.Forms.Label lblToLabel;
        private System.Windows.Forms.TextBox txtTo;
        private System.Windows.Forms.Label lblCcLabel;
        private System.Windows.Forms.TextBox txtCc;
        private System.Windows.Forms.Label lblSubjectLabel;
        private System.Windows.Forms.TextBox txtSubject;
        private System.Windows.Forms.Label lblDateLabel;
        private System.Windows.Forms.TextBox txtDate;
        private System.Windows.Forms.Panel pnAttachments;
        private System.Windows.Forms.ListBox lstAttachments;
        private System.Windows.Forms.Button btnSaveAttachment;
        private System.Windows.Forms.Label lblAttachmentsLabel;
        private System.Windows.Forms.Panel pnBody;
        private System.Windows.Forms.RichTextBox rtbBody;
    }
}