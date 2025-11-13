using His_Server.BLL.Services;
using His_Server.Model.EntityDto;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace His_Server.Api.Controllers
{
    /// <summary>
    /// 药品接口：提供对 Medicine 表的增删改查。
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class MedicineController : ControllerBase
    {
        private readonly IMedicineService _service;

        public MedicineController(IMedicineService service)
        {
            _service = service;
        }

        /// <summary>
        /// 获取所有药品
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<MedicineDto>>> GetAll()
        {
            var items = await _service.GetAllAsync();
            return Ok(items);
        }

        /// <summary>
        /// 按Id获取药品
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<MedicineDto>> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        /// <summary>
        /// 新增药品
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<int>> Create([FromBody] MedicineDto dto)
        {
            var newId = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = newId }, newId);
        }

        /// <summary>
        /// 更新药品
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] MedicineDto dto)
        {
            var ok = await _service.UpdateAsync(id, dto);
            if (!ok) return NotFound();
            return NoContent();
        }

        /// <summary>
        /// 删除药品
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _service.DeleteAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}