namespace NET1814_MilkShop.Repositories.Models.VoucherModels;

public class VoucherQueryModel : QueryModel
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool? IsActive { get; set; }

    /// <summary>
    /// Search by voucher code and description
    /// </summary>
    public new string? SearchTerm { get; set; }
    /// <summary>
    /// Sort by created_at, start_date, end_date, quantity, percent, max_discount
    /// </summary>
    public new string? SortColumn { get; set; }

    /// <summary>
    /// Sort order ("desc" for descending) (default is ascending)
    /// </summary>
    public new string? SortOrder { get; set; }
}