namespace NET1814_MilkShop.Repositories.Models.CartModels
{
    public class CartDetailModel
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = "";
        public string? Thumbnail { get; set; }
        public int Quantity { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal SalePrice { get; set; }
    }
}
