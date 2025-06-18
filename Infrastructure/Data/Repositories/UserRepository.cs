using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Abstractions;
using Infrastructure.Abstractions.Enums;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;



namespace Infrastructure.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDatabaseHelper _databaseHelper;
        private readonly string _connectionString;

        public UserRepository(IDatabaseHelper dbHelper, string connectionString)
        {
            _databaseHelper = dbHelper;
            _connectionString = connectionString;
        }

        #region CRUD

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            var dt = await _databaseHelper.GetDataTableAsync(_connectionString, "User_SelectAll", new(), CommandType.StoredProcedure);
            return MapList(dt);
        }

        public async Task<IEnumerable<User>> GetAllWhereDynamicAsync(string whereCondition)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@WhereCondition", whereCondition)
            };
            var dt = await _databaseHelper.GetDataTableAsync(_connectionString, "User_SelectAllWhereDynamic", parameters, CommandType.StoredProcedure);
            return MapList(dt);
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@UserId", id)
            };
            var dt = await _databaseHelper.GetDataTableAsync(_connectionString, "User_SelectById", parameters, CommandType.StoredProcedure);
            return dt.Rows.Count == 0 ? null : Map(dt.Rows[0]);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Email", email)
            };
            var dt = await _databaseHelper.GetDataTableAsync(_connectionString, "User_SelectByEmail", parameters, CommandType.StoredProcedure);
            return dt.Rows.Count == 0 ? null : Map(dt.Rows[0]);
        }

        public async Task<int> InsertAsync(User user)
        {
            var parameters = BuildSqlParametersForInsert(user);
            var id = await _databaseHelper.ExecuteSqlCommandAsync(
                _connectionString,
                "User_Insert",
                parameters,
                CommandType.StoredProcedure,
                DatabaseOperationType.Create,
                isPrimaryKeyGuid: false // 👈 BẮT BUỘC!
            );
            return Convert.ToInt32(id);
        }

        public async Task UpdateAsync(User user)
        {
            var parameters = BuildSqlParametersForUpdate(user);
            await _databaseHelper.ExecuteSqlCommandAsync(
                _connectionString,
                "User_Update",
                parameters,
                CommandType.StoredProcedure,
                DatabaseOperationType.Update
            );
        }

        public async Task DeleteAsync(int id)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@UserId", id)
            };
            await _databaseHelper.ExecuteSqlCommandAsync(
                _connectionString,
                "User_Delete",
                parameters,
                CommandType.StoredProcedure,
                DatabaseOperationType.Delete
            );
        }

        #endregion

        #region Paging & RecordCount

        public async Task<IEnumerable<User>> SelectSkipAndTakeAsync(int start, int rows, string sortBy)
        {
            var parameters = new List<SqlParameter>();
            _databaseHelper.AddSelectSkipAndTakeParams(parameters, sortBy, start, rows);

            var dt = await _databaseHelper.GetDataTableAsync(_connectionString, "User_SelectSkipAndTake", parameters, CommandType.StoredProcedure);
            return MapList(dt);
        }

        public async Task<IEnumerable<User>> SelectSkipAndTakeWhereDynamicAsync(string whereCondition, int start, int rows, string sortBy)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@WhereCondition", whereCondition)
            };
            _databaseHelper.AddSelectSkipAndTakeParams(parameters, sortBy, start, rows);

            var dt = await _databaseHelper.GetDataTableAsync(_connectionString, "User_SelectSkipAndTakeWhereDynamic", parameters, CommandType.StoredProcedure);
            return MapList(dt);
        }

        public async Task<int> GetRecordCountAsync()
        {
            return await _databaseHelper.GetRecordCountAsync(_connectionString, "User_GetRecordCount", new(), CommandType.StoredProcedure);
        }

        public async Task<int> GetRecordCountWhereDynamicAsync(string whereCondition)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@WhereCondition", whereCondition)
            };
            return await _databaseHelper.GetRecordCountAsync(_connectionString, "User_GetRecordCountWhereDynamic", parameters, CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<User>> GetDropDownListDataAsync()
        {
            var dt = await _databaseHelper.GetDataTableAsync(_connectionString, "User_SelectDropDownListData", new(), CommandType.StoredProcedure);
            return MapList(dt);
        }

        #endregion

        #region Helpers

        private static User Map(DataRow row) 
        {
            return new User
            {
                UserId = (int)row["UserId"],
                FullName = row["FullName"].ToString()!,
                Email = row["Email"].ToString()!,
                PasswordHash = row["PasswordHash"].ToString()!,
                Role = row["Role"].ToString()!,
                CreatedAt = (DateTime)row["CreatedAt"],
                UpdatedAt = row["UpdatedAt"] != DBNull.Value ? (DateTime?)row["UpdatedAt"] : null,
                LastLoginAt = row["LastLoginAt"] != DBNull.Value ? (DateTime?)row["LastLoginAt"] : null,
                PhoneNumber = row["PhoneNumber"]?.ToString(),
                DateOfBirth = row["DateOfBirth"] != DBNull.Value ? (DateTime?)row["DateOfBirth"] : null,
                Gender = row["Gender"]?.ToString(),
                AvatarUrl = row["AvatarUrl"]?.ToString(),
                Field1 = row["Field1"]?.ToString(),
                Field2 = row["Field2"]?.ToString(),
                Field3 = row["Field3"]?.ToString(),
                Field4 = row["Field4"]?.ToString(),
                Field5 = row["Field5"]?.ToString(),
                Flag = row["Flag"].ToString()!,
                IsActive = row["IsActive"] != DBNull.Value && (bool)row["IsActive"]
            };
        }

        private static List<User> MapList(DataTable dt)
        {
            var list = new List<User>();
            foreach (DataRow row in dt.Rows)
                list.Add(Map(row));
            return list;
        }

        private static List<SqlParameter> BuildSqlParametersForInsert(User user)
        {
            return new List<SqlParameter>
            {
                new SqlParameter("@FullName", user.FullName),
                new SqlParameter("@Email", user.Email),
                new SqlParameter("@PasswordHash", user.PasswordHash),
                new SqlParameter("@Role", user.Role ?? "User"),
                new SqlParameter("@AvatarUrl", user.AvatarUrl ?? (object)DBNull.Value),
                new SqlParameter("@PhoneNumber", user.PhoneNumber ?? (object)DBNull.Value),
                new SqlParameter("@DateOfBirth", user.DateOfBirth ?? (object)DBNull.Value),
                new SqlParameter("@Gender", user.Gender ?? (object)DBNull.Value),
                new SqlParameter("@IsActive", user.IsActive),
                new SqlParameter("@Field1", user.Field1 ?? (object)DBNull.Value),
                new SqlParameter("@Field2", user.Field2 ?? (object)DBNull.Value),
                new SqlParameter("@Field3", user.Field3 ?? (object)DBNull.Value),
                new SqlParameter("@Field4", user.Field4 ?? (object)DBNull.Value),
                new SqlParameter("@Field5", user.Field5 ?? (object)DBNull.Value),
                new SqlParameter("@Flag", user.Flag ?? (object)DBNull.Value)
            };
        }

        private static List<SqlParameter> BuildSqlParametersForUpdate(User user)
        {
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@UserId", user.UserId),
                new SqlParameter("@FullName", user.FullName),
                new SqlParameter("@Email", user.Email),
                new SqlParameter("@PasswordHash", user.PasswordHash),
                new SqlParameter("@Role", user.Role ?? "User"),
                new SqlParameter("@AvatarUrl", user.AvatarUrl ?? (object)DBNull.Value),
                new SqlParameter("@PhoneNumber", user.PhoneNumber ?? (object)DBNull.Value),
                new SqlParameter("@DateOfBirth", user.DateOfBirth ?? (object)DBNull.Value),
                new SqlParameter("@Gender", user.Gender ?? (object)DBNull.Value),
                new SqlParameter("@IsActive", user.IsActive),
                new SqlParameter("@Field1", user.Field1 ?? (object)DBNull.Value),
                new SqlParameter("@Field2", user.Field2 ?? (object)DBNull.Value),
                new SqlParameter("@Field3", user.Field3 ?? (object)DBNull.Value),
                new SqlParameter("@Field4", user.Field4 ?? (object)DBNull.Value),
                new SqlParameter("@Field5", user.Field5 ?? (object)DBNull.Value),
                new SqlParameter("@Flag", user.Flag ?? (object)DBNull.Value)
            };

            return parameters;
        }
        #endregion
    }
}
