namespace NET1814_MilkShop.Repositories.Models.CategoryModels;

public class CategoryQueryModel : QueryModel
{
    /// <summary>
    /// Set parent id to get sub categories
    /// </summary>
    public int ParentId { get; set; } = 0;

    public bool? IsActive { get; set; }

    /// <summary>
    /// Sort by id or name (default is id)
    /// </summary>
    public new string? SortColumn { get; set; }

    /// <summary>
    /// Search by name
    /// </summary>
    public new string? SearchTerm { get; set; }
}