using Application.DTOs.LoginHistories;
using Domain.Entities;

namespace Application.Mappings
{
    public static class LoginHistoryMapper
    {
        public static LoginHistoryDto ToDto(LoginHistory entity)
        {
            return new LoginHistoryDto
            {
                LoginId = entity.LoginId,
                UserId = entity.UserId,
                IsSuccess = entity.IsSuccess,
                IpAddress = entity.IpAddress,
                UserAgent = entity.UserAgent,
                Message = entity.Message,
                CreatedAt = entity.CreatedAt
            };
        }

        public static LoginHistory ToEntity(LoginHistoryDto dto)
        {
            return new LoginHistory
            {
                LoginId = dto.LoginId,
                UserId = dto.UserId,
                IsSuccess = dto.IsSuccess,
                IpAddress = dto.IpAddress,
                UserAgent = dto.UserAgent,
                Message = dto.Message,
                CreatedAt = dto.CreatedAt
            };
        }
    }
}
