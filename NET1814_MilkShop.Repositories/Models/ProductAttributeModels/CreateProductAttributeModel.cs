using System.ComponentModel.DataAnnotations;

namespace NET1814_MilkShop.Repositories.Models.ProductAttributeModels;

public class CreateProductAttributeModel
{
    [Required(ErrorMessage = "Product attribute name is required")]
    public string Name { get; set; }
    public string? Description { get; set; } 
    
}