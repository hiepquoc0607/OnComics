namespace OnComics.Library.Models.Response.Api
{
    public class Pagination
    {
        public Pagination(int totalItems, int pageIndex, int pageNum, int totalPages)
        {
            TotalItems = totalItems;
            PageIndex = pageIndex;
            PageNum = pageNum;
            TotalPages = totalPages;
        }

        public int TotalItems { get; set; }

        public int PageIndex { get; set; }

        public int PageNum { get; set; }

        public int TotalPages { get; set; }
    }
}
