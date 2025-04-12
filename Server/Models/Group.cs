using System;

namespace Server.Models
{
    public class Group
    {
        public int Id { get; set; }
        public string GroupName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
