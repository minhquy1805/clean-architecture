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
    public class UserVerificationRepository : IUserVerificationRepository
    {
        private readonly IDatabaseHelper _dbHelper;
        private readonly string _connectionString;

        public UserVerificationRepository(IDatabaseHelper dbHelper, string connectionString)
        {
            _dbHelper = dbHelper;
            _connectionString = connectionString;
        }

        public async Task<int> InsertAsync(UserVerification verification)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@UserId", verification.UserId),
                new SqlParameter("@Token", verification.Token),
                new SqlParameter("@ExpiryDate", verification.ExpiryDate),
                new SqlParameter("@Field1", (object?)verification.Field1 ?? DBNull.Value),
                new SqlParameter("@Field2", (object?)verification.Field2 ?? DBNull.Value),
                new SqlParameter("@Field3", (object?)verification.Field3 ?? DBNull.Value)
            };

            var id = await _dbHelper.ExecuteSqlCommandAsync(
                _connectionString,
                "UserVerification_Insert",
                parameters,
                CommandType.StoredProcedure,
                DatabaseOperationType.Create
            );

            return Convert.ToInt32(id);
        }

        public async Task<UserVerification?> GetByTokenAsync(string token)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Token", token)
            };

            var dt = await _dbHelper.GetDataTableAsync(
                _connectionString,
                "UserVerification_SelectByToken",
                parameters,
                CommandType.StoredProcedure
            );

            if (dt.Rows.Count == 0) return null;

            var row = dt.Rows[0];

            return new UserVerification
            {
                UserVerificationId = (int)row["UserVerificationId"],
                UserId = (int)row["UserId"],
                Token = row["Token"].ToString()!,
                ExpiryDate = (DateTime)row["ExpiryDate"],
                IsUsed = (bool)row["IsUsed"],
                CreatedAt = (DateTime)row["CreatedAt"],
                Field1 = row["Field1"]?.ToString(),
                Field2 = row["Field2"]?.ToString(),
                Field3 = row["Field3"]?.ToString()
            };
        }

        public async Task MarkAsUsedAsync(int userVerificationId)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@UserVerificationId", userVerificationId)
            };

            await _dbHelper.ExecuteSqlCommandAsync(
                _connectionString,
                "UserVerification_MarkAsUsed",
                parameters,
                CommandType.StoredProcedure,
                DatabaseOperationType.Update
            );
        }

        public async Task DeleteExpiredAsync()
        {
            await _dbHelper.ExecuteSqlCommandAsync(
                _connectionString,
                "UserVerification_DeleteExpired",
                new(),
                CommandType.StoredProcedure,
                DatabaseOperationType.Delete
            );
        }
    }
}
