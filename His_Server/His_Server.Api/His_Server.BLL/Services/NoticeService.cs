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
    /// 通知业务实现：Notice 与 NoticeDto 的映射与校验。
    /// </summary>
    public class NoticeService : INoticeService
    {
        private readonly INoticeRepository _repository;
        private readonly IMapper _mapper;

        public NoticeService(INoticeRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<NoticeDto>> GetAllAsync()
        {
            var list = await _repository.GetAllAsync();
            return list.Select(n => _mapper.Map<NoticeDto>(n)).ToList();
        }

        public async Task<NoticeDto?> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<NoticeDto>(entity);
        }

        public async Task<int> CreateAsync(NoticeDto dto)
        {
            Validate(dto);
            var entity = _mapper.Map<Notice>(dto);
            return await _repository.AddAsync(entity);
        }

        public async Task<bool> UpdateAsync(int id, NoticeDto dto)
        {
            Validate(dto);
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return false;
            _mapper.Map(dto, entity);
            entity.NoticeId = id;
            return await _repository.UpdateAsync(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        private static void Validate(NoticeDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
                throw new System.ArgumentException("Notice title is required");
            if (string.IsNullOrWhiteSpace(dto.Content))
                throw new System.ArgumentException("Notice content is required");
        }

        // 映射逻辑由 AutoMapper 承担
    }
}