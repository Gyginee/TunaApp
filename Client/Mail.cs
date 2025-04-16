using Client.Models;
using Client.Utils;
using Client.ChildForm;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Client
{
    public partial class Mail : Form
    {
        private List<EmailInfo> currentEmails = new();

        public Mail()
        {
            InitializeComponent();
        }

        private async void Mail_Load(object sender, EventArgs e)
        {
            SetupListViewColumns();
            await LoadServerMails();
        }

        private void SetupListViewColumns()
        {
            if (listViewEmails.Columns.Count == 0)
            {
                listViewEmails.Columns.Add("Sender", 150, HorizontalAlignment.Left);
                listViewEmails.Columns.Add("Subject", 250, HorizontalAlignment.Left);
                listViewEmails.Columns.Add("CC", 200, HorizontalAlignment.Left);
                listViewEmails.Columns.Add("Received Date", 150, HorizontalAlignment.Left);
                listViewEmails.View = View.Details;
                listViewEmails.FullRowSelect = true;
                listViewEmails.GridLines = true;
            }
        }

        private async Task LoadServerMails()
        {
            try
            {
                currentEmails = await MailManager.GetMailsAsync(AppState.CurrentUser);
                DisplayEmails(currentEmails);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải mail: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

     
        private void DisplayEmails(List<EmailInfo> emails)
        {
            listViewEmails.Items.Clear();
            if (emails == null) return;

            foreach (var email in emails)
            {
                var item = new ListViewItem(email.Sender);
                item.SubItems.Add(email.Subject);
                item.SubItems.Add(email.Cc ?? "");
                item.SubItems.Add(email.ReceivedDate.ToString("yyyy-MM-dd HH:mm"));
                item.Tag = email;

                item.Font = email.IsRead
                    ? new Font(listViewEmails.Font, FontStyle.Regular)
                    : new Font(listViewEmails.Font, FontStyle.Bold);

                listViewEmails.Items.Add(item);
            }

            listViewEmails.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void listViewEmails_DoubleClick(object sender, EventArgs e)
        {
            if (listViewEmails.SelectedItems.Count != 1) return;

            var selectedItem = listViewEmails.SelectedItems[0];
            if (selectedItem.Tag is EmailInfo selectedEmail)
            {
                // Đánh dấu đã đọc (giữ nguyên logic cũ)
                if (!selectedEmail.IsRead)
                {
                    selectedEmail.IsRead = true;
                    selectedItem.Font = new Font(listViewEmails.Font, FontStyle.Regular);
                    // TODO: Gửi thông báo đã đọc lên server nếu cần
                }

                // --- Thay thế MessageBox bằng Form mới ---
                // Kiểm tra xem form chi tiết đã mở cho email này chưa để tránh mở nhiều lần
                bool isOpen = false;
                foreach (Form openForm in Application.OpenForms)
                {
                    if (openForm is EmailDetailForm detailForm && detailForm.Tag == selectedEmail) // Sử dụng Tag để định danh
                    {
                        detailForm.BringToFront(); // Đưa form đã mở lên trên
                        isOpen = true;
                        break;
                    }
                }

                if (!isOpen)
                {
                    // Tạo và hiển thị form chi tiết
                    var detailForm = new EmailDetailForm(selectedEmail);
                    detailForm.Tag = selectedEmail; // Gán Tag để kiểm tra ở trên
                    detailForm.Show(this); // Hiển thị form (non-modal), this để set Owner
                                           // Hoặc dùng detailForm.ShowDialog(this); nếu muốn modal (khóa form Mail)
                }
                // --- Kết thúc thay thế ---
            }
        }

        private void tsbCompose_Click(object sender, EventArgs e)
        {
            using var mailSendForm = new MailSend();
            mailSendForm.ShowDialog(this);
        }

        private async void tsbRefresh_Click(object sender, EventArgs e)
        {
            // Thêm hiệu ứng chờ nếu muốn (ví dụ: thay đổi con trỏ chuột)
            this.Cursor = Cursors.WaitCursor;
            await LoadServerMails();
            this.Cursor = Cursors.Default;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
                Menu.Instance.Show();
            }
            else
            {
                base.OnFormClosing(e);
            }
        }
    }
}
