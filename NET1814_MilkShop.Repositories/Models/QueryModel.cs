namespace NET1814_MilkShop.Repositories.Models
{
    /// <summary>
    /// SearchTerm, SortColumn, SortOrder, Page, PageSize
    /// </summary>
    public abstract class QueryModel //Neu chi dung de ke thua thi nen dung abstract
    {
        public string? SearchTerm { get; set; }
        public string? SortColumn { get; set; }

        /// <summary>
        /// Sort order ("desc" for descending) (default is ascending)
        /// </summary>
        public string? SortOrder { get; set; }

        /// <summary>
        /// Page number (default is 1)
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// Number of items per page (default is 10)
        /// </summary>
        public int PageSize { get; set; } = 10;
    }
}
