using NET1814_MilkShop.Repositories.CoreHelpers.Enum;

namespace NET1814_MilkShop.Repositories.Models.OrderModels
{
    public class OrderStatsModel
    {
        /// <summary>
        /// Total number of orders
        /// </summary>
        public int TotalOrders { get; set; }
        /// <summary>
        /// Total number of orders per status
        /// </summary>
        public IDictionary<string, int> TotalOrdersPerStatus { get; set; } = new Dictionary<string, int>()
        {
            { OrderStatusId.PENDING.ToString(), 0},
            { OrderStatusId.PROCESSING.ToString(), 0},
            { OrderStatusId.SHIPPING.ToString(), 0},
            { OrderStatusId.DELIVERED.ToString(), 0},
            { OrderStatusId.CANCELLED.ToString(), 0}
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
}
