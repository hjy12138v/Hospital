using His_Server.Model.EntityDto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace His_Server.BLL.Services
{
    /// <summary>
    /// 用户业务接口：封装业务逻辑与DTO映射。
    /// </summary>
    public interface IUserService
    {
        Task<List<UserDto>> GetAllAsync();
        Task<UserDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(UserDto dto, string password);
        Task<bool> UpdateAsync(int id, UserDto dto);
        Task<bool> DeleteAsync(int id);
        /// <summary>
        /// 登录校验：用户名与密码正确返回用户信息，否则返回 null。
        /// </summary>
        Task<UserDto?> UserLoginAsync(string name, string password);
    }
}