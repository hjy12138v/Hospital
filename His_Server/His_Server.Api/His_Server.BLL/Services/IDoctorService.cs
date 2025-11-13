using His_Server.Model.EntityDto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace His_Server.BLL.Services
{
    /// <summary>
    /// 医生业务接口：封装 Doctor 的业务逻辑与 DTO 映射。
    /// </summary>
    public interface IDoctorService
    {
        Task<List<DoctorDto>> GetAllAsync();
        Task<DoctorDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(DoctorDto dto);
        Task<bool> UpdateAsync(int id, DoctorDto dto);
        Task<bool> DeleteAsync(int id);
    }
}