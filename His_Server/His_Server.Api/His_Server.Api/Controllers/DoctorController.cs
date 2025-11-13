using His_Server.BLL.Services;
using His_Server.Model.EntityDto;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace His_Server.Api.Controllers
{
    /// <summary>
    /// Doctor CRUD 接口
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorService _service;

        public DoctorController(IDoctorService service)
        {
            _service = service;
        }

        /// <summary>
        /// 获取全部医生
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<DoctorDto>>> GetAll()
        {
            var list = await _service.GetAllAsync();
            return Ok(list);
        }

        /// <summary>
        /// 根据 Id 获取医生
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<DoctorDto?>> GetById(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        /// <summary>
        /// 新建医生
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<int>> Create(DoctorDto dto)
        {
            var id = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id }, id);
        }

        /// <summary>
        /// 更新医生
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(int id, DoctorDto dto)
        {
            var ok = await _service.UpdateAsync(id, dto);
            if (!ok) return NotFound();
            return NoContent();
        }

        /// <summary>
        /// 删除医生
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var ok = await _service.DeleteAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}