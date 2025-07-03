using Application.DTOs.Users.Filters;
using Application.DTOs.Users.Requests;
using Application.DTOs.Users.Responses;
using Application.Interfaces.Abstract;
using Application.Interfaces.Common;

namespace Application.Interfaces.Services
{
    public interface IUserService : IBasePagingFilterService<UserDto, UserFilterDto>
    {
        // 📌 Đăng ký user mới (input: DTO Register, output: new Id)
        Task<int> RegisterUserAsync(UserRegisterDto dto, string verifyLinkBase);

        // 📌 Get chi tiết theo Email (output: DTO)
        Task<UserDto?> GetByEmailAsync(string email);

        // 📌 Get Dropdown (UserId + FullName)
        Task<IEnumerable<UserDropDownDto>> GetDropDownListDataAsync();

        // 📌 Update user (input: DTO)
        Task UpdateUserAsync(UserDto dto);

        // 📌 Update profile (chính user cập nhật)
        Task UpdateOwnProfileAsync(int userId, UpdateOwnProfileRequest dto);

        // 📌 Đổi mật khẩu
        Task ChangePasswordAsync(int userId, ChangePasswordRequest request);

        // 📌 Quên mật khẩu & reset
        Task ResetPasswordAsync(ResetPasswordRequest request);
        Task ForgotPasswordAsync(string email, string verifyLinkBase);

        // 📌 Xoá mềm và khôi phục
        Task SoftDeleteUserAsync(int userId);
        Task RestoreUserAsync(int userId);

    }
}
