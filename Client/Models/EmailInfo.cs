using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models
{
    // Thêm class này để biểu diễn một file đính kèm
    public class AttachmentInfo
    {
        public string FileName { get; set; }
        public byte[] Content { get; set; } // Nội dung file dưới dạng byte array
        public string ContentType { get; set; } // Ví dụ: "image/jpeg", "application/pdf" (Tùy chọn)
    }

    public class EmailInfo
    {
        public string Sender { get; set; }
        public string To { get; set; } // Thêm trường To
        public string Subject { get; set; }
        public string Cc { get; set; }
        public DateTime ReceivedDate { get; set; }
        public string Body { get; set; } // Nội dung email (có thể là text hoặc HTML)
        public bool IsRead { get; set; }
        public List<AttachmentInfo> Attachments { get; set; } = new List<AttachmentInfo>(); // Danh sách file đính kèm
        public bool IsHtmlBody { get; set; } // Cho biết Body là HTML hay Plain Text (Quan trọng)
    }

}
