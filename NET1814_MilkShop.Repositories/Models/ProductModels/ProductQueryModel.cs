using System.ComponentModel.DataAnnotations;

namespace NET1814_MilkShop.Repositories.Models.ProductModels;

public class ProductQueryModel : QueryModel
{
    public int[] CategoryIds { get; set; } = [];
    public int[] BrandIds { get; set; } = [];
    public int[] UnitIds { get; set; } = [];

    /// <summary>
    /// 1 => SELLING |
    /// 2 => PREORDER |
    /// 3 => OUT OF STOCK
    /// <para>Default is SELLING</para>
    /// </summary>
    public int StatusId { get; set; } = 1;

    [Range(0, double.MaxValue, ErrorMessage = "Min price must be greater than or equal to 0")]
    public int MinPrice { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Max price must be greater than or equal to 0")]
    public int MaxPrice { get; set; }

    public bool? IsActive { get; set; }

    /// <summary>
    /// true = on sale, false = all
    /// </summary>
    public bool OnSale { get; set; }

    /// <summary>
    /// Sort by id, name, quantity, sale price, created at, rating, order count (default is id)
    /// </summary>
    public new string? SortColumn { get; set; }

    /// <summary>
    /// Search by name, description, brand, unit, category
    /// </summary>
    public new string? SearchTerm { get; set; }
}