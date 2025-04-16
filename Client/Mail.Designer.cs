namespace Client
{
    partial class Mail
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
            // Khởi tạo các thành phần giao diện
            this.components = new System.ComponentModel.Container(); // Cần thiết cho ToolStrip
            this.listViewEmails = new System.Windows.Forms.ListView();
            this.colSender = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSubject = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colCc = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colDate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsbCompose = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbRefresh = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout(); // Bắt đầu cấu hình ToolStrip
            this.SuspendLayout(); // Bắt đầu cấu hình Form
            //
            // toolStrip1
            //
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden; // Ẩn grip để trông phẳng hơn
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20); // Kích thước icon (nếu có)
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbCompose,
            this.toolStripSeparator1,
            this.tsbRefresh});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(5, 0, 1, 0); // Thêm padding cho đẹp
            this.toolStrip1.Size = new System.Drawing.Size(800, 27); // Chiều cao ToolStrip
            this.toolStrip1.TabIndex = 2; // Đặt TabIndex
            this.toolStrip1.Text = "toolStrip1";
            //
            // tsbCompose
            //
            this.tsbCompose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text; // Chỉ hiển thị Text
            //this.tsbCompose.Image = ((System.Drawing.Image)(resources.GetObject("tsbCompose.Image"))); // Thêm icon nếu có
            //this.tsbCompose.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbCompose.Name = "tsbCompose";
            this.tsbCompose.Size = new System.Drawing.Size(78, 24);
            this.tsbCompose.Text = "Soạn thư"; // Compose New Mail
            this.tsbCompose.ToolTipText = "Soạn thư mới";
            this.tsbCompose.Click += new System.EventHandler(this.tsbCompose_Click); // Đổi tên event handler
            //
            // toolStripSeparator1
            //
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            //
            // tsbRefresh
            //
            this.tsbRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text; // Chỉ hiển thị Text
            //this.tsbRefresh.Image = ((System.Drawing.Image)(resources.GetObject("tsbRefresh.Image"))); // Thêm icon nếu có
            //this.tsbRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbRefresh.Name = "tsbRefresh";
            this.tsbRefresh.Size = new System.Drawing.Size(71, 24);
            this.tsbRefresh.Text = "Làm mới"; // Refresh
            this.tsbRefresh.ToolTipText = "Tải lại danh sách thư";
            this.tsbRefresh.Click += new System.EventHandler(this.tsbRefresh_Click); // Đổi tên event handler
            //
            // listViewEmails
            //
            this.listViewEmails.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colSender,
            this.colSubject,
            this.colCc,
            this.colDate});
            this.listViewEmails.Dock = System.Windows.Forms.DockStyle.Fill; // Chiếm toàn bộ không gian còn lại
            this.listViewEmails.FullRowSelect = true;
            this.listViewEmails.GridLines = true;
            this.listViewEmails.HideSelection = false;
            // Vị trí được đặt dưới ToolStrip do Dock = Fill
            this.listViewEmails.Location = new System.Drawing.Point(0, 27); // Vị trí Y bắt đầu sau ToolStrip
            this.listViewEmails.MultiSelect = false;
            this.listViewEmails.Name = "listViewEmails";
            // Kích thước sẽ tự điều chỉnh theo Dock = Fill
            this.listViewEmails.Size = new System.Drawing.Size(800, 423); // ClientSize.Height - toolStrip1.Height
            this.listViewEmails.TabIndex = 1; // Đặt TabIndex sau ToolStrip
            this.listViewEmails.UseCompatibleStateImageBehavior = false;
            this.listViewEmails.View = System.Windows.Forms.View.Details;
            this.listViewEmails.DoubleClick += new System.EventHandler(this.listViewEmails_DoubleClick);
            //
            // colSender
            //
            this.colSender.Text = "Người gửi"; // Sender
            this.colSender.Width = 150;
            //
            // colSubject
            //
            this.colSubject.Text = "Tiêu đề"; // Subject
            this.colSubject.Width = 250;
            //
            // colCc
            //
            this.colCc.Text = "CC";
            this.colCc.Width = 200;
            //
            // colDate
            //
            this.colDate.Text = "Ngày nhận"; // Received Date
            this.colDate.Width = 150;
            //
            // Mail
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            // Quan trọng: Add ListView trước ToolStrip để ToolStrip hiển thị đúng ở trên cùng khi Dock
            this.Controls.Add(this.listViewEmails);
            this.Controls.Add(this.toolStrip1);
            this.Name = "Mail";
            this.Text = "Hộp thư đến"; // Mail Inbox - Có thể đổi thành tên phù hợp hơn
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen; // Canh giữa màn hình khi mở
            this.Load += new System.EventHandler(this.Mail_Load);
            this.toolStrip1.ResumeLayout(false); // Kết thúc cấu hình ToolStrip
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false); // Kết thúc cấu hình Form
            this.PerformLayout(); // Áp dụng layout của ToolStrip

        }

        #endregion

        // Đổi tên các control và thêm ToolStrip
        private System.Windows.Forms.ListView listViewEmails;
        private System.Windows.Forms.ColumnHeader colSender;
        private System.Windows.Forms.ColumnHeader colSubject;
        private System.Windows.Forms.ColumnHeader colCc;
        private System.Windows.Forms.ColumnHeader colDate;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tsbCompose; // Thay thế btnCompose
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton tsbRefresh; // Thêm nút Refresh
    }
}