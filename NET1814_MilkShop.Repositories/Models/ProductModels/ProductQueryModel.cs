using System.ComponentModel.DataAnnotations;

namespace NET1814_MilkShop.Repositories.Models.ProductModels;

public class ProductQueryModel : QueryModel
{
    public string? Category { get; set; }
    public string? Brand { get; set; }
    public string? Unit { get; set; }
    public string? Status { get; set; }

    [Range(0, double.MaxValue)] public decimal MinPrice { get; set; }

    [Range(0, double.MaxValue)] public decimal MaxPrice { get; set; }

    public bool IsActive { get; set; } = true;

    /// <summary>
    ///     Sort by id, name, quantity, sale price (default is id)
    /// </summary>
    public new string? SortColumn { get; set; }

    /// <summary>
    ///     Search by name, description, brand, unit, category
    /// </summary>
    public new string? SearchTerm { get; set; }
}