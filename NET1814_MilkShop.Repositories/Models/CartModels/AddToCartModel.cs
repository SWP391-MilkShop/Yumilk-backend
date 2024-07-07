using System.ComponentModel.DataAnnotations;

namespace NET1814_MilkShop.Repositories.Models.CartModels;

public class AddToCartModel
{
    [Required(ErrorMessage = "ProductId is required")]
    public Guid ProductId { get; set; }

    [Required(ErrorMessage = "Quantity is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public int Quantity { get; set; }
}