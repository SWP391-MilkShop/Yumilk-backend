using System.ComponentModel.DataAnnotations;

namespace NET1814_MilkShop.Repositories.Models.CheckoutModels;

public class CheckoutModel
{
    public decimal ShippingFee { get; set; }

    [Required] public int AddressId { get; set; }
    public string? Note { get; set; }
    [Required] public string? PaymentMethod { get; set; }
}