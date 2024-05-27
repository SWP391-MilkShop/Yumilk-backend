using System.ComponentModel.DataAnnotations;

namespace NET1814_MilkShop.Repositories.Models.ProductModels
{
    public class UpdateProductModel
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int? Quantity { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Original Price must be greater than 0")]
        public decimal? OriginalPrice { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Sale Price must be greater than 0")]
        public decimal? SalePrice { get; set; }
        public string? Thumbnail { get; set; }
        public int? CategoryId { get; set; }
        public int? BrandId { get; set; }
        public int? UnitId { get; set; }
    }
}
