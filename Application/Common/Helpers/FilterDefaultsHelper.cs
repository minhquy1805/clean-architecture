using Application.DTOs.Abstract;
using Shared.Helpers;


namespace Application.Common.Helpers
{
    public static class FilterDefaultsHelper
    {
        public static void ApplyDefaults(BasePagingFilterDto filter)
        {
            if (filter == null) return;

            // Đảm bảo số dòng mỗi trang
            if (filter.NumberOfRows <= 0)
                filter.NumberOfRows = GridConfig.NumberOfRows;

            // Đảm bảo CurrentPage >= 1
            if (filter.CurrentPage <= 0)
                filter.CurrentPage = 1;

            // Xử lý SortBy nếu để trống (nếu cần)
            if (string.IsNullOrWhiteSpace(filter.SortBy))
                filter.SortBy = "CreatedAt";

            // SortDirection hợp lệ
            if (string.IsNullOrWhiteSpace(filter.SortDirection) ||
                (filter.SortDirection.ToUpper() != "ASC" && filter.SortDirection.ToUpper() != "DESC"))
            {
                filter.SortDirection = "DESC";
            }
        }
    }
}
