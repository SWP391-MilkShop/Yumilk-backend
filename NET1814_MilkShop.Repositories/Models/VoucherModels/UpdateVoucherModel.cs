namespace NET1814_MilkShop.Repositories.Models.VoucherModels;

public class UpdateVoucherModel
{
    public string? Description { get; set; } 
    
    public DateTime? StartDate { get; set; }
    
    public DateTime? EndDate { get; set; }

    public int? Quantity { get; set; }
    
    public int? Percent { get; set; }
    
    public int? MaxDiscount { get; set; }
    
    public bool IsActive { get; set; }
}