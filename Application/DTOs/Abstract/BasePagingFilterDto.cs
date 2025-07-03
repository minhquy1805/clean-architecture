using Application.Common.Attributes;
using Shared.Helpers;


namespace Application.DTOs.Abstract
{
    public class BasePagingFilterDto : BaseFilterDto
    {
        [IgnoreSqlParam]
        public int Start { get; set; } = 0;

        [IgnoreSqlParam]
        public int NumberOfRows { get; set; } = GridConfig.NumberOfRows;

        [IgnoreSqlParam]
        public string SortBy { get; set; } = "CreatedAt";

        [IgnoreSqlParam]
        public string SortDirection { get; set; } = "DESC";

        [IgnoreSqlParam]
        public int CurrentPage { get; set; } = 1;

    }
}
