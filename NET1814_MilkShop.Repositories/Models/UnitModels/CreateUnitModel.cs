using System.ComponentModel.DataAnnotations;

namespace NET1814_MilkShop.Repositories.Models.UnitModels;

public class CreateUnitModel
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;
    [Required]
    [Range(1,int.MaxValue)]
    public int Gram { get; set; }
}
