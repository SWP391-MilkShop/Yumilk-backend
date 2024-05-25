namespace NET1814_MilkShop.Repositories.Models.ProductModels
{
    public class CategoryModel
    {
        public string Id { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
