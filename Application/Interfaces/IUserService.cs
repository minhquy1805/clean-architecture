
using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IUserService
    {
        // 📌 Đăng ký user mới (input: DTO Register, output: new Id)
        Task<int> RegisterUserAsync(UserRegisterDto dto, string verifyLinkBase);

        // 📌 Đăng nhập (input: DTO Login, output: JWT token string)
        //Task<string> LoginAsync(LoginRequest request);

        // 📌 Get chi tiết theo ID (output: DTO)
        Task<UserDto?> GetByIdAsync(int id);

        // 📌 Get chi tiết theo Email (output: DTO)
        Task<UserDto?> GetByEmailAsync(string email);

        // 📌 Get all (KHÔNG paging, chỉ khi cần export nhỏ)
        Task<IEnumerable<UserDto>> GetAllAsync();

        // 📌 Get list có paging & filter (output: DTO list)
        Task<IEnumerable<UserDto>> SelectSkipAndTakeWhereDynamicAsync(string whereCondition, int start, int rows, string sortBy);

        // 📌 Get tổng số record cho paging & filter
        Task<int> GetRecordCountWhereDynamicAsync(string whereCondition);

        // 📌 Get Dropdown (UserId + FullName)
        Task<IEnumerable<UserDropDownDto>> GetDropDownListDataAsync();

        // 📌 Update user (input: DTO)
        Task UpdateUserAsync(UserDto dto);

        // 📌 Xoá user
        Task DeleteUserAsync(int id);

        Task UpdateOwnProfileAsync(int userId, UserDto dto);
        Task ChangePasswordAsync(int userId, ChangePasswordRequest request);

        Task ResetPasswordAsync(ResetPasswordRequest request);
        Task ForgotPasswordAsync(string email, string verifyLinkBase);

        Task SoftDeleteUserAsync(int userId);
        Task RestoreUserAsync(int userId);
    }
}
