using His_Server.Model.EntityMap;
using SqlSugar;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace His_Server.DAL.Repositories
{
    /// <summary>
    /// 通知仓储实现：使用 SqlSugar 操作 Notice 表。
    /// </summary>
    public class NoticeRepository : INoticeRepository
    {
        private readonly SqlSugarClient _db;

        public NoticeRepository(SqlSugarClient db)
        {
            _db = db;
        }

        public async Task<List<Notice>> GetAllAsync()
        {
            return await _db.Queryable<Notice>().ToListAsync();
        }

        public async Task<Notice?> GetByIdAsync(int id)
        {
            return await _db.Queryable<Notice>().InSingleAsync(id);
        }

        public async Task<int> AddAsync(Notice notice)
        {
            return await _db.Insertable(notice).ExecuteReturnIdentityAsync();
        }

        public async Task<bool> UpdateAsync(Notice notice)
        {
            var rows = await _db.Updateable(notice).ExecuteCommandAsync();
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var rows = await _db.Deleteable<Notice>().In(id).ExecuteCommandAsync();
            return rows > 0;
        }
    }
}