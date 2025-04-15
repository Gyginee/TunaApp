using System;
using System.Windows.Forms;
using Client.Models;
using Client.Utils;  // Đảm bảo rằng lớp InformationManager và AppState đã được định nghĩa

namespace Client
{
    public partial class MailSend : Form
    {
        public MailSend()
        {
            InitializeComponent();
        }

        // Sự kiện gửi mail khi nhấn nút "Gửi"
        // Sự kiện nhấn nút Gửi mail
        private async void btnSend_Click(object sender, EventArgs e)
        {
            string recipient = txtRecipient.Text.Trim();
            string title = txtTitle.Text.Trim();
            string cc = txtCC.Text.Trim();
            string bcc = txtBCC.Text.Trim();
            string body = txtBody.Text.Trim();

            // Kiểm tra thông tin bắt buộc: người nhận, tiêu đề và nội dung mail
            if (string.IsNullOrEmpty(recipient) || string.IsNullOrEmpty(title) || string.IsNullOrEmpty(body))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin bắt buộc: Người nhận, Tiêu đề và Nội dung.",
                                  "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Cho phép CC và BCC để trống; nếu trống thì chuyển thành chuỗi rỗng
            if (string.IsNullOrWhiteSpace(cc)) cc = "";
            if (string.IsNullOrWhiteSpace(bcc)) bcc = "";

            // Tạo lệnh gửi mail theo định dạng: SEND_MAIL|recipient|title|cc|bcc|body
            string command = $"SEND_MAIL|{recipient}|{title}|{cc}|{bcc}|{body}";

            try
            {
                // Gửi lệnh mail đến server sử dụng UtilityManager gửi lệnh một lần
                // Câu lệnh command có định dạng: SEND_MAIL|recipient|title|cc|bcc|body
                string response = await UtilityManager.SendSingleCommandAsync(AppState.CurrentUser, command);

                if (response.StartsWith("SEND_MAIL_SUCCESS"))
                {
                    MessageBox.Show("Mail đã được gửi thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Gửi mail thất bại: " + response, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi gửi mail: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Sự kiện nhấn nút "Hủy"
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        
    }
}
