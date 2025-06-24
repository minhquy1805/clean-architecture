using CommercialNews.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CommercialNews.Controllers
{
    /// <summary>
    /// BaseController: common helpers for ApiResponse, user info, IP.
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

        // ✅ Current user helper
        protected int CurrentUserId =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        protected string? CurrentUserEmail =>
            User.FindFirstValue(ClaimTypes.Email);

        protected string? GetIpAddress() =>
            HttpContext?.Connection?.RemoteIpAddress?.ToString();
    }
}
