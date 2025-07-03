using Microsoft.Data.SqlClient;
using System.Data;

namespace Infrastructure.Data.Helpers
{
    /// <summary>
    /// Cung cấp các phương thức tiện ích để tạo danh sách SqlParameter một cách nhanh chóng và ngắn gọn.
    /// </summary>
    public class SqlHelper
    {
        /// <summary>
        /// Tạo nhanh một danh sách SqlParameter chỉ với một cặp tên và giá trị.
        /// </summary>
        /// <param name="name">Tên parameter (bao gồm cả ký tự @ nếu cần)</param>
        /// <param name="value">Giá trị của parameter</param>
        /// <returns>Danh sách chứa một SqlParameter duy nhất</returns>
        public static List<SqlParameter> Single(string name, object? value)
        {
            return new List<SqlParameter>
            {
                new SqlParameter(name, value ?? DBNull.Value)
            };
        }

        /// <summary>
        /// Tạo danh sách SqlParameter từ dictionary chứa nhiều cặp key-value.
        /// </summary>
        /// <param name="values">Dictionary chứa tên và giá trị các parameter</param>
        /// <returns>Danh sách SqlParameter tương ứng</returns>
        public static List<SqlParameter> FromDictionary(Dictionary<string, object?> values)
        {
            var list = new List<SqlParameter>();

            foreach (var kvp in values)
            {
                list.Add(new SqlParameter(kvp.Key, kvp.Value ?? DBNull.Value));
            }

            return list;
        }

        /// <summary>
        /// Tạo nhanh SqlParameter có kiểu dữ liệu cụ thể (tùy chọn).
        /// </summary>
        /// <param name="name">Tên parameter</param>
        /// <param name="value">Giá trị</param>
        /// <param name="dbType">Kiểu dữ liệu SqlDbType (nếu có)</param>
        /// <returns>SqlParameter tương ứng</returns>
        public static SqlParameter Typed(string name, object? value, SqlDbType? dbType = null)
        {
            var param = new SqlParameter(name, value ?? DBNull.Value);
            if (dbType.HasValue)
                param.SqlDbType = dbType.Value;
            return param;
        }
    }
}
