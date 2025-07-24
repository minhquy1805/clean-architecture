using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data.Helpers;
using Infrastructure.Database.Abstractions;
using Infrastructure.Database.Enums;
using Infrastructure.Database.Extensions;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Infrastructure.Repositories.SQL.Users
{
    public class SqlRefreshTokenRepository : BaseRepository<RefreshToken>, IRefreshTokenRepository
    {
        public SqlRefreshTokenRepository(IDatabaseHelper dbHelper, string connectionString)
            : base(dbHelper, connectionString) { }

        protected override string TableName => "RefreshToken";
        protected override string PrimaryKeyParamName => "@TokenId";

        protected override Func<DataRow, RefreshToken> MapRow => row => new RefreshToken
        {
            TokenId = row.Get<int>("TokenId"),
            UserId = row.Get<int>("UserId"),
            Token = row.GetString("Token")!,
            CreatedAt = row.Get<DateTime>("CreatedAt"),
            ExpiryDate = row.Get<DateTime>("ExpiryDate"),
            RevokedAt = row.GetNullable<DateTime>("RevokedAt"),
            ReplacedByToken = row.GetString("ReplacedByToken"),
            IPAddress = row.GetString("IPAddress"),
            UserAgent = row.GetString("UserAgent"),
            Flag = row.GetString("Flag")!,
            Field1 = row.GetString("Field1"),
            Field2 = row.GetString("Field2"),
            Field3 = row.GetString("Field3"),
            Field4 = row.GetString("Field4"),
            Field5 = row.GetString("Field5")
        };

        protected override List<SqlParameter> BuildInsertParams(RefreshToken token)
        {
            return SqlHelper.FromDictionary(new()
            {
                ["@UserId"] = token.UserId,
                ["@Token"] = token.Token,
                ["@ExpiryDate"] = token.ExpiryDate,
                ["@RevokedAt"] = token.RevokedAt,
                ["@ReplacedByToken"] = token.ReplacedByToken,
                ["@IPAddress"] = token.IPAddress,
                ["@UserAgent"] = token.UserAgent,
                ["@Flag"] = token.Flag ?? "T",
                ["@Field1"] = token.Field1,
                ["@Field2"] = token.Field2,
                ["@Field3"] = token.Field3,
                ["@Field4"] = token.Field4,
                ["@Field5"] = token.Field5
            });
        }

        protected override List<SqlParameter> BuildUpdateParams(RefreshToken token)
        {
            return SqlHelper.FromDictionary(new()
            {
                ["@TokenId"] = token.TokenId,
                ["@UserId"] = token.UserId,
                ["@Token"] = token.Token,
                ["@ExpiryDate"] = token.ExpiryDate,
                ["@RevokedAt"] = token.RevokedAt,
                ["@ReplacedByToken"] = token.ReplacedByToken,
                ["@IPAddress"] = token.IPAddress,
                ["@UserAgent"] = token.UserAgent,
                ["@Flag"] = token.Flag,
                ["@Field1"] = token.Field1,
                ["@Field2"] = token.Field2,
                ["@Field3"] = token.Field3,
                ["@Field4"] = token.Field4,
                ["@Field5"] = token.Field5
            });
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            var parameters = SqlHelper.Single("@Token", token);
            var dt = await _databaseHelper.GetDataTableAsync(
                _connectionString,
                "RefreshToken_SelectByToken",
                parameters,
                CommandType.StoredProcedure);

            return dt.Rows.Count == 0 ? null : MapRow(dt.Rows[0]);
        }

        public async Task DeleteByUserIdAsync(int userId)
        {
            var parameters = SqlHelper.Single("@UserId", userId);
            await _databaseHelper.ExecuteSqlCommandAsync(
                _connectionString,
                "RefreshToken_DeleteByUserId",
                parameters,
                CommandType.StoredProcedure,
                DatabaseOperationType.Delete);
        }
    }
}
