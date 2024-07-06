namespace NET1814_MilkShop.Repositories.Models.CategoryModels;

public class CategoryModel
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int? ParentId { get; set; }

    public string? ParentName { get; set; }

    public bool IsActive { get; set; } = true;
}