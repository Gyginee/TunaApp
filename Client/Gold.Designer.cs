namespace Client
{
    partial class Gold
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
            goldData = new DataGridView();
            refreshButton = new Button();
            statusStrip = new StatusStrip();
            statusLabel = new ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)goldData).BeginInit();
            statusStrip.SuspendLayout();
            SuspendLayout();
            // 
            // goldData
            // 
            goldData.BackgroundColor = SystemColors.Control;
            goldData.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            goldData.Location = new Point(4, 51);
            goldData.Name = "goldData";
            goldData.Size = new Size(447, 227);
            goldData.TabIndex = 0;
            // 
            // refreshButton
            // 
            refreshButton.Location = new Point(355, 9);
            refreshButton.Name = "refreshButton";
            refreshButton.Size = new Size(96, 33);
            refreshButton.TabIndex = 0;
            refreshButton.Text = "Làm mới";
            refreshButton.UseVisualStyleBackColor = true;
            // 
            // statusStrip
            // 
            statusStrip.Items.AddRange(new ToolStripItem[] { statusLabel });
            statusStrip.Location = new Point(0, 297);
            statusStrip.Name = "statusStrip";
            statusStrip.Size = new Size(455, 22);
            statusStrip.TabIndex = 1;
            statusStrip.Text = "statusStrip1";
            // 
            // statusLabel
            // 
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(118, 17);
            statusLabel.Text = "toolStripStatusLabel1";
            // 
            // Gold
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(455, 319);
            Controls.Add(statusStrip);
            Controls.Add(refreshButton);
            Controls.Add(goldData);
            Name = "Gold";
            Text = "Gold";
            ((System.ComponentModel.ISupportInitialize)goldData).EndInit();
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView goldData;
        private Button refreshButton;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel statusLabel;
    }
}