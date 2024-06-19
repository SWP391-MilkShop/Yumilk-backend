namespace NET1814_MilkShop.Repositories.Models.ProductModels;

public class ProductSearchModel : QueryModel
{
    /// <summary>
    /// Sort by id, name, quantity, sale price, created at, rating, order count (default is id)
    /// </summary>
    public new string? SortColumn { get; set; }

    /// <summary>
    /// Search by name, description, brand, unit, category
    /// </summary>
    public new string? SearchTerm { get; set; }
}