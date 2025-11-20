using SqlSugar;

namespace His_Server.Model.EntityMap
{
    [SugarTable("Doctor")]
    public class Doctor
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int DoctorId { get; set; }

        public int UserId { get; set; }

        public int DepartmentId { get; set; }

        [SugarColumn(ColumnDataType = "NVARCHAR(50)")]
        public string? Position { get; set; }

        [SugarColumn(ColumnDataType = "NVARCHAR(255)")]
        public string? Specialty { get; set; }
    }
}
