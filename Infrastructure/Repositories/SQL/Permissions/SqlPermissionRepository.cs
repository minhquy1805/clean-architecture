using Application.Common.Helpers;
using Application.DTOs.Permissions;
using Application.Enums;
using Application.Interfaces.Repositories.AccessControl;
using Domain.Entities.AccessControl;
using Infrastructure.Database.Abstractions;
using Infrastructure.Database.Extensions;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Infrastructure.Repositories.SQL.Permissions
{
    public class SqlPermissionRepository : BaseRepository<Permission>, IPermissionRepository
    {
        public SqlPermissionRepository(IDatabaseHelper databaseHelper, string connectionString)
           : base(databaseHelper, connectionString) { }

        protected override string TableName => "Permission";

        protected override string PrimaryKeyParamName => "@PermissionId";

        protected override Func<DataRow, Permission> MapRow => row => new Permission
        {
            PermissionId = row.Get<int>("PermissionId"),
            Name = row.GetString("Name")!,
            Module = row.GetString("Module"),
            Action = row.GetString("Action"),
            Description = row.GetString("Description"),
            Flag = row.GetString("Flag")!,
            CreatedAt = row.Get<DateTime>("CreatedAt"),
        };

        protected override List<SqlParameter> BuildInsertParams(Permission permission)
            => permission.ToSqlParameters(SqlOperationType.Create);

        protected override List<SqlParameter> BuildUpdateParams(Permission permission)
            => permission.ToSqlParameters(SqlOperationType.Update);

        public async Task<int> GetRecordCountWhereDynamicAsync(PermissionFilterDto filter)
        {
            var dt = await _databaseHelper.GetDataTableAsync(
                _connectionString,
                "Permission_GetRecordCountWhereDynamic",
                filter.ToSqlParametersWithWhereClauseOnly(),
                CommandType.StoredProcedure
            );

            return dt.Rows.Count > 0 ? Convert.ToInt32(dt.Rows[0]["RecordCount"]) : 0;
        }

        public async Task<IEnumerable<Permission>> SelectSkipAndTakeWhereDynamicAsync(PermissionFilterDto filter)
        {
            filter.SortBy = SortFieldValidator.Validate(filter.SortBy, PermissionFilterDto.AllowedSortFields, "CreatedAt");
            filter.SortDirection = SortFieldValidator.ValidateDirection(filter.SortDirection);

            var dt = await _databaseHelper.GetDataTableAsync(
                _connectionString,
                "Permission_SelectSkipAndTakeWhereDynamic",
                filter.ToSqlParametersWithPagingAndSorting(),
                CommandType.StoredProcedure
            );

            return MapList(dt);
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            var dt = await _databaseHelper.GetDataTableAsync(
                _connectionString,
                "Permission_SelectByName",
                new List<SqlParameter> { new SqlParameter("@Name", name) },
                CommandType.StoredProcedure
            );

            return dt.Rows.Count > 0;
        }

        public async Task<Permission?> GetByNameAsync(string name)
        {
            var dt = await _databaseHelper.GetDataTableAsync(
                _connectionString,
                "Permission_SelectByName",
                new List<SqlParameter> { new SqlParameter("@Name", name) },
                CommandType.StoredProcedure
            );

            return dt.Rows.Count > 0 ? MapRow(dt.Rows[0]) : null;
        }

    }
}
