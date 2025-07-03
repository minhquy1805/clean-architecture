using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IUserVerificationService
    {
        /// <summary>
        /// Tạo token xác minh, lưu DB và trả về token (hoặc link)
        /// </summary>
        Task<string> CreateVerificationTokenAsync(int userId);

        /// <summary>
        /// Xác minh token, cập nhật User.Flag = T và mark IsUsed
        /// </summary>
        Task VerifyTokenAsync(string token);

    }
}
