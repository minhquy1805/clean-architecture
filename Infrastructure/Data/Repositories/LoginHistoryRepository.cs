using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Abstractions;
using Infrastructure.Abstractions.Enums;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Infrastructure.Data.Repositories
{
    public class LoginHistoryRepository : ILoginHistoryRepository
    {
        private readonly IDatabaseHelper _databaseHelper;
        private readonly string _connectionString;

        public LoginHistoryRepository(IDatabaseHelper dbHelper, string connectionString)
        {
            _databaseHelper = dbHelper;
            _connectionString = connectionString;
        }

        public async Task InsertAsync(LoginHistory log)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@UserId", log.UserId),
                new SqlParameter("@IsSuccess", log.IsSuccess),
                new SqlParameter("@IpAddress", log.IpAddress ?? (object)DBNull.Value),
                new SqlParameter("@UserAgent", log.UserAgent ?? (object)DBNull.Value),
                new SqlParameter("@Message", log.Message ?? (object)DBNull.Value)
            };

            await _databaseHelper.ExecuteSqlCommandAsync(
                _connectionString,
                "LoginHistory_Insert",
                parameters,
                CommandType.StoredProcedure,
                DatabaseOperationType.Create
            );
        }

        public async Task<IEnumerable<LoginHistory>> GetByUserIdAsync(int userId)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@UserId", userId)
            };

            var dt = await _databaseHelper.GetDataTableAsync(
                _connectionString,
                "LoginHistory_SelectByUserId",
                parameters,
                CommandType.StoredProcedure);

            return MapList(dt);
        }

        public async Task<IEnumerable<LoginHistory>> GetPagingAsync(int? userId, int start, int numberOfRows)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@UserId", userId ?? (object)DBNull.Value),
                new SqlParameter("@Start", start),
                new SqlParameter("@NumberOfRows", numberOfRows)
            };

            var dt = await _databaseHelper.GetDataTableAsync(
                _connectionString,
                "LoginHistory_SelectSkipAndTake",
                parameters,
                CommandType.StoredProcedure);

            return MapList(dt);
        }

        // ✅ Helper: Map DataTable to List<LoginHistory>
        private static List<LoginHistory> MapList(DataTable dt)
        {
            var list = new List<LoginHistory>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(Map(row));
            }
            return list;
        }

        // ✅ Helper: Map DataRow to LoginHistory
        private static LoginHistory Map(DataRow row)
        {
            return new LoginHistory
            {
                LoginId = (int)row["LoginId"],
                UserId = (int)row["UserId"],
                IsSuccess = (bool)row["IsSuccess"],
                IpAddress = row["IpAddress"]?.ToString(),
                UserAgent = row["UserAgent"]?.ToString(),
                Message = row["Message"]?.ToString(),
                CreatedAt = (DateTime)row["CreatedAt"]
            };
        }
    }
}
