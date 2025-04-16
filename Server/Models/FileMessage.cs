using System;

namespace Server.Models
{
    public class FileMessage
    {
        public int Id { get; set; }
        public int SenderId { get; set; }

        // Có thể là UserId hoặc GroupId tùy loại
        public int ReceiverId { get; set; }

        // "private" hoặc "group"
        public string ReceiverType { get; set; }

        public string FileName { get; set; }
        public string FilePath { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
