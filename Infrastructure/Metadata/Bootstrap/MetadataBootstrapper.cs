using Infrastructure.Abstractions.Metadata;
using Infrastructure.Metadata.Users;
using Domain.Entities;
using Domain.Entities.AccessControl;
using Infrastructure.Metadata.Role;
using Infrastructure.Metadata.Permissions;

namespace Infrastructure.Metadata.Bootstrap
{
    public static class MetadataBootstrapper
    {
        public static void Initialize() 
        {
            MetadataRegistry.Register<User>(
                UserSqlMetadata.CreateFields,
                UserSqlMetadata.UpdateFields
            );

            MetadataRegistry.Register<Domain.Entities.AccessControl.Role>(
                RoleSqlMetadata.CreateFields,
                RoleSqlMetadata.UpdateFields
            );

            MetadataRegistry.Register<Permission>(
                PermissionSqlMetadata.CreateFields,
                PermissionSqlMetadata.UpdateFields
            );
        }

    }
}
