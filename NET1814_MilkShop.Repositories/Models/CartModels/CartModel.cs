namespace NET1814_MilkShop.Repositories.Models.CartModels
{
    public class CartModel
    {
        public int Id { get; set; }
        public Guid CustomerId { get; set; }
        public int TotalPrice { get; set; }
        public int TotalQuantity { get; set; }
        public object CartItems { get; set; } = new List<CartDetailModel>();
    }
}
