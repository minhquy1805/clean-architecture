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
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly IDatabaseHelper _databaseHelper;
        private readonly string _connectionString;

        public RefreshTokenRepository(IDatabaseHelper databaseHelper, string connectionString)
        {
            _databaseHelper = databaseHelper;
            _connectionString = connectionString;
        }

        public async Task InsertAsync(RefreshToken token)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@UserId", token.UserId),
                new SqlParameter("@Token", token.Token),
                new SqlParameter("@CreatedAt", token.CreatedAt),
                new SqlParameter("@ExpiryDate", token.ExpiryDate),
                new SqlParameter("@RevokedAt", token.RevokedAt ?? (object)DBNull.Value),
                new SqlParameter("@ReplacedByToken", token.ReplacedByToken ?? (object)DBNull.Value),
                new SqlParameter("@IPAddress", token.IPAddress ?? (object)DBNull.Value),
                new SqlParameter("@Flag", token.Flag),
                new SqlParameter("@Field1", token.Field1 ?? (object)DBNull.Value),
                new SqlParameter("@Field2", token.Field2 ?? (object)DBNull.Value),
                new SqlParameter("@Field3", token.Field3 ?? (object)DBNull.Value),
                new SqlParameter("@Field4", token.Field4 ?? (object)DBNull.Value),
                new SqlParameter("@Field5", token.Field5 ?? (object)DBNull.Value)
            };

            await _databaseHelper.ExecuteSqlCommandAsync(
                _connectionString,
                "RefreshToken_Insert",
                parameters,
                CommandType.StoredProcedure,
                DatabaseOperationType.Create);
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Token", token)
            };

            var dt = await _databaseHelper.GetDataTableAsync(
                _connectionString,
                "RefreshToken_SelectByToken",
                parameters,
                CommandType.StoredProcedure);

            if (dt.Rows.Count == 0)
                return null;

            return Map(dt.Rows[0]);
        }

        public async Task UpdateAsync(RefreshToken token)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@TokenId", token.TokenId),
                new SqlParameter("@UserId", token.UserId),
                new SqlParameter("@Token", token.Token),
                new SqlParameter("@CreatedAt", token.CreatedAt),
                new SqlParameter("@ExpiryDate", token.ExpiryDate),
                new SqlParameter("@RevokedAt", token.RevokedAt ?? (object)DBNull.Value),
                new SqlParameter("@ReplacedByToken", token.ReplacedByToken ?? (object)DBNull.Value),
                new SqlParameter("@IPAddress", token.IPAddress ?? (object)DBNull.Value),
                new SqlParameter("@Flag", token.Flag),
                new SqlParameter("@Field1", token.Field1 ?? (object)DBNull.Value),
                new SqlParameter("@Field2", token.Field2 ?? (object)DBNull.Value),
                new SqlParameter("@Field3", token.Field3 ?? (object)DBNull.Value),
                new SqlParameter("@Field4", token.Field4 ?? (object)DBNull.Value),
                new SqlParameter("@Field5", token.Field5 ?? (object)DBNull.Value)
            };

            await _databaseHelper.ExecuteSqlCommandAsync(
                _connectionString,
                "RefreshToken_Update",
                parameters,
                CommandType.StoredProcedure,
                DatabaseOperationType.Update);
        }

        #region Helper

        private static RefreshToken Map(DataRow row)
        {
            return new RefreshToken
            {
                TokenId = (int)row["TokenId"],
                UserId = (int)row["UserId"],
                Token = row["Token"].ToString()!,
                CreatedAt = (DateTime)row["CreatedAt"],
                ExpiryDate = (DateTime)row["ExpiryDate"],
                RevokedAt = row["RevokedAt"] != DBNull.Value ? (DateTime?)row["RevokedAt"] : null,
                ReplacedByToken = row["ReplacedByToken"]?.ToString(),
                IPAddress = row["IPAddress"]?.ToString(),
                Flag = row["Flag"].ToString()!,
                Field1 = row["Field1"]?.ToString(),
                Field2 = row["Field2"]?.ToString(),
                Field3 = row["Field3"]?.ToString(),
                Field4 = row["Field4"]?.ToString(),
                Field5 = row["Field5"]?.ToString()
            };
        }

        #endregion
    }
}
