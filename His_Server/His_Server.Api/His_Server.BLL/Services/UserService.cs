using His_Server.DAL.Repositories;
using His_Server.Model.EntityDto;
using His_Server.Model.EntityMap;
using AutoMapper;
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
        private readonly IMapper _mapper;

        public UserService(IUserRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<UserDto>> GetAllAsync()
        {
            var list = await _repository.GetAllAsync();
            return list.Select(u => _mapper.Map<UserDto>(u)).ToList();
        }

        public async Task<UserDto?> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<UserDto>(entity);
        }

        public async Task<int> CreateAsync(UserDto dto, string password)
        {
            // 简单校验（可根据需要扩展）
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new System.ArgumentException("用户名不能为空");
            if (string.IsNullOrWhiteSpace(password))
                throw new System.ArgumentException("密码不能为空");

            var entity = _mapper.Map<User>(dto);
            entity.Password = password; // 生产环境建议加密存储
            var newId = await _repository.AddAsync(entity);
            return newId;
        }

        public async Task<bool> UpdateAsync(int id, UserDto dto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return false;

            _mapper.Map(dto, existing); // 不修改密码
            return await _repository.UpdateAsync(existing);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<UserDto?> UserLoginAsync(string name, string password)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(password))
                throw new System.ArgumentException("用户名和密码不能为空");

            var user = await _repository.GetByNameAsync(name);
            if (user == null) return null;

            // 简单明文校验（示例）；生产环境请使用哈希比较
            if (!string.Equals(user.Password, password)) return null;

            return _mapper.Map<UserDto>(user);
        }

        // 映射逻辑已由 AutoMapper 承担
    }
}