using His_Server.Model.EntityDto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace His_Server.BLL.Services
{
    /// <summary>
    /// 药品业务接口：封装 Medicine 的业务逻辑与 DTO 映射。
    /// </summary>
    public interface IMedicineService
    {
        Task<List<MedicineDto>> GetAllAsync();
        Task<MedicineDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(MedicineDto dto);
        Task<bool> UpdateAsync(int id, MedicineDto dto);
        Task<bool> DeleteAsync(int id);
    }
}