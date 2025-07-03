namespace Shared.Helpers
{
    public static class PaginationHelper
    {
        public static int GetPagerStartPage(int currentPage, int numberOfPagesToShow) 
        {
            if (currentPage <= numberOfPagesToShow)
                return 1;
            else if (currentPage % numberOfPagesToShow == 0)
                return ((currentPage / numberOfPagesToShow) - 1) * numberOfPagesToShow + 1;
            else
                return (currentPage / numberOfPagesToShow) * numberOfPagesToShow + 1;
        }

        public static int GetPagerEndPage(int startPage, int numberOfPagesToShow, int totalPages)
        {
            int endPage = startPage + (numberOfPagesToShow - 1);
            return endPage >= totalPages ? totalPages : endPage;
        }

        public static (int Start, int TotalPages) GetStartRowIndexAndTotalPages(int page, int rows, int totalRecords)
        {
            if (rows <= 0) rows = GridConfig.NumberOfRows;
            var totalPages = CalculateTotalPages(totalRecords, rows);
            var start = (page - 1) * rows;
            return (start, totalPages);
        }

        public static int CalculateTotalPages(int totalRecords, int rowsPerPage)
        {
            if (rowsPerPage <= 0) rowsPerPage = GridConfig.NumberOfRows;
            return (int)Math.Ceiling((double)totalRecords / rowsPerPage);
        }



    }
}
