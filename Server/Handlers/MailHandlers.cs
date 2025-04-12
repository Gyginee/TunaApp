using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Server; // Để sử dụng ServerConfig

namespace Server.Handlers
{
    public static class MailHandlers
    {
        // Phương thức gửi mail sử dụng Gmail SMTP với Title, CC và BCC.
        // sender: tên người gửi (được lấy từ thông tin đăng nhập của client)
        // recipient: địa chỉ email nhận chính
        // title: tiêu đề mail
        // cc: danh sách các địa chỉ gửi CC, phân cách bằng dấu phẩy (nếu có)
        // bcc: danh sách các địa chỉ gửi BCC, phân cách bằng dấu phẩy (nếu có)
        // body: nội dung của mail
        public static async Task HandleSendMail(string sender, string recipient, string title, string cc, string bcc, string body)
        {
            try
            {
                MailMessage mail = new MailMessage();

                // Sử dụng địa chỉ Gmail cấu hình từ ServerConfig
                mail.From = new MailAddress(ServerConfig.GmailAddress);
                mail.To.Add(recipient);

                // Thêm CC nếu có
                if (!string.IsNullOrWhiteSpace(cc))
                {
                    string[] ccAddresses = cc.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var address in ccAddresses)
                    {
                        mail.CC.Add(address.Trim());
                    }
                }

                // Thêm BCC nếu có
                if (!string.IsNullOrWhiteSpace(bcc))
                {
                    string[] bccAddresses = bcc.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var address in bccAddresses)
                    {
                        mail.Bcc.Add(address.Trim());
                    }
                }

                mail.Subject = title;
                mail.Body = $"From: {sender}\n\n{body}";
                mail.IsBodyHtml = false; // Nếu không dùng HTML

                // Cấu hình SmtpClient dựa trên ServerConfig
                using (SmtpClient smtp = new SmtpClient(ServerConfig.GmailSmtpServer, ServerConfig.GmailSmtpPort))
                {
                    smtp.Credentials = new NetworkCredential(ServerConfig.GmailAddress, ServerConfig.GmailAppPassword);
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(mail);
                }

                Console.WriteLine($"[MAIL] Mail gửi từ {sender} đến {recipient} thành công.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MAIL ERROR] Lỗi gửi mail: {ex.Message}");
                // Có thể gửi phản hồi lỗi về client nếu cần.
            }
        }
    }
}
