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
    /// 医生业务实现：Doctor 与 DoctorDto 的映射与校验。
    /// </summary>
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository _repository;
        private readonly IMapper _mapper;

        public DoctorService(IDoctorRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<DoctorDto>> GetAllAsync()
        {
            var list = await _repository.GetAllAsync();
            return list.Select(d => _mapper.Map<DoctorDto>(d)).ToList();
        }

        public async Task<DoctorDto?> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<DoctorDto>(entity);
        }

        public async Task<int> CreateAsync(DoctorDto dto)
        {
            Validate(dto);
            var entity = _mapper.Map<Doctor>(dto);
            return await _repository.AddAsync(entity);
        }

        public async Task<bool> UpdateAsync(int id, DoctorDto dto)
        {
            Validate(dto);
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return false;
            _mapper.Map(dto, entity);
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

        // 映射逻辑由 AutoMapper 承担
    }
}