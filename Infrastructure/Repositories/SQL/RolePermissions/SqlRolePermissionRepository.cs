using Application.Interfaces.Repositories.AccessControl;
using Domain.Entities.AccessControl;
using Infrastructure.Database.Abstractions;
using Infrastructure.Database.Enums;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Infrastructure.Repositories.SQL.RolePermissions
{
    public class SqlRolePermissionRepository : IRolePermissionRepository
    {
        private readonly IDatabaseHelper _databaseHelper;
        private readonly string _connectionString;

        public SqlRolePermissionRepository(IDatabaseHelper databaseHelper, string connectionString)
        {
            _databaseHelper = databaseHelper;
            _connectionString = connectionString;
        }

        public async Task InsertAsync(int roleId, int permissionId)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@RoleId", roleId),
                new SqlParameter("@PermissionId", permissionId)
            };

            await _databaseHelper.ExecuteSqlCommandAsync(
                _connectionString,
                "RolePermission_Insert",
                parameters,
                CommandType.StoredProcedure,
                DatabaseOperationType.Create,
                isPrimaryKeyGuid: false
            );
        }

        public async Task DeleteAsync(int roleId, int permissionId)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@RoleId", roleId),
                new SqlParameter("@PermissionId", permissionId)
            };

            await _databaseHelper.ExecuteSqlCommandAsync(
                _connectionString,
                "RolePermission_Delete",
                parameters,
                CommandType.StoredProcedure,
                DatabaseOperationType.Delete
            );
        }

        public async Task<IEnumerable<Permission>> GetPermissionsByRoleIdAsync(int roleId)
        {
            var dt = await _databaseHelper.GetDataTableAsync(
                _connectionString,
                "RolePermission_SelectByRoleId",
                new List<SqlParameter> { new SqlParameter("@RoleId", roleId) },
                CommandType.StoredProcedure
            );

            return dt.AsEnumerable().Select(row => new Permission
            {
                PermissionId = row.Field<int>("PermissionId"),
                Name = row.Field<string>("Name")!,
                Module = row.Field<string?>("Module"),
                Action = row.Field<string?>("Action"),
                Description = row.Field<string?>("Description"),
                Flag = row.Field<string>("Flag")!,
                CreatedAt = row.Field<DateTime>("CreatedAt")
            });
        }

        public async Task<IEnumerable<int>> GetPermissionIdsByRoleIdAsync(int roleId)
        {
            var dt = await _databaseHelper.GetDataTableAsync(
                _connectionString,
                "RolePermission_SelectByRoleId",
                new List<SqlParameter> { new SqlParameter("@RoleId", roleId) },
                CommandType.StoredProcedure
            );

            return dt.AsEnumerable().Select(row => row.Field<int>("PermissionId"));
        }

        public async Task<IEnumerable<Permission>> GetPermissionsByRoleIdsAsync(IEnumerable<int> roleIds)
        {
            var joinedIds = string.Join(",", roleIds);

            var dt = await _databaseHelper.GetDataTableAsync(
                _connectionString,
                "RolePermission_SelectByRoleIds",
                new List<SqlParameter>
                {
                    new SqlParameter("@RoleIds", joinedIds)
                },
                CommandType.StoredProcedure
            );

            return dt.AsEnumerable().Select(row => new Permission
            {
                PermissionId = row.Field<int>("PermissionId"),
                Name = row.Field<string>("Name")!,
                Module = row.Field<string?>("Module"),
                Action = row.Field<string?>("Action"),
                Description = row.Field<string?>("Description"),
                Flag = row.Field<string>("Flag")!,
                CreatedAt = row.Field<DateTime?>("CreatedAt")
            });
        }


    }
}
