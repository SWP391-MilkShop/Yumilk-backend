using System.ComponentModel.DataAnnotations;

namespace NET1814_MilkShop.Repositories.Models.ProductModels
{
    public class CreateProductModel
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(255, ErrorMessage = "Name must be less than 255 characters")]
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Original Price is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Original Price must be greater than 0")]
        public int OriginalPrice { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Sale Price must be greater than 0")]
        public int SalePrice { get; set; }
        public string? Thumbnail { get; set; }

        [Required(ErrorMessage = "Category is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Category must be greater than 1")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Brand is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Brand must be greater than 1")]
        public int BrandId { get; set; }

        [Required(ErrorMessage = "Unit is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Unit must be greater than 1")]
        public int UnitId { get; set; }

        //Status default is Pending
    }
}
