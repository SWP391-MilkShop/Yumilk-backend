using System.ComponentModel.DataAnnotations;

namespace NET1814_MilkShop.Repositories.Models.ProductReviewModels;

public class CreateReviewModel
{
    [Required(ErrorMessage = "Order Id is required")]
    public Guid OrderId { get; set; }

    [Required(ErrorMessage = "Review is required")]
    public string Review { get; set; } = null!;

    [Required(ErrorMessage = "Rating is required")]
    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
    public int Rating { get; set; }
}