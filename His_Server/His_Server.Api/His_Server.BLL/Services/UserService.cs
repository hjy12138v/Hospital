using His_Server.DAL.Repositories;
using His_Server.Model.EntityDto;
using His_Server.Model.EntityMap;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace His_Server.BLL.Services
{
    /// <summary>
    /// 用户业务实现：处理基本校验与DTO映射。
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;

        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<UserDto>> GetAllAsync()
        {
            var list = await _repository.GetAllAsync();
            return list.Select(MapToDto).ToList();
        }

        public async Task<UserDto?> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity == null ? null : MapToDto(entity);
        }

        public async Task<int> CreateAsync(UserDto dto, string password)
        {
            // 简单校验（可根据需要扩展）
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new System.ArgumentException("用户名不能为空");
            if (string.IsNullOrWhiteSpace(password))
                throw new System.ArgumentException("密码不能为空");

            var entity = new User
            {
                Name = dto.Name,
                Gender = dto.Gender,
                PhoneNumber = dto.PhoneNumber,
                Email = dto.Email,
                RoleId = dto.RoleId,
                DateOfBirth = dto.DateOfBirth,
                Password = password // 生产环境建议加密存储
            };
            var newId = await _repository.AddAsync(entity);
            return newId;
        }

        public async Task<bool> UpdateAsync(int id, UserDto dto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return false;

            existing.Name = dto.Name;
            existing.Gender = dto.Gender;
            existing.PhoneNumber = dto.PhoneNumber;
            existing.Email = dto.Email;
            existing.RoleId = dto.RoleId;
            existing.DateOfBirth = dto.DateOfBirth;
            // 密码更新可单独接口，此处不改密码

            return await _repository.UpdateAsync(existing);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        private static UserDto MapToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Gender = user.Gender,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                RoleId = user.RoleId,
                DateOfBirth = user.DateOfBirth
            };
        }
    }
}