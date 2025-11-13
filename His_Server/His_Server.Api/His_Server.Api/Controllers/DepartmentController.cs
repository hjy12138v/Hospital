using His_Server.BLL.Services;
using His_Server.Model.EntityDto;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace His_Server.Api.Controllers
{
    /// <summary>
    /// Department CRUD 接口
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _service;

        public DepartmentController(IDepartmentService service)
        {
            _service = service;
        }

        /// <summary>
        /// 获取全部科室
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<DepartmentDto>>> GetAll()
        {
            var list = await _service.GetAllAsync();
            return Ok(list);
        }

        /// <summary>
        /// 根据 Id 获取科室
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<DepartmentDto?>> GetById(int id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null) return NotFound();
            return Ok(dto);
        }

        /// <summary>
        /// 新建科室
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<int>> Create(DepartmentDto dto)
        {
            var id = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id }, id);
        }

        /// <summary>
        /// 更新科室
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(int id, DepartmentDto dto)
        {
            var ok = await _service.UpdateAsync(id, dto);
            if (!ok) return NotFound();
            return NoContent();
        }

        /// <summary>
        /// 删除科室
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