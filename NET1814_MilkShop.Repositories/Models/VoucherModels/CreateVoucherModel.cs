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
    [Range(5, 50, ErrorMessage = "Percent must be between 5 and 50")]
    public int Percent { get; set; }
    
    [Required(ErrorMessage = "Max Discount is required")]
    [Range(0, int.MaxValue, ErrorMessage = "Max Discount must be greater than or equal to 0")]
    public int MaxDiscount { get; set; }
    
    [Required(ErrorMessage = "Min Price Condition is required")]
    [Range(0, int.MaxValue, ErrorMessage = "Max Discount must be greater than or equal to 0")]
    public int MinPriceCondition { get; set; }
}