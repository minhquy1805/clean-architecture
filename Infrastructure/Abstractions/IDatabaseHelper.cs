using Infrastructure.Abstractions.Enums;
using Microsoft.Data.SqlClient;
using System.Data;


namespace Infrastructure.Abstractions
{
    public interface IDatabaseHelper
    {
        Task<DataTable> GetDataTableAsync(string connectionString, string sql, List<SqlParameter> parameters, CommandType commandType);
        Task<object?> ExecuteSqlCommandAsync(string connectionString, string sql, List<SqlParameter> parameters, CommandType commandType, DatabaseOperationType operationType, bool? isPrimaryKeyGuid = null);
        Task<int> GetRecordCountAsync(string connectionString, string sql, List<SqlParameter> parameters, CommandType commandType);
        void AddSelectSkipAndTakeParams(List<SqlParameter> parameters, string sortByExpression, int start, int rows);
        void AddSqlParameter(List<SqlParameter> parameters, string name, object? value);
    }
}
