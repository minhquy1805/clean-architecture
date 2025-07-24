using Application.Common.Helpers;
using Application.DTOs.Roles;
using Application.Enums;
using Application.Interfaces.Repositories.AccessControl;
using Domain.Entities.AccessControl;
using Infrastructure.Database.Abstractions;
using Infrastructure.Database.Extensions;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Infrastructure.Repositories.SQL.Roles
{
    public class SqlRoleRepository : BaseRepository<Role>, IRoleRepository
    {
        public SqlRoleRepository(IDatabaseHelper databaseHelper, string connectionString)
            : base(databaseHelper, connectionString) { }

        protected override string TableName => "Role";

        protected override string PrimaryKeyParamName => "@RoleId";

        protected override Func<DataRow, Role> MapRow => row => new Role
        {
            RoleId = row.Get<int>("RoleId"),
            RoleName = row.GetString("RoleName")!,
            Description = row.GetString("Description"),
            Flag = row.GetString("Flag")!,
            CreatedAt = row.Get<DateTime>("CreatedAt"),
        };

        protected override List<SqlParameter> BuildInsertParams(Role role)
           => role.ToSqlParameters(SqlOperationType.Create);

        protected override List<SqlParameter> BuildUpdateParams(Role role)
            => role.ToSqlParameters(SqlOperationType.Update);

        public async Task<int> GetRecordCountWhereDynamicAsync(RoleFilterDto filter)
        {
            var dt = await _databaseHelper.GetDataTableAsync(
                _connectionString,
                "Role_GetRecordCountWhereDynamic",
                filter.ToSqlParametersWithWhereClauseOnly(),
                CommandType.StoredProcedure
            );

            return Convert.ToInt32(dt.Rows[0]["RecordCount"]);
        }

        public async Task<IEnumerable<Role>> SelectSkipAndTakeWhereDynamicAsync(RoleFilterDto filter)
        {
            filter.SortBy = SortFieldValidator.Validate(filter.SortBy, RoleFilterDto.AllowedSortFields, "CreatedAt");
            filter.SortDirection = SortFieldValidator.ValidateDirection(filter.SortDirection);

            var dt = await _databaseHelper.GetDataTableAsync(
                _connectionString,
                "Role_SelectSkipAndTakeWhereDynamic",
                filter.ToSqlParametersWithPagingAndSorting(),
                CommandType.StoredProcedure
            );
            return MapList(dt);
        }

        public async Task<Role?> GetByNameAsync(string roleName)
        {
            var dt = await _databaseHelper.GetDataTableAsync(
                _connectionString,
                "Role_SelectByName",
                new List<SqlParameter>
                {
                    new SqlParameter("@RoleName", roleName)
                },
                CommandType.StoredProcedure
            );

            return dt.Rows.Count > 0 ? MapRow(dt.Rows[0]) : null;
        }

        public async Task<bool> ExistsByNameAsync(string roleName)
        {
            var dt = await _databaseHelper.GetDataTableAsync(
                _connectionString,
                "Role_SelectByName",
                new List<SqlParameter>
                {
            new SqlParameter("@RoleName", roleName)
                },
                CommandType.StoredProcedure
            );

            return dt.Rows.Count > 0;
        }
    }
}
