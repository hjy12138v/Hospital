using AutoMapper;
using His_Server.Model.EntityDto;
using His_Server.Model.EntityMap;

namespace His_Server.BLL.Mapping
{
    // 统一维护 Entity ↔ Dto 的映射关系
    public class EntityProfiles : Profile
    {
        public EntityProfiles()
        {
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<Doctor, DoctorDto>().ReverseMap();
            CreateMap<Department, DepartmentDto>().ReverseMap();
            CreateMap<Notice, NoticeDto>().ReverseMap();
            CreateMap<Medicine, MedicineDto>().ReverseMap();
        }
    }
}