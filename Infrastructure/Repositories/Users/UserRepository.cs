using Application.Common.Helpers;
using Application.DTOs.Users.Filters;
using Application.Enums;
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
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(IDatabaseHelper databaseHelper, string connectionString)
            : base(databaseHelper, connectionString) { }

        protected override string TableName => "User";
        protected override string PrimaryKeyParamName => "@UserId";

        protected override Func<DataRow, User> MapRow => row => new User
        {
            UserId = row.Get<int>("UserId"),
            FullName = row.GetString("FullName")!,
            Email = row.GetString("Email")!,
            PasswordHash = row.GetString("PasswordHash")!,
            Role = row.GetString("Role")!,
            CreatedAt = row.Get<DateTime>("CreatedAt"),
            UpdatedAt = row.GetNullable<DateTime>("UpdatedAt"),
            LastLoginAt = row.GetNullable<DateTime>("LastLoginAt"),
            PhoneNumber = row.GetString("PhoneNumber"),
            DateOfBirth = row.GetNullable<DateTime>("DateOfBirth"),
            Gender = row.GetString("Gender"),
            AvatarUrl = row.GetString("AvatarUrl"),
            Field1 = row.GetString("Field1"),
            Field2 = row.GetString("Field2"),
            Field3 = row.GetString("Field3"),
            Field4 = row.GetString("Field4"),
            Field5 = row.GetString("Field5"),
            Flag = row.GetString("Flag")!,
            IsActive = row.Get<bool>("IsActive")
        };

        private static User MapRowForDropdown(DataRow row) => new User
        {
            UserId = row.Get<int>("UserId"),
            FullName = row.GetString("FullName")!
        };

        protected override List<SqlParameter> BuildInsertParams(User user)
            => user.ToSqlParameters(SqlOperationType.Create);

        protected override List<SqlParameter> BuildUpdateParams(User user)
            => user.ToSqlParameters(SqlOperationType.Update);

        // Các phương thức đặc biệt ngoài CRUD

        public async Task<User?> GetByEmailAsync(string email)
        {
            var parameters = SqlHelper.Single("@Email", email);
            var dt = await _databaseHelper.GetDataTableAsync(_connectionString, "User_SelectByEmail", parameters, CommandType.StoredProcedure);
            return dt.Rows.Count == 0 ? null : MapRow(dt.Rows[0]);
        }

        public async Task UpdateIsActiveAsync(int userId, bool isActive)
        {
            var parameters = SqlHelper.FromDictionary(new()
            {
                ["@UserId"] = userId,
                ["@IsActive"] = isActive
            });

            await _databaseHelper.ExecuteSqlCommandAsync(
                _connectionString,
                "User_UpdateIsActive",
                parameters,
                CommandType.StoredProcedure,
                DatabaseOperationType.Update
            );
        }

        public async Task<IEnumerable<User>> GetAllWhereDynamicAsync(UserFilterDto filter)
        {
            var parameters = filter.ToSqlParametersWithWhereClauseOnly();

            var dt = await _databaseHelper.GetDataTableAsync(
                _connectionString,
                "User_SelectAllWhereDynamic", // Store này phải nhận @WhereCondition
                parameters,
                CommandType.StoredProcedure
            );

            return MapList(dt);
        }


        public async Task<IEnumerable<User>> SelectSkipAndTakeWhereDynamicAsync(UserFilterDto filter)
        {
            filter.SortBy = SortFieldValidator.Validate(filter.SortBy, UserFilterDto.AllowedSortFields, "CreatedAt");
            filter.SortDirection = SortFieldValidator.ValidateDirection(filter.SortDirection);

            var dt = await _databaseHelper.GetDataTableAsync(
                _connectionString,
                "User_SelectSkipAndTakeWhereDynamic",
                filter.ToSqlParametersWithPagingAndSorting(),
                CommandType.StoredProcedure
            );
            return MapList(dt);
        }

        public async Task<int> GetRecordCountWhereDynamicAsync(UserFilterDto filter)
        {
            var dt = await _databaseHelper.GetDataTableAsync(
                _connectionString,
                "User_GetRecordCountWhereDynamic",
                filter.ToSqlParametersWithWhereClauseOnly(),
                CommandType.StoredProcedure
            );

            return Convert.ToInt32(dt.Rows[0]["RecordCount"]);
        }

        public async Task<int> GetRecordCountAsync()
        {
            return await _databaseHelper.GetRecordCountAsync(_connectionString, "User_GetRecordCount", new(), CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<User>> GetDropDownListDataAsync()
        {
            var dt = await _databaseHelper.GetDataTableAsync(
                _connectionString,
                "User_SelectDropDownListData",
                new(),
                CommandType.StoredProcedure
            );

            return dt.AsEnumerable().Select(MapRowForDropdown).ToList();
        }

        public async Task<IEnumerable<User>> SelectSkipAndTakeAsync(int start, int rows, string sortBy)
        {
            var parameters = SqlHelper.FromDictionary(new()
            {
                ["@Start"] = start,
                ["@NumberOfRows"] = rows,
                ["@SortBy"] = sortBy,
                ["@SortDirection"] = "DESC"
            });

            var dt = await _databaseHelper.GetDataTableAsync(
                _connectionString,
                "User_SelectSkipAndTake",
                parameters,
                CommandType.StoredProcedure
            );

            return MapList(dt);
        }
    }
}
