using Application.DTOs.Users.Responses;
using Domain.Entities;

namespace Application.Mappings
{
    public class UserMapper
    {
        public static UserDto ToDto(User entity) => new UserDto
        {
            UserId = entity.UserId,
            FullName = entity.FullName,
            Email = entity.Email,
            PasswordHash = entity.PasswordHash,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            LastLoginAt = entity.LastLoginAt,
            PhoneNumber = entity.PhoneNumber,
            DateOfBirth = entity.DateOfBirth,
            Gender = entity.Gender,
            AvatarUrl = entity.AvatarUrl,
            Field1 = entity.Field1,
            Field2 = entity.Field2,
            Field3 = entity.Field3,
            Field4 = entity.Field4,
            Field5 = entity.Field5,
            Flag = entity.Flag,
            IsActive = entity.IsActive
        };

        public static User ToEntity(UserDto dto) => new User
        {
            UserId = dto.UserId,
            FullName = dto.FullName,
            Email = dto.Email,
            PasswordHash = dto.PasswordHash,
            PhoneNumber = dto.PhoneNumber,
            UpdatedAt = dto.UpdatedAt ?? DateTime.UtcNow,
            Gender = dto.Gender,
            DateOfBirth = dto.DateOfBirth,
            AvatarUrl = dto.AvatarUrl,
            Field1 = dto.Field1,
            Field2 = dto.Field2,
            Field3 = dto.Field3,
            Field4 = dto.Field4,
            Field5 = dto.Field5,
            Flag = dto.Flag,
            IsActive = dto.IsActive
        };
    }
}
