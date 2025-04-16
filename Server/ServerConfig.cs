using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class ServerConfig
    {
        public const string ServerIP = "192.168.1.248";
        public const int ServerPort = 12345;

        // Cấu hình Gmail SMTP
        public const string GmailAddress = "example@gmail.com";
        public const string GmailAppPassword = "Password APP"; // Sử dụng mật khẩu ứng dụng nếu bật 2FA
        public const string GmailSmtpServer = "smtp.gmail.com";
        public const int GmailSmtpPort = 587;
    }
}
