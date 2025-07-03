using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data.Helpers;
using Infrastructure.Database.Abstractions;
using Infrastructure.Database.Enums;
using Infrastructure.Database.Extensions;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Infrastructure.Repositories.Users
{
    public class UserVerificationRepository : BaseRepository<UserVerification>, IUserVerificationRepository
    {
        public UserVerificationRepository(IDatabaseHelper dbHelper, string connectionString)
            : base(dbHelper, connectionString) { }

        protected override string TableName => "UserVerification";
        protected override string PrimaryKeyParamName => "@UserVerificationId";

        protected override Func<DataRow, UserVerification> MapRow => row => new UserVerification
        {
            UserVerificationId = row.Get<int>("UserVerificationId"),
            UserId = row.Get<int>("UserId"),
            Token = row.GetString("Token")!,
            ExpiryDate = row.Get<DateTime>("ExpiryDate"),
            IsUsed = row.Get<bool>("IsUsed"),
            CreatedAt = row.Get<DateTime>("CreatedAt"),
            Field1 = row.GetString("Field1"),
            Field2 = row.GetString("Field2"),
            Field3 = row.GetString("Field3")
        };

        protected override List<SqlParameter> BuildInsertParams(UserVerification v) =>
            SqlHelper.FromDictionary(new()
            {
                ["@UserId"] = v.UserId,
                ["@Token"] = v.Token,
                ["@ExpiryDate"] = v.ExpiryDate,
                ["@Field1"] = v.Field1,
                ["@Field2"] = v.Field2,
                ["@Field3"] = v.Field3
            });

        protected override List<SqlParameter> BuildUpdateParams(UserVerification v)
        {
            throw new NotSupportedException("Update operation not supported for UserVerification.");
        }

        public async Task<UserVerification?> GetByTokenAsync(string token)
        {
            var parameters = SqlHelper.Single("@Token", token);
            var dt = await _databaseHelper.GetDataTableAsync(_connectionString, "UserVerification_SelectByToken", parameters, CommandType.StoredProcedure);
            return dt.Rows.Count == 0 ? null : MapRow(dt.Rows[0]);
        }

        public async Task MarkAsUsedAsync(int userVerificationId)
        {
            var parameters = SqlHelper.Single("@UserVerificationId", userVerificationId);
            await _databaseHelper.ExecuteSqlCommandAsync(_connectionString, "UserVerification_MarkAsUsed", parameters, CommandType.StoredProcedure, DatabaseOperationType.Update);
        }

        public async Task DeleteExpiredAsync()
        {
            await _databaseHelper.ExecuteSqlCommandAsync(_connectionString, "UserVerification_DeleteExpired", new(), CommandType.StoredProcedure, DatabaseOperationType.Delete);
        }
    }
}
