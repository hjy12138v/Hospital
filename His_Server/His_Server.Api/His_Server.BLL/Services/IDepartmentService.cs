using His_Server.Model.EntityDto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace His_Server.BLL.Services
{
    /// <summary>
    /// 科室业务接口：封装 Department 的业务逻辑与 DTO 映射。
    /// </summary>
    public interface IDepartmentService
    {
        Task<List<DepartmentDto>> GetAllAsync();
        Task<DepartmentDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(DepartmentDto dto);
        Task<bool> UpdateAsync(int id, DepartmentDto dto);
        Task<bool> DeleteAsync(int id);
    }
}