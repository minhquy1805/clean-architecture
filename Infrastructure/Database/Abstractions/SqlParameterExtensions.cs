using Microsoft.Data.SqlClient;
using System.Reflection;
using Application.Enums;
using Infrastructure.Abstractions.Metadata;

namespace Infrastructure.Database.Abstractions
{
    /// <summary>
    /// Extension method hỗ trợ chuyển một object thành danh sách SqlParameter,
    /// dùng trong việc tương tác với Stored Procedure trong ADO.NET.
    /// </summary>
    public static class SqlParameterExtensions
    {
        /// <summary>
        /// Tạo danh sách SqlParameter từ một object và danh sách tên các thuộc tính cụ thể.
        /// </summary>
        /// <typeparam name="T">Kiểu object (Entity hoặc DTO)</typeparam>
        /// <param name="obj">Object cần chuyển thành SqlParameter</param>
        /// <param name="fieldNames">Danh sách tên các thuộc tính cần chuyển</param>
        /// <returns>Danh sách SqlParameter</returns>
        public static List<SqlParameter> ToSqlParameters<T>(this T obj, string[] fieldNames)
        {
            // Khởi tạo danh sách kết quả
            var parameters = new List<SqlParameter>();

            // Trường hợp đầu vào không hợp lệ thì trả về danh sách rỗng
            if (obj == null || fieldNames == null || fieldNames.Length == 0)
                return parameters;

            // Lấy tất cả thuộc tính public instance của kiểu T
            var type = typeof(T);
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Lặp qua từng field được chỉ định trong mảng fieldNames
            foreach (var field in fieldNames)
            {
                // Tìm property tương ứng với field (không phân biệt hoa thường)
                var prop = props.FirstOrDefault(p => string.Equals(p.Name, field, StringComparison.OrdinalIgnoreCase));
                if (prop == null)
                    continue; // Nếu không tìm thấy property thì bỏ qua

                // Lấy giá trị của property, nếu null thì gán là DBNull để tương thích với database
                var value = prop.GetValue(obj) ?? DBNull.Value;

                // Thêm parameter vào danh sách, tên parameter trùng với tên cột trong store (prefix @)
                parameters.Add(new SqlParameter("@" + prop.Name, value));
            }

            return parameters;
        }

        /// <summary>
        /// Chuyển object thành danh sách SqlParameter dựa vào loại thao tác (Create/Update),
        /// và lấy field từ MetadataRegistry.
        /// </summary>
        public static List<SqlParameter> ToSqlParameters<T>(this T obj, SqlOperationType operationType)
        {
            var type = typeof(T);

            string[] fieldList = operationType switch
            {
                SqlOperationType.Create => MetadataRegistry.GetCreateFields(type),
                SqlOperationType.Update => MetadataRegistry.GetUpdateFields(type),
                _ => throw new NotSupportedException($"Operation '{operationType}' is not supported.")
            };

            return obj.ToSqlParameters(fieldList);
        }
    }
}
