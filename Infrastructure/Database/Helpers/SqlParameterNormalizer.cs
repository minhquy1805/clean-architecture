using Microsoft.Data.SqlClient;
using System.Data.SqlTypes;

namespace Infrastructure.Database.Helpers
{
    public static class SqlParameterNormalizer
    {
        public static List<SqlParameter> Normalize(List<SqlParameter> parameters)
        {
            foreach (var param in parameters)
            {
                if (param.Value is DateTime dt && dt < (DateTime)SqlDateTime.MinValue)
                {
                    param.Value = DBNull.Value;
                }
            }
            return parameters;
        }
    }
}
