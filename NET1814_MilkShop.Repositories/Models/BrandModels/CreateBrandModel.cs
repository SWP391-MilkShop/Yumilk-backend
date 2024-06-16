using System.ComponentModel.DataAnnotations;

namespace NET1814_MilkShop.Repositories.Models.BrandModels;

public class CreateBrandModel
{
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Description is required")]
    public string? Description { get; set; } = null;

}