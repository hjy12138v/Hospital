using His_Server.BLL.Services;
using His_Server.Model.EntityDto;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace His_Server.Api.Controllers
{
    /// <summary>
    /// 用户接口：提供对 User 表的增删改查。
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        /// <summary>
        /// 构造函数依赖注入
        /// </summary>
        /// <param name="userService"></param>
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// 获取所有用户
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<UserDto>>> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        /// <summary>
        /// 按Id获取用户
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<UserDto>> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        /// <summary>
        /// 新增用户（密码在请求体中传入）
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<int>> Create([FromBody] CreateUserRequest request)
        {
            var newId = await _userService.CreateAsync(ToDto(request), request.Password);
            return CreatedAtAction(nameof(GetById), new { id = newId }, newId);
        }

        /// <summary>
        /// 更新用户（不修改密码）
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UserDto dto)
        {
            var ok = await _userService.UpdateAsync(id, dto);
            if (!ok) return NotFound();
            return NoContent();
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _userService.DeleteAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }

        private static UserDto ToDto(CreateUserRequest r)
        {
            return new UserDto
            {
                Id = r.Id,
                Name = r.Name,
                Gender = r.Gender,
                PhoneNumber = r.PhoneNumber,
                Email = r.Email,
                RoleId = r.RoleId,
                DateOfBirth = r.DateOfBirth
            };
        }

        /// <summary>
        /// 用户登录验证：验证 Name 与 Password。
        /// 成功返回用户基本信息，失败返回 401。
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> UserLogin([FromBody] UserLoginRequest request)
        {
            var user = await _userService.UserLoginAsync(request.Name, request.Password);
            if (user == null)
            {
                return Unauthorized(new { message = "用户名或密码错误" });
            }
            return Ok(user);
        }
    }

    /// <summary>
    /// 创建用户的请求模型，包含密码字段。
    /// </summary>
    public class CreateUserRequest : UserDto
    {
        /// <summary>
        /// 明文密码（示例项目使用，生产环境请做加密与安全处理）
        /// </summary>
        public string Password { get; set; }
    }

    /// <summary>
    /// 用户登录请求模型。
    /// </summary>
    public class UserLoginRequest
    {
        public string Name { get; set; }
        public string Password { get; set; }
    }
}