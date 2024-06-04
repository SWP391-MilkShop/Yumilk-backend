using System.ComponentModel.DataAnnotations;

namespace NET1814_MilkShop.Repositories.Models.CheckoutModels;

public class CheckoutModel
{
    public decimal ShippingFee { get; set; }
    [Required] public string? Address { get; set; }
    [Required] public string? PhoneNumber { get; set; }
    public string? Note { get; set; }
    [Required] public string? PaymentMethod { get; set; }
}