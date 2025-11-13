using His_Server.Model.EntityMap;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace His_Server.DAL.Repositories
{
    /// <summary>
    /// 科室仓储接口：Department 表基础 CRUD。
    /// </summary>
    public interface IDepartmentRepository
    {
        Task<List<Department>> GetAllAsync();
        Task<Department?> GetByIdAsync(int id);
        Task<int> AddAsync(Department department);
        Task<bool> UpdateAsync(Department department);
        Task<bool> DeleteAsync(int id);
    }
}