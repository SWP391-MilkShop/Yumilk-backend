namespace NET1814_MilkShop.Repositories.Models.ProductModels
{
    public class ProductModel
    {
        public string Id { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public int Quantity { get; set; }

        public decimal OriginalPrice { get; set; }

        public decimal SalePrice { get; set; }

        public string? Thumbnail { get; set; }

        public string Category { get; set; } = null!;

        public string Brand { get; set; } = null!;

        public string Unit { get; set; } = null!;

        public string Status { get; set; } = null!;
    }
}
