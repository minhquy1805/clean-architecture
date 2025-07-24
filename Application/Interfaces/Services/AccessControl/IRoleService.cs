using Application.DTOs.Roles;
using Application.Interfaces.Abstract;

namespace Application.Interfaces.Services.AccessControl
{
    public interface IRoleService : IBasePagingFilterService<RoleDto, RoleFilterDto>
    {
        // Nếu cần custom logic riêng cho Role, thêm tại đây
        Task<RoleDto?> GetByNameAsync(string roleName);
        Task<bool> ExistsByNameAsync(string roleName);
    }
}
