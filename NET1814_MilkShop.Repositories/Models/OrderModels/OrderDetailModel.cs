namespace NET1814_MilkShop.Repositories.Models.OrderModels;

public class OrderDetailModel
{
    public string? RecieverName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? Note { get; set; }
    public List<CheckoutOrderDetailModel> OrderDetail { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal ShippingFee { get; set; }
    public decimal TotalAmount { get; set; }
    public string? PaymentMethod { get; set; }

    public string? OrderStatus { get; set; }
}