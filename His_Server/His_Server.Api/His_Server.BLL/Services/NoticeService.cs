using His_Server.DAL.Repositories;
using His_Server.Model.EntityDto;
using His_Server.Model.EntityMap;
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

        public NoticeService(INoticeRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<NoticeDto>> GetAllAsync()
        {
            var list = await _repository.GetAllAsync();
            return list.Select(MapToDto).ToList();
        }

        public async Task<NoticeDto?> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity == null ? null : MapToDto(entity);
        }

        public async Task<int> CreateAsync(NoticeDto dto)
        {
            Validate(dto);
            var entity = MapToEntity(dto);
            return await _repository.AddAsync(entity);
        }

        public async Task<bool> UpdateAsync(int id, NoticeDto dto)
        {
            Validate(dto);
            var entity = MapToEntity(dto);
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

        private static NoticeDto MapToDto(Notice entity)
        {
            return new NoticeDto
            {
                NoticeId = entity.NoticeId,
                Title = entity.Title,
                Content = entity.Content,
                PublishDate = entity.PublishDate,
                Type = entity.Type,
                SenderId = entity.SenderId
            };
        }

        private static Notice MapToEntity(NoticeDto dto)
        {
            return new Notice
            {
                NoticeId = dto.NoticeId,
                Title = dto.Title,
                Content = dto.Content,
                PublishDate = dto.PublishDate,
                Type = dto.Type,
                SenderId = dto.SenderId
            };
        }
    }
}