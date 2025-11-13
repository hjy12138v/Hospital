using His_Server.Model.EntityMap;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace His_Server.DAL.Repositories
{
    /// <summary>
    /// 医生仓储接口：Doctor 表基础 CRUD。
    /// </summary>
    public interface IDoctorRepository
    {
        Task<List<Doctor>> GetAllAsync();
        Task<Doctor?> GetByIdAsync(int id);
        Task<int> AddAsync(Doctor doctor);
        Task<bool> UpdateAsync(Doctor doctor);
        Task<bool> DeleteAsync(int id);
    }
}