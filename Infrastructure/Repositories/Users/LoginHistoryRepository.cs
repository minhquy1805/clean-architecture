using Application.DTOs.LoginHistories;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data.Helpers;
using Infrastructure.Database.Abstractions;
using Infrastructure.Database.Extensions;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Infrastructure.Repositories.Users
{
    public class LoginHistoryRepository : BaseRepository<LoginHistory>, ILoginHistoryRepository
    {
        public LoginHistoryRepository(IDatabaseHelper dbHelper, string connectionString)
            : base(dbHelper, connectionString) { }

        protected override string TableName => "LoginHistory";
        protected override string PrimaryKeyParamName => "@LoginId";

        protected override Func<DataRow, LoginHistory> MapRow => row => new LoginHistory
        {
            LoginId = row.Get<int>("LoginId"),
            UserId = row.Get<int>("UserId"),
            IsSuccess = row.Get<bool>("IsSuccess"),
            IpAddress = row.GetString("IpAddress"),
            UserAgent = row.GetString("UserAgent"),
            Message = row.GetString("Message"),
            CreatedAt = row.Get<DateTime>("CreatedAt")
        };

        protected override List<SqlParameter> BuildInsertParams(LoginHistory log) =>
            SqlHelper.FromDictionary(new()
            {
                ["@UserId"] = log.UserId,
                ["@IsSuccess"] = log.IsSuccess,
                ["@IpAddress"] = log.IpAddress,
                ["@UserAgent"] = log.UserAgent,
                ["@Message"] = log.Message
            });

        protected override List<SqlParameter> BuildUpdateParams(LoginHistory entity) =>
            throw new NotSupportedException("LoginHistory does not support update operation.");

        // ✅ Vẫn giữ phương thức đơn giản
        public async Task<IEnumerable<LoginHistory>> GetByUserIdAsync(int userId)
        {
            var dt = await _databaseHelper.GetDataTableAsync(
                _connectionString,
                "LoginHistory_SelectByUserId",
                SqlHelper.Single("@UserId", userId),
                CommandType.StoredProcedure);
            return MapList(dt);
        }

        // ✅ MỚI: dùng DTO cho filter động + paging
        public async Task<IEnumerable<LoginHistory>> SelectSkipAndTakeWhereDynamicAsync(LoginHistoryFilterDto filter)
        {
            var dt = await _databaseHelper.GetDataTableAsync(
                _connectionString,
                "LoginHistory_SelectSkipAndTakeWhereDynamic",
                filter.ToSqlParametersWithPagingAndSorting(),
                CommandType.StoredProcedure);
            return MapList(dt);
        }

        public async Task<int> GetRecordCountWhereDynamicAsync(LoginHistoryFilterDto filter)
        {
            var dt = await _databaseHelper.GetDataTableAsync(
                _connectionString,
                "LoginHistory_GetRecordCountWhereDynamic",
                filter.ToSqlParametersWithWhereClauseOnly(),
                CommandType.StoredProcedure);
            return Convert.ToInt32(dt.Rows[0]["RecordCount"]);
        }
    }
}
