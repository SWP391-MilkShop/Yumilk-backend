namespace NET1814_MilkShop.Repositories.Models.VoucherModels;

public class VoucherQueryModel : QueryModel
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool? IsActive { get; set; }
    /// <summary>
    /// Filter voucher satisfy min_price_condition
    /// </summary>
    public int? MinPriceCondition { get; set; }
    /// <summary>
    /// Search by voucher code and description
    /// </summary>
    public new string? SearchTerm { get; set; }
    /// <summary>
    /// Sort by created at, start date, end date, quantity, percent, max discount, min price condition
    /// (Default is percent)
    /// </summary>
    public new string? SortColumn { get; set; }

    /// <summary>
    /// Sort order ("desc" for descending) (default is ascending)
    /// </summary>
    public new string? SortOrder { get; set; }
}