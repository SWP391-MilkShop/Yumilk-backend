namespace NET1814_MilkShop.Repositories.Models.CategoryModels;

public class CategoryModel
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;
}