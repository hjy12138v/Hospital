using His_Server.Model.EntityMap;
using SqlSugar;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace His_Server.DAL.Repositories
{
    /// <summary>
    /// 医生仓储实现：使用 SqlSugar 操作 Doctor 表。
    /// </summary>
    public class DoctorRepository : IDoctorRepository
    {
        private readonly SqlSugarClient _db;

        public DoctorRepository(SqlSugarClient db)
        {
            _db = db;
        }

        public async Task<List<Doctor>> GetAllAsync()
        {
            return await _db.Queryable<Doctor>().ToListAsync();
        }

        public async Task<Doctor?> GetByIdAsync(int id)
        {
            return await _db.Queryable<Doctor>().InSingleAsync(id);
        }

        public async Task<int> AddAsync(Doctor doctor)
        {
            return await _db.Insertable(doctor).ExecuteReturnIdentityAsync();
        }

        public async Task<bool> UpdateAsync(Doctor doctor)
        {
            var rows = await _db.Updateable(doctor).ExecuteCommandAsync();
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var rows = await _db.Deleteable<Doctor>().In(id).ExecuteCommandAsync();
            return rows > 0;
        }
    }
}