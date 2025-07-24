using Application.DTOs.Permissions;
using Application.Interfaces.Abstract;

namespace Application.Interfaces.Services.AccessControl
{
    public interface IPermissionService : IBasePagingFilterService<PermissionDto, PermissionFilterDto>
    {
        // Thêm các nghiệp vụ nâng cao nếu cần
        Task<bool> ExistsByNameAsync(string name);

        Task<PermissionDto?> GetByNameAsync(string name);
    }
}
