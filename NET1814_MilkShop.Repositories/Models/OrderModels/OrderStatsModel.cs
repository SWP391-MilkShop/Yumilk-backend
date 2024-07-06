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
    public List<OrderStatusCount> TotalOrdersPerStatus { get; set; } =
    [
        new OrderStatusCount { Status = OrderStatusId.Pending.ToString(), Count = 0 },
        new OrderStatusCount { Status = OrderStatusId.Processing.ToString(), Count = 0 },
        new OrderStatusCount { Status = OrderStatusId.Preorder.ToString(), Count = 0 },
        new OrderStatusCount { Status = OrderStatusId.Shipping.ToString(), Count = 0 },
        new OrderStatusCount { Status = OrderStatusId.Delivered.ToString(), Count = 0 },
        new OrderStatusCount { Status = OrderStatusId.Cancelled.ToString(), Count = 0 }
    ];
    /// <summary>
    /// Only count orders that have been delivered
    /// </summary>
    public int TotalRevenue { get; set; }
    /// <summary>
    /// Only count orders that have been delivered
    /// </summary>
    public int TotalShippingFee { get; set; }
}
public class OrderStatusCount
{
    public string Status { get; set; } = null!;
    public int Count { get; set; }
}