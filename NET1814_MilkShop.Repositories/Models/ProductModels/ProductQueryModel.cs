namespace NET1814_MilkShop.Repositories.Models.ProductModels
{
    public class ProductQueryModel : QueryModel
    {
        public string? Category { get; set; }
        public string? Brand { get; set; }
        public string? Unit { get; set; }
        public string? Status { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
