namespace NET1814_MilkShop.Repositories.Models.OrderModels
{
    public class OrderModel
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public decimal TotalAmount { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? PaymentMethod { get; set; }
        public string? OrderStatus { get; set; }

        public DateTime? CreatedDate { get; set; }
        public DateTime? PaymentDate { get; set; }
    }
}