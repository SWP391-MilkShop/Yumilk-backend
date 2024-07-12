namespace NET1814_MilkShop.Repositories.Models.OrderModels;

/// <summary>
/// Logs will be displayed in the order detail page, therefore Guid orderIs is not included in OrderLogsModel
/// </summary>
public class OrderLogsModel
{
    public string? Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
