using His_Server.Model.EntityMap;
using SqlSugar;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace His_Server.DAL.Repositories
{
    /// <summary>
    /// 科室仓储实现：使用 SqlSugar 操作 Department 表。
    /// </summary>
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly SqlSugarClient _db;

        public DepartmentRepository(SqlSugarClient db)
        {
            _db = db;
        }

        public async Task<List<Department>> GetAllAsync()
        {
            return await _db.Queryable<Department>().ToListAsync();
        }

        public async Task<Department?> GetByIdAsync(int id)
        {
            return await _db.Queryable<Department>().InSingleAsync(id);
        }

        public async Task<int> AddAsync(Department department)
        {
            return await _db.Insertable(department).ExecuteReturnIdentityAsync();
        }

        public async Task<bool> UpdateAsync(Department department)
        {
            var rows = await _db.Updateable(department).ExecuteCommandAsync();
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var rows = await _db.Deleteable<Department>().In(id).ExecuteCommandAsync();
            return rows > 0;
        }
    }
}