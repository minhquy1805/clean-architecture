using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Abstractions;
using Infrastructure.Abstractions.Enums;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.Repositories
{
    public class UserAuditRepository: IUserAuditRepository
    {
        private readonly IDatabaseHelper _databaseHelper;
        private readonly string _connectionString;

        public UserAuditRepository(IDatabaseHelper dbHelper, string connectionString)
        {
            _databaseHelper = dbHelper;
            _connectionString = connectionString;
        }

        public async Task InsertAsync(UserAudit audit)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@UserId", audit.UserId),
                new SqlParameter("@Action", audit.Action),
                new SqlParameter("@OldValue", audit.OldValue ?? (object)DBNull.Value),
                new SqlParameter("@NewValue", audit.NewValue ?? (object)DBNull.Value),
                new SqlParameter("@IpAddress", audit.IpAddress ?? (object)DBNull.Value),
                new SqlParameter("@Flag", audit.Flag ?? (object)DBNull.Value),
                new SqlParameter("@Field1", audit.Field1 ?? (object)DBNull.Value),
                new SqlParameter("@Field2", audit.Field2 ?? (object)DBNull.Value),
                new SqlParameter("@Field3", audit.Field3 ?? (object)DBNull.Value),
                new SqlParameter("@Field4", audit.Field4 ?? (object)DBNull.Value),
                new SqlParameter("@Field5", audit.Field5 ?? (object)DBNull.Value)
            };

            await _databaseHelper.ExecuteSqlCommandAsync(
                _connectionString,
                "UserAudit_Insert",
                parameters,
                CommandType.StoredProcedure,
                DatabaseOperationType.Create
            );
        }

        public async Task<IEnumerable<UserAudit>> GetByUserIdAsync(int userId)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@UserId", userId)
            };

            var dt = await _databaseHelper.GetDataTableAsync(
                _connectionString,
                "UserAudit_SelectByUserId",
                parameters,
                CommandType.StoredProcedure);

            return MapList(dt);
        }

        public async Task<IEnumerable<UserAudit>> GetPagingAsync(int? userId, int start, int numberOfRows)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@UserId", userId ?? (object)DBNull.Value),
                new SqlParameter("@Start", start),
                new SqlParameter("@NumberOfRows", numberOfRows)
            };

            var dt = await _databaseHelper.GetDataTableAsync(
                _connectionString,
                "UserAudit_SelectSkipAndTake",
                parameters,
                CommandType.StoredProcedure);

            return MapList(dt);
        }
        private static List<UserAudit> MapList(DataTable dt)
        {
            var list = new List<UserAudit>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new UserAudit
                {
                    AuditId = (int)row["AuditId"],
                    UserId = (int)row["UserId"],
                    Action = row["Action"].ToString()!,
                    OldValue = row["OldValue"]?.ToString(),
                    NewValue = row["NewValue"]?.ToString(),
                    IpAddress = row["IpAddress"]?.ToString(),
                    Flag = row["Flag"]?.ToString(),
                    Field1 = row["Field1"]?.ToString(),
                    Field2 = row["Field2"]?.ToString(),
                    Field3 = row["Field3"]?.ToString(),
                    Field4 = row["Field4"]?.ToString(),
                    Field5 = row["Field5"]?.ToString(),
                    CreatedAt = (DateTime)row["CreatedAt"]
                });
            }
            return list;
        }
    }
}
