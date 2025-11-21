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
    /// 药品业务实现：Medicine 与 MedicineDto 的映射与校验。
    /// </summary>
    public class MedicineService : IMedicineService
    {
        private readonly IMedicineRepository _repository;
        private readonly IMapper _mapper;

        public MedicineService(IMedicineRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<MedicineDto>> GetAllAsync()
        {
            var list = await _repository.GetAllAsync();
            return list.Select(m => _mapper.Map<MedicineDto>(m)).ToList();
        }

        public async Task<MedicineDto?> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<MedicineDto>(entity);
        }

        public async Task<int> CreateAsync(MedicineDto dto)
        {
            Validate(dto);
            var entity = _mapper.Map<Medicine>(dto);
            return await _repository.AddAsync(entity);
        }

        public async Task<bool> UpdateAsync(int id, MedicineDto dto)
        {
            Validate(dto);
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return false;
            _mapper.Map(dto, entity);
            entity.MedicineId = id;
            return await _repository.UpdateAsync(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        private static void Validate(MedicineDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new System.ArgumentException("Medicine name is required");
            if (dto.Stock < 0)
                throw new System.ArgumentException("Stock cannot be negative");
            if (dto.UnitPrice < 0)
                throw new System.ArgumentException("UnitPrice cannot be negative");
        }

        // 映射逻辑由 AutoMapper 承担
    }
}