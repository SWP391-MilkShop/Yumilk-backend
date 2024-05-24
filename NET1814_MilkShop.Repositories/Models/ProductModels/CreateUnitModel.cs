using System.ComponentModel.DataAnnotations;

namespace NET1814_MilkShop.Repositories.Models.ProductModels;

public class CreateUnitModel
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;
}
