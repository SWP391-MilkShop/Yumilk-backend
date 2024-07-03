using System.ComponentModel.DataAnnotations;

namespace NET1814_MilkShop.Repositories.Models.CheckoutModels;

public class CheckoutModel
{
    [Required]
    [Range(0, Int32.MaxValue, ErrorMessage = "Phí ship tối thiểu 0 VND")]
    public int ShippingFee { get; set; }

    [Required]
    public int AddressId { get; set; }

    public string? Note { get; set; }

    [Required]
    public string? PaymentMethod { get; set; }
}
