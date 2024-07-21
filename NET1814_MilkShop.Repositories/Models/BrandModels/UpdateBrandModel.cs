namespace NET1814_MilkShop.Repositories.Models.BrandModels;

public class UpdateBrandModel
{
    public string? Name { get; set; } = null;

    public string? Description { get; set; } = null;

    public string? Logo { get; set; }

    public bool IsActive { get; set; } = true;
}