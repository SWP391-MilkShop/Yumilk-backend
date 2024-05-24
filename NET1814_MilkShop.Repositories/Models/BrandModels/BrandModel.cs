using System.ComponentModel.DataAnnotations;

namespace NET1814_MilkShop.Repositories.Models.BrandModels;

public class BrandModel
{
    public int Id { get; set; }
    [Required] public string Name { get; set; }

    public string? Description { get; set; }
}