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
                LoginHistoryId = entity.LoginHistoryId,
                UserId = entity.UserId,
                IsSuccess = entity.IsSuccess,
                IpAddress = entity.IpAddress,
                UserAgent = entity.UserAgent,
                Device = entity.Device,
                OS = entity.OS,
                Browser = entity.Browser,
                Message = entity.Message,
                CreatedAt = entity.CreatedAt
            };
        }

        public static LoginHistory ToEntity(LoginHistoryDto dto)
        {
            return new LoginHistory
            {
                LoginHistoryId = dto.LoginHistoryId,
                UserId = dto.UserId,
                IsSuccess = dto.IsSuccess,
                IpAddress = dto.IpAddress,
                UserAgent = dto.UserAgent,
                Device = dto.Device,
                OS = dto.OS,
                Browser = dto.Browser,
                Message = dto.Message,
                CreatedAt = dto.CreatedAt
            };
        }
    }
}
