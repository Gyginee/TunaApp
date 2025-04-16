using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Imap;
using MailKit.Search;
using MailKit.Security;
using MimeKit;
using Server;

namespace Server.Handlers
{
    public static class MailHandlers
    {
        public static async Task HandleSendMail(string sender, string recipient, string title, string cc, string bcc, string body)
        {
            try
            {
                MailMessage mail = new()
                {
                    From = new MailAddress(ServerConfig.GmailAddress),
                    Subject = title,
                    Body = $"From: {sender}\n\n{body}",
                    IsBodyHtml = false
                };

                mail.To.Add(recipient);

                if (!string.IsNullOrWhiteSpace(cc))
                {
                    foreach (var address in cc.Split(',', StringSplitOptions.RemoveEmptyEntries))
                        mail.CC.Add(address.Trim());
                }

                if (!string.IsNullOrWhiteSpace(bcc))
                {
                    foreach (var address in bcc.Split(',', StringSplitOptions.RemoveEmptyEntries))
                        mail.Bcc.Add(address.Trim());
                }

                using SmtpClient smtp = new(ServerConfig.GmailSmtpServer, ServerConfig.GmailSmtpPort);
                smtp.Credentials = new NetworkCredential(ServerConfig.GmailAddress, ServerConfig.GmailAppPassword);
                smtp.EnableSsl = true;
                await smtp.SendMailAsync(mail);

                Console.WriteLine($"[MAIL] Gửi từ {sender} đến {recipient} thành công.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MAIL ERROR] {ex.Message}");
            }
        }

        public static async Task HandleGetMails(StreamWriter writer, string username)
        {
            try
            {
                string email = ServerConfig.GmailAddress;
                string appPassword = ServerConfig.GmailAppPassword;

                using var client = new ImapClient();
                await client.ConnectAsync("imap.gmail.com", 993, SecureSocketOptions.SslOnConnect);
                await client.AuthenticateAsync(email, appPassword);

                var inbox = client.Inbox;
                await inbox.OpenAsync(MailKit.FolderAccess.ReadOnly);

                var uids = inbox.Search(SearchQuery.All);
                var sb = new StringBuilder();

                int count = 0;
                for (int i = uids.Count - 1; i >= 0 && count < 10; i--, count++)
                {
                    var msg = inbox.GetMessage(uids[i]);

                    string from = msg.From.ToString().Replace("\r", "").Replace("\n", "").Trim();
                    string subject = msg.Subject?.Trim() ?? "(Không tiêu đề)";
                    string cc = string.Join(",", msg.Cc.Select(cc => cc.ToString()));
                    string date = msg.Date.LocalDateTime.ToString();
                    string body = Convert.ToBase64String(Encoding.UTF8.GetBytes(msg.TextBody ?? ""));

                    sb.Append($"MAIL|{from}|{subject}|{cc}|{date}|{body}|||");
                }

                await writer.WriteLineAsync(sb.ToString().TrimEnd('|'));
                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                await writer.WriteLineAsync("ERROR|GET_MAILS|" + ex.Message);
            }
        }
    }
}
