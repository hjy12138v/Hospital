using SqlSugar;

namespace His_Server.Model.EntityMap
{
    [SugarTable("Department")]
    public class Department
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int DepartmentId { get; set; }

        [SugarColumn(ColumnDataType = "NVARCHAR(255)")]
        public string? Name { get; set; }

        [SugarColumn(ColumnDataType = "TEXT")]
        public string? Description { get; set; }

        [SugarColumn(ColumnDataType = "NVARCHAR(255)")]
        public string? Location { get; set; }
    }
}
