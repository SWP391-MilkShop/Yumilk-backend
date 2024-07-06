using NET1814_MilkShop.Repositories.CoreHelpers.Enum;

namespace NET1814_MilkShop.Repositories.Models.OrderModels;

public class OrderStatsModel
{
    /// <summary>
    /// Total number of orders
    /// </summary>
    public int TotalOrders { get; set; }

    /// <summary>
    /// Total number of orders per status
    /// </summary>
    public IDictionary<string, int> TotalOrdersPerStatus { get; set; } = new Dictionary<string, int>
    {
        { OrderStatusId.Pending.ToString(), 0 },
        { OrderStatusId.Processing.ToString(), 0 },
        { OrderStatusId.Shipping.ToString(), 0 },
        { OrderStatusId.Delivered.ToString(), 0 },
        { OrderStatusId.Cancelled.ToString(), 0 }
    };

    /// <summary>
    /// Only count orders that have been delivered
    /// </summary>
    public int TotalRevenue { get; set; }

    /// <summary>
    /// Only count orders that have been delivered
    /// </summary>
    public int TotalShippingFee { get; set; }
}