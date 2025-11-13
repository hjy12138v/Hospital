using His_Server.DAL.Repositories;
using His_Server.Model.EntityDto;
using His_Server.Model.EntityMap;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace His_Server.BLL.Services
{
    /// <summary>
    /// 医生业务实现：Doctor 与 DoctorDto 的映射与校验。
    /// </summary>
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository _repository;

        public DoctorService(IDoctorRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<DoctorDto>> GetAllAsync()
        {
            var list = await _repository.GetAllAsync();
            return list.Select(MapToDto).ToList();
        }

        public async Task<DoctorDto?> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity == null ? null : MapToDto(entity);
        }

        public async Task<int> CreateAsync(DoctorDto dto)
        {
            Validate(dto);
            var entity = MapToEntity(dto);
            return await _repository.AddAsync(entity);
        }

        public async Task<bool> UpdateAsync(int id, DoctorDto dto)
        {
            Validate(dto);
            var entity = MapToEntity(dto);
            entity.DoctorId = id;
            return await _repository.UpdateAsync(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        private static void Validate(DoctorDto dto)
        {
            if (dto.DepartmentId <= 0)
            {
                throw new System.ArgumentException("DepartmentId is required");
            }
            if (dto.UserId <= 0)
            {
                throw new System.ArgumentException("UserId is required");
            }
        }

        private static DoctorDto MapToDto(Doctor entity)
        {
            return new DoctorDto
            {
                DoctorId = entity.DoctorId,
                UserId = entity.UserId,
                DepartmentId = entity.DepartmentId,
                Position = entity.Position,
                Specialty = entity.Specialty
            };
        }

        private static Doctor MapToEntity(DoctorDto dto)
        {
            return new Doctor
            {
                DoctorId = dto.DoctorId,
                UserId = dto.UserId,
                DepartmentId = dto.DepartmentId,
                Position = dto.Position,
                Specialty = dto.Specialty
            };
        }
    }
}