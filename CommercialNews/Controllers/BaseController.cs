using CommercialNews.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CommercialNews.Controllers
{
    /// <summary>
    /// BaseController: common helpers for ApiResponse, user info, IP, etc.
    /// </summary>
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        // ✅ Response helper
        protected OkObjectResult OkResponse<T>(T data, string? message = null)
            => Ok(new ApiResponse<T>(data, message));

        protected NotFoundObjectResult NotFoundResponse(string message)
            => NotFound(new ApiResponse<string>(message) { Success = false });

        protected BadRequestObjectResult BadRequestResponse(string message)
            => BadRequest(new ApiResponse<string>(message) { Success = false });

        // ✅ Current User ID (int)
        protected int? CurrentUserId
        {
            get
            {
                var value = User?.FindFirstValue(ClaimTypes.NameIdentifier);
                return int.TryParse(value, out var id) ? id : null;
            }
        }

        // ✅ Current Email
        protected string? CurrentUserEmail =>
            User?.FindFirstValue(ClaimTypes.Email);

        // ✅ Current Role
        protected string? CurrentUserRole =>
            User?.FindFirstValue(ClaimTypes.Role);

        // ✅ Client IP Address
        protected string? GetIpAddress() =>
            HttpContext?.Connection?.RemoteIpAddress?.ToString();
    }
}
