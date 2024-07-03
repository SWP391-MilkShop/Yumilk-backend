namespace NET1814_MilkShop.Repositories.Models.PostModel
{
    public class PostQueryModel : QueryModel
    {
        public Guid AuthorId { get; set; } = Guid.Empty;
        public bool? IsActive { get; set; }

        /// <summary>
        /// Search by title, content, meta title, meta description
        /// </summary>
        public new string? SearchTerm { get; set; }

        /// <summary>
        /// Sort by title, update at, created at (default is updated at)
        /// </summary>
        public new string? SortColumn { get; set; }
    }
}
