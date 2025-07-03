using Application.Interfaces.Common;
using Infrastructure.Data.Helpers;
using Infrastructure.Database.Abstractions;
using Infrastructure.Database.Enums;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Infrastructure.Repositories
{
    public abstract class BaseRepository<T> : IBaseRepository<T>
    {
        protected readonly IDatabaseHelper _databaseHelper;
        protected readonly string _connectionString;

        protected BaseRepository(IDatabaseHelper databaseHelper, string connectionString)
        {
            _databaseHelper = databaseHelper;
            _connectionString = connectionString;
        }

        protected abstract string TableName { get; }
        protected abstract Func<DataRow, T> MapRow { get; }
        protected abstract string PrimaryKeyParamName { get; }
        protected abstract List<SqlParameter> BuildInsertParams(T entity);
        protected abstract List<SqlParameter> BuildUpdateParams(T entity);

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            var dt = await _databaseHelper.GetDataTableAsync(_connectionString, $"{TableName}_SelectAll", new(), CommandType.StoredProcedure);
            return MapList(dt);
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            var parameters = SqlHelper.Single(PrimaryKeyParamName, id);
            var dt = await _databaseHelper.GetDataTableAsync(_connectionString, $"{TableName}_SelectById", parameters, CommandType.StoredProcedure);
            return dt.Rows.Count == 0 ? default : MapRow(dt.Rows[0]);
        }

        public virtual async Task<int> InsertAsync(T entity)
        {
            var parameters = BuildInsertParams(entity);
            var id = await _databaseHelper.ExecuteSqlCommandAsync(
                _connectionString,
                $"{TableName}_Insert",
                parameters,
                CommandType.StoredProcedure,
                DatabaseOperationType.Create,
                isPrimaryKeyGuid: false
            );
            return Convert.ToInt32(id);
        }

        public virtual async Task UpdateAsync(T entity)
        {
            var parameters = BuildUpdateParams(entity);
            await _databaseHelper.ExecuteSqlCommandAsync(
                _connectionString,
                $"{TableName}_Update",
                parameters,
                CommandType.StoredProcedure,
                DatabaseOperationType.Update
            );
        }

        public virtual async Task DeleteAsync(int id)
        {
            var parameters = SqlHelper.Single("@Id", id);
            await _databaseHelper.ExecuteSqlCommandAsync(
                _connectionString,
                $"{TableName}_Delete",
                parameters,
                CommandType.StoredProcedure,
                DatabaseOperationType.Delete
            );
        }

        protected List<T> MapList(DataTable dt)
        {
            var list = new List<T>();
            foreach (DataRow row in dt.Rows)
                list.Add(MapRow(row));
            return list;
        }
    }
}
