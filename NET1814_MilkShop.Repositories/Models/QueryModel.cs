namespace NET1814_MilkShop.Repositories.Models;

public abstract class QueryModel //Neu chi dung de ke thua thi nen dung abstract
{
    public string? SearchTerm { get; set; }
    public string? SortColumn { get; set; }

    /// <summary>
    ///     Sort order ("desc" for descending) (default is ascending)
    /// </summary>
    public string? SortOrder { get; set; }

    /// <summary>
    ///     Page number
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    ///     Number of items per page
    /// </summary>
    public int PageSize { get; set; } = 10;
}