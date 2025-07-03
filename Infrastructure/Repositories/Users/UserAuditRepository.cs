using Application.Common.Helpers;
using Application.DTOs.AuditLogs;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data.Helpers;
using Infrastructure.Database.Abstractions;
using Infrastructure.Database.Extensions;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Infrastructure.Repositories.Users
{
    public class UserAuditRepository : BaseRepository<UserAudit>, IUserAuditRepository
    {
        public UserAuditRepository(IDatabaseHelper dbHelper, string connectionString)
            : base(dbHelper, connectionString) { }

        protected override string TableName => "UserAudit";
        protected override string PrimaryKeyParamName => "@AuditId";

        protected override Func<DataRow, UserAudit> MapRow => row => new UserAudit
        {
            AuditId = row.Get<int>("AuditId"),
            UserId = row.Get<int>("UserId"),
            Action = row.GetString("Action")!,
            OldValue = row.GetString("OldValue"),
            NewValue = row.GetString("NewValue"),
            IpAddress = row.GetString("IpAddress"),
            Flag = row.GetString("Flag"),
            Field1 = row.GetString("Field1"),
            Field2 = row.GetString("Field2"),
            Field3 = row.GetString("Field3"),
            Field4 = row.GetString("Field4"),
            Field5 = row.GetString("Field5"),
            CreatedAt = row.Get<DateTime>("CreatedAt")
        };

        protected override List<SqlParameter> BuildInsertParams(UserAudit audit) =>
            SqlHelper.FromDictionary(new Dictionary<string, object?>
            {
                ["@UserId"] = audit.UserId,
                ["@Action"] = audit.Action,
                ["@OldValue"] = audit.OldValue,
                ["@NewValue"] = audit.NewValue,
                ["@IpAddress"] = audit.IpAddress,
                ["@Flag"] = audit.Flag,
                ["@Field1"] = audit.Field1,
                ["@Field2"] = audit.Field2,
                ["@Field3"] = audit.Field3,
                ["@Field4"] = audit.Field4,
                ["@Field5"] = audit.Field5
            });

        protected override List<SqlParameter> BuildUpdateParams(UserAudit entity)
        {
            throw new NotSupportedException("UserAudit does not support update operation.");
        }

        public async Task<IEnumerable<UserAudit>> GetByUserIdAsync(int userId)
        {
            var parameters = SqlHelper.Single("@UserId", userId);
            var dt = await _databaseHelper.GetDataTableAsync(
                _connectionString,
                "UserAudit_SelectByUserId",
                parameters,
                CommandType.StoredProcedure);
            return MapList(dt);
        }

        public async Task<IEnumerable<UserAudit>> SelectSkipAndTakeWhereDynamicAsync(AuditLogFilterDto filter)
        {
            filter.SortBy = SortFieldValidator.Validate(filter.SortBy, AuditLogFilterDto.AllowedSortFields, "CreatedAt");
            filter.SortDirection = SortFieldValidator.ValidateDirection(filter.SortDirection);

            var parameters = filter.ToSqlParametersWithPagingAndSorting();

            var dt = await _databaseHelper.GetDataTableAsync(
                _connectionString,
                "UserAudit_SelectSkipAndTakeWhereDynamic",
                parameters,
                CommandType.StoredProcedure);

            return MapList(dt);
        }

        public async Task<int> GetRecordCountWhereDynamicAsync(AuditLogFilterDto filter)
        {
            var parameters = filter.ToSqlParametersWithWhereClauseOnly();

            var dt = await _databaseHelper.GetDataTableAsync(
                _connectionString,
                "UserAudit_GetRecordCountWhereDynamic",
                parameters,
                CommandType.StoredProcedure);

            return Convert.ToInt32(dt.Rows[0]["RecordCount"]);
        }

    }
}
