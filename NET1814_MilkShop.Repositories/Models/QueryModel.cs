namespace NET1814_MilkShop.Repositories.Models;

/// <summary>
/// SearchTerm, SortColumn, SortOrder, Page, PageSize
/// </summary>
public abstract class QueryModel : PageModel //Neu chi dung de ke thua thi nen dung abstract
{
    public string? SearchTerm { get; set; }
    public string? SortColumn { get; set; }

    /// <summary>
    /// Sort order ("desc" for descending) (default is ascending)
    /// </summary>
    public string? SortOrder { get; set; }
}