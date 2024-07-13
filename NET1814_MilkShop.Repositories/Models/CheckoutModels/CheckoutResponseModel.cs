namespace NET1814_MilkShop.Repositories.Models.CheckoutModels;

public class CheckoutResponseModel
{
    public Guid? OrderId { get; set; }
    public int? OrderCode { get; set; } = null;
    public Guid? CustomerId { get; set; }
    public string? FullName { get; set; }

    public string? Email { get; set; }

    public int TotalPrice { get; set; }
    
    public int TotalAmount { get; set; }

    public int TotalGram { get; set; }
    public int ShippingFee { get; set; }
    public string? Address { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Note { get; set; }
    public string? PaymentMethod { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CheckoutUrl { get; set; }
    public object? OrderDetail { get; set; }
    public string? Message { get; set; }

    public Guid VoucherId { get; set; }

    public bool IsUsingPoint { get; set; }

    public int VoucherDiscount { get; set; }
    public int PointDiscount { get; set; }
    public bool IsPreorder { get; set; }
}