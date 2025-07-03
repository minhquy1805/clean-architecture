using Infrastructure.Database.Enums;
using Microsoft.Data.SqlClient;
using System.Data;


namespace Infrastructure.Database.Abstractions
{
    public interface IDatabaseHelper
    {
        Task<DataTable> GetDataTableAsync(string connectionString, string sql, List<SqlParameter> parameters, CommandType commandType);
        Task<object?> ExecuteSqlCommandAsync(string connectionString, string sql, List<SqlParameter> parameters, CommandType commandType, DatabaseOperationType operationType, bool? isPrimaryKeyGuid = null);
        Task<int> GetRecordCountAsync(string connectionString, string sql, List<SqlParameter> parameters, CommandType commandType);
        void AddSqlParameter(List<SqlParameter> parameters, string name, object? value);
    }
}
