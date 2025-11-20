namespace His_Server.Model.EntityDto
{
    public class DoctorDto
    {
        public int DoctorId { get; set; }
        public int UserId { get; set; }
        public int DepartmentId { get; set; }
        public string ? Position { get; set; }
        public string ? Specialty { get; set; }
        // 可根据需要添加科室名称、医生姓名等关联信息
    }
}
