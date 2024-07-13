using System.ComponentModel.DataAnnotations;

namespace NET1814_MilkShop.Repositories.Models.ProductModels;

public class UpdateProductModel
{
    public string? Name { get; set; }
    public string? Description { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public int? Quantity { get; set; }

    [Range(1, double.MaxValue, ErrorMessage = "Original Price must be greater than 0")]
    public int? OriginalPrice { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Sale Price must be greater than 0")]
    public int? SalePrice { get; set; }

    public string? Thumbnail { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Category must be greater than or equal to 1")]
    public int CategoryId { get; set; } = 0;

    [Range(0, int.MaxValue, ErrorMessage = "Brand must be greater than or equal to 1")]
    public int BrandId { get; set; } = 0;

    [Range(0, int.MaxValue, ErrorMessage = "Unit must be greater than or equal to 1")]
    public int UnitId { get; set; } = 0;

    [Range(0, 3, ErrorMessage = "Status must be in range 1-3")]
    public int StatusId { get; set; } = 0;

    public bool IsActive { get; set; } 
}