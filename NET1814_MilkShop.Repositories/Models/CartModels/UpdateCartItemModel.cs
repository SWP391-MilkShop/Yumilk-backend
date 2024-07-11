using System.ComponentModel.DataAnnotations;

namespace NET1814_MilkShop.Repositories.Models.CartModels;

public class UpdateCartItemModel
{
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public int Quantity { get; set; }
}