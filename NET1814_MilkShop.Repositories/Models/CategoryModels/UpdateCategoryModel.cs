namespace NET1814_MilkShop.Repositories.Models.CategoryModels
{
    public class UpdateCategoryModel
    {
        public string? Name { get; set; }
        public string? Description { get; set; }

        /// <summary>
        /// Default is true
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}
