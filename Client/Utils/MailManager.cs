using Client.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Client.Utils
{
    public static class MailManager
    {
        public static async Task<List<EmailInfo>> GetMailsAsync(string currentUser)
        {
            var emails = new List<EmailInfo>();

            if (!await InformationManager.InitializeAsync(currentUser))
                throw new Exception("Không thể kết nối tới server.");

            await InformationManager.SendCommandAsync("GET_MAILS");
            string response = await InformationManager.ReadResponseAsync();

            if (!response.StartsWith("MAIL|"))
                throw new Exception("Server trả về dữ liệu không hợp lệ: " + response);

            var lines = response.Split(new[] { "|||" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var parts = line.Split('|', 6); // Chỉ tách thành 6 phần: MAIL, Sender, Subject, CC, Date, Body
                if (parts.Length == 6)
                {
                    emails.Add(new EmailInfo
                    {
                        Sender = parts[1],
                        Subject = parts[2],
                        Cc = parts[3],
                        ReceivedDate = DateTime.TryParse(parts[4], out var dt) ? dt : DateTime.Now,
                        Body = DecodeBody(parts[5]),
                        IsRead = false
                    });
                }
            }

            return emails;
        }

        private static string DecodeBody(string base64)
        {
            try
            {
                var bytes = Convert.FromBase64String(base64);
                return Encoding.UTF8.GetString(bytes);
            }
            catch
            {
                return base64; // fallback nếu không phải base64 hợp lệ
            }
        }
    }
}
