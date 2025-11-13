using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace His_Server.Model.EntityDto
{
    public class NoticeDto
    {
        public int NoticeId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime PublishDate { get; set; }
        public string Type { get; set; }
        public int SenderId { get; set; }
        // 可根据需要添加发布者姓名等关联信息
    }
}
