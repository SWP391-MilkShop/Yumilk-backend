namespace NET1814_MilkShop.Repositories.Models.VoucherModels;

public class VoucherModel
{
    public Guid Id { get; set; }
    
    /// <summary>
    /// Mã voucher tối đa 10 ký tự
    /// </summary>
    public string Code { get; set; } = null!;

    public string Description { get; set; } = null!;
    
    public DateTime StartDate { get; set; }
    
    public DateTime EndDate { get; set; }
    
    /// <summary>
    /// Số lượng voucher còn lại
    /// </summary>
    public int Quantity { get; set; }
    
    /// <summary>
    /// Phần trăm giảm giá
    /// </summary>
    public int Percent { get; set; }
    
    public bool IsActive { get; set; }
    
    public bool IsAvailable { get; set; }
    
    public int MaxDiscount { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public int MinPriceCondition { get; set; }
}