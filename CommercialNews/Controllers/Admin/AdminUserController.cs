using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Interfaces.Services;
using Shared.Helpers;
using Application.DTOs.Users.Responses;
using Application.DTOs.Users.Filters;


namespace CommercialNews.Controllers.Admin
{
    [Route("api/v1/admin/user")]
    [ApiController]
    
    public class AdminUserController : BaseController
    {
        private readonly IUserService _userService;

        public AdminUserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// ✅ Get User by Id (Admin view)
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Policy = "User:View")]
        public async Task<ActionResult> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return NotFoundResponse("User not found.");
            return OkResponse(user, "Fetched successfully!");
        }

        /// <summary>
        /// ✅ Get all users
        /// </summary>
        [HttpGet("all")]
        [Authorize(Policy = "User:View")]
        public async Task<ActionResult> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return OkResponse(users, "Fetched all users.");
        }

        /// <summary>
        /// ✅ Get users with paging & filter
        /// </summary>
        [HttpPost("paging")]
        [Authorize(Policy = "User:View")]
        public async Task<IActionResult> GetPaging([FromBody] UserFilterDto filter)
        {
            
            var (data, totalRecords) = await _userService.GetPagingAsync(filter);

            var pagination = new
            {
                currentPage = filter.CurrentPage,
                totalPages = PaginationHelper.CalculateTotalPages(totalRecords, filter.NumberOfRows),
                totalRecords,
                rowsPerPage = filter.NumberOfRows,
                pagesToShow = GridConfig.NumberOfPagesToShow
            };

            return Ok(new { data, pagination });
        }


        /// <summary>
        /// ✅ Get users for dropdown
        /// </summary>
        [HttpGet("dropdown")]
        [Authorize(Policy = "User:View")]
        public async Task<ActionResult> GetDropDownList()
        {
            var dropdown = await _userService.GetDropDownListDataAsync();
            return OkResponse(dropdown, "Fetched dropdown list.");
        }

        /// <summary>
        /// ✅ Update user (Admin)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Policy = "User:Update")]
        public async Task<ActionResult> Update(int id, [FromBody] UserDto dto)
        {
            if (id != dto.UserId)
                return BadRequestResponse("ID mismatch.");

            await _userService.UpdateUserAsync(dto);
            return OkResponse<string>(null!, "User updated successfully!");
        }

        /// <summary>
        /// ✅ Soft Delete user: IsActive = false & revoke token
        /// </summary>
        [HttpPut("soft-delete/{id}")]
        [Authorize(Policy = "User:Delete")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            await _userService.SoftDeleteUserAsync(id);
            return OkResponse<string>(null!, "User has been soft deleted (IsActive = 0)!");
        }

        /// <summary>
        /// ✅ Restore user: IsActive = true
        /// </summary>
        [HttpPut("restore/{id}")]
        [Authorize(Policy = "User:Update")]
        public async Task<IActionResult> Restore(int id)
        {
            await _userService.RestoreUserAsync(id);
            return OkResponse<string>(null!, "User has been restored (IsActive = 1)!");
        }

        /// <summary>
        /// ✅ Delete user (Admin)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Policy = "User:Delete")]
        public async Task<ActionResult> Delete(int id)
        {
            await _userService.DeleteAsync(id);
            return OkResponse<string>(null!, "User deleted successfully!");
        }
    }
}
