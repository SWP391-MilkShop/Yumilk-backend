namespace NET1814_MilkShop.Repositories.Models;

public abstract class PageModel
{
    /// <summary>
    /// Page number (default is 1)
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Number of items per page (default is 10)
    /// </summary>
    public int PageSize { get; set; } = 10;
}