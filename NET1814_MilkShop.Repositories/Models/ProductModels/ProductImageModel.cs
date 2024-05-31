namespace NET1814_MilkShop.Repositories.Models.ProductModels
{
    public class ProductImageModel
    {
        public int Id { get; set; }

        public Guid ProductId { get; set; }

        public string ImageUrl { get; set; } = null!;

    }
}
