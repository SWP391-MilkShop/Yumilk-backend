namespace NET1814_MilkShop.Repositories.Models.OrderModels;

public class OrderDetailModel
{
    public Guid Id { get; set; }
    public Guid? CustomerId { get; set; }
    public string? ReceiverName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? Note { get; set; }
    public List<CheckoutOrderDetailModel> OrderDetail { get; set; } = [];
    public int TotalPriceBeforeDiscount { get; set; }
    public int VoucherDiscount { get; set; }
    public int PointDiscount { get; set; }
    public int TotalPriceAfterDiscount { get; set; }
    public int RecievingPoint { get; set; }
    public int ShippingFee { get; set; }
    public int TotalAmount { get; set; }
    public string? PaymentMethod { get; set; }
    public string? OrderStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; } = null;
    public object? PaymentData { get; set; }
    public List<OrderLogsModel?> Logs { get; set; }
    public bool IsPreorder { get; set; }

    public string? ShippingCode { get; set; }
}