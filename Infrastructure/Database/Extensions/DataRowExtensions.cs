using System.Data;

namespace Infrastructure.Database.Extensions
{
    /// <summary>
    /// Extension methods hỗ trợ thao tác an toàn và tiện lợi với DataRow khi lấy dữ liệu từ SQL.
    /// </summary>
    public static class DataRowExtensions
    {
        /// <summary>
        /// Trả về giá trị không null của một cột trong DataRow.
        /// Nếu là DBNull thì trả về default của kiểu T (ví dụ: 0, null, false,...).
        /// </typeparam>
        /// <typeparam name="T">Kiểu dữ liệu mong muốn</typeparam>
        /// <param name="row">Dòng dữ liệu</param>
        /// <param name="columnName">Tên cột</param>
        /// <returns>Giá trị kiểu T</returns>
        public static T Get<T>(this DataRow row, string columnName)
        {
            var value = row[columnName];
            return value == DBNull.Value ? default! : (T)value;
        }

        /// <summary>
        /// Trả về giá trị nullable của một cột trong DataRow.
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu nullable (phải là struct)</typeparam>
        /// <param name="row">Dòng dữ liệu</param>
        /// <param name="columnName">Tên cột</param>
        /// <returns>Nullable kiểu T</returns>
        public static T? GetNullable<T>(this DataRow row, string columnName) where T : struct
        {
            var value = row[columnName];
            return value == DBNull.Value ? null : (T)value;
        }

        // <summary>
        /// Trả về chuỗi từ một cột trong DataRow. Nếu là DBNull thì trả về null.
        /// </summary>
        /// <param name="row">Dòng dữ liệu</param>
        /// <param name="columnName">Tên cột</param>
        /// <returns>Chuỗi hoặc null</returns>
        public static string? GetString(this DataRow row, string columnName)
        {
            var value = row[columnName];
            return value == DBNull.Value ? null : value.ToString();
        }
    }
}
