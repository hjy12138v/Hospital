using His_Server.DAL.Repositories;
using His_Server.Model.EntityDto;
using His_Server.Model.EntityMap;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace His_Server.BLL.Services
{
    /// <summary>
    /// 科室业务实现：Department 与 DepartmentDto 的映射与校验。
    /// </summary>
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _repository;

        public DepartmentService(IDepartmentRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<DepartmentDto>> GetAllAsync()
        {
            var list = await _repository.GetAllAsync();
            return list.Select(MapToDto).ToList();
        }

        public async Task<DepartmentDto?> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity == null ? null : MapToDto(entity);
        }

        public async Task<int> CreateAsync(DepartmentDto dto)
        {
            Validate(dto);
            var entity = MapToEntity(dto);
            return await _repository.AddAsync(entity);
        }

        public async Task<bool> UpdateAsync(int id, DepartmentDto dto)
        {
            Validate(dto);
            var entity = MapToEntity(dto);
            entity.DepartmentId = id;
            return await _repository.UpdateAsync(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        private static void Validate(DepartmentDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                throw new System.ArgumentException("Department name is required");
            }
        }

        private static DepartmentDto MapToDto(Department entity)
        {
            return new DepartmentDto
            {
                DepartmentId = entity.DepartmentId,
                Name = entity.Name,
                Description = entity.Description,
                Location = entity.Location
            };
        }

        private static Department MapToEntity(DepartmentDto dto)
        {
            return new Department
            {
                DepartmentId = dto.DepartmentId,
                Name = dto.Name,
                Description = dto.Description,
                Location = dto.Location
            };
        }
    }
}