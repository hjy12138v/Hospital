using His_Server.Model.EntityMap;
using SqlSugar;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace His_Server.DAL.Repositories
{
    /// <summary>
    /// 用户仓储实现：使用 SqlSugar 操作 User 表。
    /// 通过依赖注入获取 SqlSugarClient，避免手动管理连接。
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly SqlSugarClient _db;

        public UserRepository(SqlSugarClient db)
        {
            _db = db;
        }

        /// <summary>
        /// 查询所有用户
        /// </summary>
        public async Task<List<User>> GetAllAsync()
        {
            return await _db.Queryable<User>().ToListAsync();
        }

        /// <summary>
        /// 按主键查询用户
        /// </summary>
        public async Task<User?> GetByIdAsync(int id)
        {
            return await _db.Queryable<User>().InSingleAsync(id);
        }

        /// <summary>
        /// 新增用户，返回新记录的主键Id
        /// </summary>
        public async Task<int> AddAsync(User user)
        {
            var newId = await _db.Insertable(user).ExecuteReturnIdentityAsync();
            return newId;
        }

        /// <summary>
        /// 更新用户，返回是否成功
        /// </summary>
        public async Task<bool> UpdateAsync(User user)
        {
            var rows = await _db.Updateable(user).ExecuteCommandAsync();
            return rows > 0;
        }

        /// <summary>
        /// 删除用户，返回是否成功
        /// </summary>
        public async Task<bool> DeleteAsync(int id)
        {
            var rows = await _db.Deleteable<User>().In(id).ExecuteCommandAsync();
            return rows > 0;
        }
    }
}