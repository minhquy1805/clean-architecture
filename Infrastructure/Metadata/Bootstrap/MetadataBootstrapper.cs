using Infrastructure.Abstractions.Metadata;
using Infrastructure.Metadata.Users;
using Domain.Entities;

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
        }
    }
}
