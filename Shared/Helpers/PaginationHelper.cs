using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static (int startRowIndex, int totalPages) GetStartRowIndexAndTotalPages(int page, int rows, int totalRecords)
        {
            int startRowIndex = ((page * rows) - rows);
            int totalPages = (int)Math.Ceiling((float)totalRecords / rows);
            return (startRowIndex, totalPages);
        }
    }
}
