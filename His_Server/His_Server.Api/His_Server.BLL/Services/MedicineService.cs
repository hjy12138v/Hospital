using His_Server.DAL.Repositories;
using His_Server.Model.EntityDto;
using His_Server.Model.EntityMap;
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

        public MedicineService(IMedicineRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<MedicineDto>> GetAllAsync()
        {
            var list = await _repository.GetAllAsync();
            return list.Select(MapToDto).ToList();
        }

        public async Task<MedicineDto?> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity == null ? null : MapToDto(entity);
        }

        public async Task<int> CreateAsync(MedicineDto dto)
        {
            Validate(dto);
            var entity = MapToEntity(dto);
            return await _repository.AddAsync(entity);
        }

        public async Task<bool> UpdateAsync(int id, MedicineDto dto)
        {
            Validate(dto);
            var entity = MapToEntity(dto);
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

        private static MedicineDto MapToDto(Medicine entity)
        {
            return new MedicineDto
            {
                MedicineId = entity.MedicineId,
                Name = entity.Name,
                Description = entity.Description,
                Stock = entity.Stock,
                UnitPrice = entity.UnitPrice,
                ExpiryDate = entity.ExpiryDate,
                Type = entity.Type
            };
        }

        private static Medicine MapToEntity(MedicineDto dto)
        {
            return new Medicine
            {
                MedicineId = dto.MedicineId,
                Name = dto.Name,
                Description = dto.Description,
                Stock = dto.Stock,
                UnitPrice = dto.UnitPrice,
                ExpiryDate = dto.ExpiryDate,
                Type = dto.Type
            };
        }
    }
}