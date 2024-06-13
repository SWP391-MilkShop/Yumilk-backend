namespace NET1814_MilkShop.Repositories.Models.UserModels;

public class CustomersStatsQueryModel
{
    /// <summary>
    /// From date (default is last 30 days)
    /// Format is mm-dd-yyyy or yyyy-mm-dd
    /// </summary>
    public DateTime? From { get; set; }

    /// <summary>
    /// To date (default is now)
    /// Format is mm-dd-yyyy or yyyy-mm-dd
    /// </summary>
    public DateTime? To { get; set; }
}