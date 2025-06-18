using Application.Interfaces;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<int> RegisterUserAsync(User user)
        {
            // Logic auto cấp Admin
            if (user.Email == "minhquy073@gmail.com")
                user.Role = "Admin";
            else
                user.Role = "User";

            // Check email tồn tại chưa
            var existing = await _userRepository.GetByEmailAsync(user.Email);
            if (existing != null)
                throw new Exception("Email đã được đăng ký!");

            // TODO: Thêm hash password nếu cần

            return await _userRepository.InsertAsync(user);
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _userRepository.GetByEmailAsync(email);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            await _userRepository.UpdateAsync(user);
        }

        public async Task DeleteUserAsync(int id)
        {
            await _userRepository.DeleteAsync(id);
        }
    }
}
