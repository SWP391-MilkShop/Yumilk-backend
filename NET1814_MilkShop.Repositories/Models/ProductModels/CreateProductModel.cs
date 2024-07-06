using System.ComponentModel.DataAnnotations;

namespace NET1814_MilkShop.Repositories.Models.ProductModels;

public class CreateProductModel
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(255, ErrorMessage = "Name must be less than 255 characters")]
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    [Required(ErrorMessage = "Quantity is required")]
    [Range(0, int.MaxValue, ErrorMessage = "Quantity must be greater than or equal to 0")]
    public int Quantity { get; set; }

    [Required(ErrorMessage = "Original Price is required")]
    [Range(0, double.MaxValue, ErrorMessage = "Original Price must be greater than or equal to 0")]
    public int OriginalPrice { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Sale Price must be greater than or equal to 0")]
    public int SalePrice { get; set; }

    public string? Thumbnail { get; set; }

    [Required(ErrorMessage = "Category is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Category must be greater than or equal to 1")]
    public int CategoryId { get; set; }

    [Required(ErrorMessage = "Brand is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Brand must be greater than or equal to 1")]
    public int BrandId { get; set; }

    [Required(ErrorMessage = "Unit is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Unit must be greater than or equal to 1")]
    public int UnitId { get; set; }

    /// <summary>
    /// 1. SELLING  2.PRE_ORDER
    /// Neu set selling thi quantity phai lon hon 0
    /// Neu set PRE_ORDER thi quantity = 0
    /// </summary>
    [Range(1, 2, ErrorMessage = "Status must be in range 1-2")]
    public int StatusId { get; set; }
}