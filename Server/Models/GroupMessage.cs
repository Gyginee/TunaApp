using System;

namespace Server.Models
{
    public class GroupMessage
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public int SenderId { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
