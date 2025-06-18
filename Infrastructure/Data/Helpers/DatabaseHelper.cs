using Infrastructure.Abstractions;
using Infrastructure.Abstractions.Enums;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.Helpers
{
    public class DatabaseHelper : IDatabaseHelper
    {
        private MsSqlDatabase GetMsSqlDatabase(string connectionString, string sql, List<SqlParameter> parameters, CommandType commandType)
        {
            if (commandType == CommandType.TableDirect)
                throw new ArgumentException("Only StoredProcedure or Text allowed.", nameof(commandType));

            var connection = new SqlConnection(connectionString);
            var command = new SqlCommand(sql, connection)
            {
                CommandType = commandType
            };

            foreach (var param in parameters)
            {
                command.Parameters.AddWithValue(param.ParameterName, param.Value ?? DBNull.Value);
            }

            var adapter = new SqlDataAdapter(command);
            return new MsSqlDatabase(connection, command, adapter);
        }

        public async Task<DataTable> GetDataTableAsync(string connectionString, string sql, List<SqlParameter> parameters, CommandType commandType)
        {
            var db = GetMsSqlDatabase(connectionString, sql, parameters, commandType);
            var dt = new DataTable();

            using (db.SqlConnection)
            {
                await db.SqlConnection.OpenAsync().ConfigureAwait(false);
                using (db.SqlCommand)
                using (db.SqlDataAdapter)
                {
                    db.SqlDataAdapter.Fill(dt);
                }
            }
            return dt;
        }
        public async Task<object?> ExecuteSqlCommandAsync(string connectionString, string sql, List<SqlParameter> parameters, CommandType commandType, DatabaseOperationType operationType, bool? isPrimaryKeyGuid = null)
        {
            if (commandType == CommandType.TableDirect)
                throw new ArgumentException("Invalid CommandType.", nameof(commandType));

            if (operationType == DatabaseOperationType.RetrieveDataTable)
                throw new ArgumentException("Cannot use RetrieveDataTable for ExecuteSqlCommand.");

            if (operationType == DatabaseOperationType.Create && isPrimaryKeyGuid == null)
                throw new ArgumentException("isPrimaryKeyGuid must be specified for Create.");

            var db = GetMsSqlDatabase(connectionString, sql, parameters, commandType);

            using (db.SqlConnection)
            {
                await db.SqlConnection.OpenAsync().ConfigureAwait(false);

                using (db.SqlCommand)
                {
                    if (operationType == DatabaseOperationType.Update || operationType == DatabaseOperationType.Delete)
                    {
                        await db.SqlCommand.ExecuteNonQueryAsync().ConfigureAwait(false);
                        return null;
                    }

                    if (isPrimaryKeyGuid!.Value)
                        return db.SqlCommand.ExecuteScalar()?.ToString();

                    return await db.SqlCommand.ExecuteScalarAsync().ConfigureAwait(false);
                }
            }
        }

        public async Task<int> GetRecordCountAsync(string connectionString, string sql, List<SqlParameter> parameters, CommandType commandType)
        {
            var dt = await GetDataTableAsync(connectionString, sql, parameters, commandType).ConfigureAwait(false);
            return dt.Rows.Count > 0 ? Convert.ToInt32(dt.Rows[0]["RecordCount"]) : 0;
        }

        public void AddSelectSkipAndTakeParams(List<SqlParameter> parameters, string sortByExpression, int start, int rows)
        {
            AddSqlParameter(parameters, "@start", start);
            AddSqlParameter(parameters, "@numberOfRows", rows);
            AddSqlParameter(parameters, "@sortByExpression", sortByExpression);
        }

        public void AddSqlParameter(List<SqlParameter> parameters, string name, object? value)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                parameters.Add(new SqlParameter(name, value ?? DBNull.Value));
            }
        }
    }
}
