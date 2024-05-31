namespace NET1814_MilkShop.Repositories.Models.ProductAttributeModels;

public class ProductAttributeQueryModel : QueryModel
{
    public bool? IsActive { get; set; }

    /// <summary>
    /// Search by name or description
    /// </summary>
    public new string? SearchTerm { get; set; }

    /// <summary>
    /// Sort by id or name (default is id)
    /// </summary>
    public new string? SortColumn { get; set; }
}