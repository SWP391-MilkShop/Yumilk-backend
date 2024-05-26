namespace NET1814_MilkShop.Repositories.Models.ProductModels
{
    public class CategoryQueryModel : QueryModel
    {
        public bool IsActive { get; set; } = true;
        /// <summary>
        /// Sort by id or name (default is id)
        /// </summary>
        public new string? SortColumn { get; set; }
        /// <summary>
        /// Search by name
        /// </summary>
        public new string? SearchTerm { get; set; }
    }
}
