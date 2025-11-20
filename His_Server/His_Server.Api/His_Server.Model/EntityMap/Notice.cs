using SqlSugar;

namespace His_Server.Model.EntityMap
{
    [SugarTable("Notice")]
    public class Notice
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int NoticeId { get; set; }

        [SugarColumn(ColumnDataType = "NVARCHAR(255)")]
        public string? Title { get; set; }

        [SugarColumn(ColumnDataType = "TEXT")]
        public string? Content { get; set; }

        [SugarColumn(ColumnDataType = "DATETIME")]
        public DateTime PublishDate { get; set; }

        [SugarColumn(ColumnDataType = "NVARCHAR(50)")]
        public string? Type { get; set; }

        public int SenderId { get; set; }
    }
}
