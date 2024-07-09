using System.ComponentModel.DataAnnotations;

namespace NET1814_MilkShop.Repositories.Models.ProductAttributeModels;

public class UpdateProductAttributeModel
{
    [Required(ErrorMessage = "Product attribute name is required")]
    public string? Name { get; set; } = null;

    public string? Description { get; set; } = null;

    /// <summary>
    /// default value is true
    /// </summary>
    public bool IsActive { get; set; } = true;
}