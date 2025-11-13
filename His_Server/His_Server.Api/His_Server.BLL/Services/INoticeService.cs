using His_Server.Model.EntityDto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace His_Server.BLL.Services
{
    /// <summary>
    /// 通知业务接口：封装 Notice 的业务逻辑与 DTO 映射。
    /// </summary>
    public interface INoticeService
    {
        Task<List<NoticeDto>> GetAllAsync();
        Task<NoticeDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(NoticeDto dto);
        Task<bool> UpdateAsync(int id, NoticeDto dto);
        Task<bool> DeleteAsync(int id);
    }
}