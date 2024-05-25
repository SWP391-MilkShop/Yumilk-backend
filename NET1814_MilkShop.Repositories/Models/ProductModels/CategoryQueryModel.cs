namespace NET1814_MilkShop.Repositories.Models.ProductModels
{
    public class CategoryQueryModel : QueryModel
    {
        public string? Name { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
