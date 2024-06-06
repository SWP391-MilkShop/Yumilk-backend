namespace NET1814_MilkShop.Repositories.Models.CheckoutModels;

public class CheckoutResponseModel
{
    public Guid? OrderId { get; set; }
    public Guid? CustomerId { get; set; }
    public string? FullName { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal ShippingFee { get; set; }
    public string? Address { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Note { get; set; }
    public object? OrderDetail { get; set; }
}
