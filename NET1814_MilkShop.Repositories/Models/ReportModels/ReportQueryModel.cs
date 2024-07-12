namespace NET1814_MilkShop.Repositories.Models.ReportModels;

public class ReportQueryModel : QueryModel
{
    // issuer
    public Guid CustomerId { get; set; } = Guid.Empty;
    
    public Guid ProductId { get; set; } = Guid.Empty;
    
    public bool? IsResolved { get; set; }

    public int[] ReportTypeIds { get; set; } = [];
    
    /// <summary>
    /// Search by report type name
    /// </summary>
    public new string? SearchTerm { get; set; }
    
    /// <summary>
    /// Sort by created at, resolved at (default: created at)
    /// </summary>
    public new string? SortColumn { get; set; }
    
    
}