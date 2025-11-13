using His_Server.Model.EntityMap;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace His_Server.DAL.Repositories
{
    /// <summary>
    /// 通知仓储接口：Notice 表基础 CRUD。
    /// </summary>
    public interface INoticeRepository
    {
        Task<List<Notice>> GetAllAsync();
        Task<Notice?> GetByIdAsync(int id);
        Task<int> AddAsync(Notice notice);
        Task<bool> UpdateAsync(Notice notice);
        Task<bool> DeleteAsync(int id);
    }
}