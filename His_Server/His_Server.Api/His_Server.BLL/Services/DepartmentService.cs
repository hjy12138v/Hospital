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
    /// 科室业务实现：Department 与 DepartmentDto 的映射与校验。
    /// </summary>
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _repository;
        private readonly IMapper _mapper;

        public DepartmentService(IDepartmentRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<DepartmentDto>> GetAllAsync()
        {
            var list = await _repository.GetAllAsync();
            return list.Select(d => _mapper.Map<DepartmentDto>(d)).ToList();
        }

        public async Task<DepartmentDto?> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<DepartmentDto>(entity);
        }

        public async Task<int> CreateAsync(DepartmentDto dto)
        {
            Validate(dto);
            var entity = _mapper.Map<Department>(dto);
            return await _repository.AddAsync(entity);
        }

        public async Task<bool> UpdateAsync(int id, DepartmentDto dto)
        {
            Validate(dto);
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return false;
            _mapper.Map(dto, entity);
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

        // 映射逻辑由 AutoMapper 承担
    }
}