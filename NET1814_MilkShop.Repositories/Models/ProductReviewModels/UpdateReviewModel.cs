using System.ComponentModel.DataAnnotations;

namespace NET1814_MilkShop.Repositories.Models.ProductReviewModels
{
    public class UpdateReviewModel
    {
        public string? Review { get; set; }
        public int Rating { get; set; } = 0;
        public bool? IsActive { get; set; }
    }
}
