using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CommercialNews.Controllers
{
    [Route("api/v1/")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // POST: api/User/register
        [Route("[controller]/register")]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            try
            {
                var id = await _userService.RegisterUserAsync(user);
                return Ok(new { message = "Đăng ký thành công!", userId = id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/User/{id}
        [Route("[controller]/selectbyid")]
        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        // GET: api/User/email/{email}
        [Route("[controller]/getbyemail")]
        [HttpGet]
        public async Task<IActionResult> GetByEmail(string email)
        {
            var user = await _userService.GetByEmailAsync(email);
            if (user == null) return NotFound();
            return Ok(user);
        }

        // GET: api/User
        [Route("[controller]/selectall")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        // PUT: api/User/{id}
        [Route("[controller]/update")]
        [HttpPut]
        public async Task<IActionResult> Update(int id, [FromBody] User user)
        {
            if (id != user.UserId)
                return BadRequest(new { message = "UserId không khớp!" });

            await _userService.UpdateUserAsync(user);
            return Ok(new { message = "Cập nhật thành công!" });
        }

        // DELETE: api/User/{id}
        [Route("[controller]/delete")]
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            await _userService.DeleteUserAsync(id);
            return Ok(new { message = "Xóa thành công!" });
        }
    }
}
