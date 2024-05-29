namespace NET1814_MilkShop.Repositories.Models.ProductAttributeValueModels;

public class ProductAttributeValueQueryModel : QueryModel
{
    public Guid? ProductId { get; set; }

    /// <summary>
    /// search by value
    /// </summary>
    public new string? SearchTerm { get; set; }
    /// <summary>
    /// sort by productid, attributeid (default: sort by name)
    /// </summary>
    public new string? SortColumn { get; set; }
}