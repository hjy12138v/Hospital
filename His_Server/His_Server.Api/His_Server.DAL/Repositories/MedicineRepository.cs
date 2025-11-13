using His_Server.Model.EntityMap;
using SqlSugar;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace His_Server.DAL.Repositories
{
    /// <summary>
    /// 药品仓储实现：使用 SqlSugar 操作 Medicine 表。
    /// </summary>
    public class MedicineRepository : IMedicineRepository
    {
        private readonly SqlSugarClient _db;

        public MedicineRepository(SqlSugarClient db)
        {
            _db = db;
        }

        public async Task<List<Medicine>> GetAllAsync()
        {
            return await _db.Queryable<Medicine>().ToListAsync();
        }

        public async Task<Medicine?> GetByIdAsync(int id)
        {
            return await _db.Queryable<Medicine>().InSingleAsync(id);
        }

        public async Task<int> AddAsync(Medicine medicine)
        {
            return await _db.Insertable(medicine).ExecuteReturnIdentityAsync();
        }

        public async Task<bool> UpdateAsync(Medicine medicine)
        {
            var rows = await _db.Updateable(medicine).ExecuteCommandAsync();
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var rows = await _db.Deleteable<Medicine>().In(id).ExecuteCommandAsync();
            return rows > 0;
        }
    }
}