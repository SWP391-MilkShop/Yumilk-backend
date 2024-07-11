using System.ComponentModel.DataAnnotations;

namespace NET1814_MilkShop.Repositories.Models.VoucherModels;

public class CreateVoucherModel
{
    public string? Description { get; set; } 
    
    [Required(ErrorMessage = "Start date is required")]
    public DateTime StartDate { get; set; }
    
    [Required(ErrorMessage = "End date is required")]
    public DateTime EndDate { get; set; }
    
    [Required(ErrorMessage = "Quantity is required")]
    [Range(0, int.MaxValue, ErrorMessage = "Quantity must be greater than or equal to 0")]
    public int Quantity { get; set; }
    
    [Required(ErrorMessage = "Percent is required")]
    public int Percent { get; set; }
    
    [Range(0, int.MaxValue, ErrorMessage = "Max Discount must be greater than or equal to 0")]
    public int MaxDiscount { get; set; }
    public int MinPriceCondition { get; set; }
}