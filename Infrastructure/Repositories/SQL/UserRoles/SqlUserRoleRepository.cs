using Application.Interfaces.Repositories.AccessControl;
using Domain.Entities.AccessControl;
using Infrastructure.Database.Abstractions;
using Infrastructure.Database.Enums;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Infrastructure.Repositories.SQL.UserRoles
{
    public class SqlUserRoleRepository : IUserRoleRepository
    {
        private readonly IDatabaseHelper _databaseHelper;
        private readonly string _connectionString;

        public SqlUserRoleRepository(IDatabaseHelper databaseHelper, string connectionString)
        {
            _databaseHelper = databaseHelper;
            _connectionString = connectionString;
        }

        public async Task InsertAsync(int userId, int roleId)
        {
            await _databaseHelper.ExecuteSqlCommandAsync(
                _connectionString,
                "UserRole_Insert",
                new List<SqlParameter>
                {
                    new SqlParameter("@UserId", userId),
                    new SqlParameter("@RoleId", roleId)
                },
                CommandType.StoredProcedure,
                DatabaseOperationType.Create,
                isPrimaryKeyGuid: false
            );
        }

        public async Task DeleteAsync(int userId, int roleId)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@UserId", userId),
                new SqlParameter("@RoleId", roleId)
            };

            await _databaseHelper.ExecuteSqlCommandAsync(
                _connectionString,
                "UserRole_Delete",
                parameters,
                CommandType.StoredProcedure,
                DatabaseOperationType.Delete
            );
        }

        public async Task<IEnumerable<Role>> GetRolesByUserIdAsync(int userId)
        {
            var dt = await _databaseHelper.GetDataTableAsync(
                _connectionString,
                "UserRole_SelectByUserId",
                new List<SqlParameter> { new SqlParameter("@UserId", userId) },
                CommandType.StoredProcedure
            );

            return dt.AsEnumerable().Select(row => new Role
            {
                RoleId = row.Field<int>("RoleId"),
                RoleName = row.Field<string>("RoleName")!,
                Description = row.Field<string?>("Description"),
                Flag = row.Field<string>("Flag")!,
                CreatedAt = row.Field<DateTime>("CreatedAt")
            });
        }

        public async Task<IEnumerable<int>> GetRoleIdsByUserIdAsync(int userId)
        {
            var dt = await _databaseHelper.GetDataTableAsync(
                _connectionString,
                "UserRole_SelectByUserId",
                new List<SqlParameter> { new SqlParameter("@UserId", userId) },
                CommandType.StoredProcedure
            );

            return dt.AsEnumerable().Select(row => row.Field<int>("RoleId"));
        }

        public async Task<IEnumerable<(int UserId, Role Role)>> GetRolesByUserIdsAsync(IEnumerable<int> userIds)
        {
            var userIdString = string.Join(",", userIds);

            var dt = await _databaseHelper.GetDataTableAsync(
                _connectionString,
                "UserRole_SelectByUserIds",
                new List<SqlParameter> {
            new SqlParameter("@UserIds", userIdString)
                },
                CommandType.StoredProcedure
            );

            return dt.AsEnumerable().Select(row => (
                row.Field<int>("UserId"),
                new Role
                {
                    RoleId = row.Field<int>("RoleId"),
                    RoleName = row.Field<string>("RoleName")!,
                    Description = row.Field<string?>("Description"),
                    Flag = row.Field<string>("Flag")!,
                    CreatedAt = row.Field<DateTime>("CreatedAt")
                }
            ));
        }

    }
}
