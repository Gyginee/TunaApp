using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
namespace Client.Models
{
    // Lớp MessageEntry để lưu trữ mọi loại tin nhắn
    public class MessageEntry
    {
        public string Sender { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsImage { get; set; }
        public bool IsFile { get; set; }
        public string FileName { get; set; }
        public byte[] FileData { get; set; }
        public string GroupName { get; set; } // Nếu là tin nhắn nhóm

        public string GetFormattedTime()
        {
            return Timestamp.ToString("HH:mm:ss");
        }

        // Tạo key duy nhất cho tin nhắn để tránh hiển thị trùng lặp
        public string GetUniqueKey()
        {
            string type = IsImage ? "IMAGE" : (IsFile ? "FILE" : "TEXT");
            string contentHash = string.Empty;

            if (IsImage || IsFile)
            {
                using (SHA256 sha = SHA256.Create())
                {
                    contentHash = Convert.ToBase64String(sha.ComputeHash(FileData)).Substring(0, 10);
                }
            }
            else
            {
                contentHash = Content;
            }

            return $"{type}|{Sender}|{Timestamp.Ticks}|{contentHash}";
        }
    }
}
