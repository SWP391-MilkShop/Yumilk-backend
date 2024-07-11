using System.ComponentModel.DataAnnotations;

namespace NET1814_MilkShop.Repositories.Models.CheckoutModels;

public class PreorderCheckoutModel
{
    [Required] public Guid ProductId { get; set; }

    [Required]
    [Range(0, Int32.MaxValue, ErrorMessage = "Số lượng mua phải lớn hơn 0")]
    public int Quantity { get; set; }

    [Required]
    [Range(0, Int32.MaxValue, ErrorMessage = "Phí ship tối thiểu 0 VND")]
    public int ShippingFee { get; set; }

    [Required] public int AddressId { get; set; }

    public string? Note { get; set; }
    public bool IsUsingPoint { get; set; } = true;
}