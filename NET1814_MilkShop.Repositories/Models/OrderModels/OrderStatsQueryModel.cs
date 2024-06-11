namespace NET1814_MilkShop.Repositories.Models.OrderModels
{
    public class OrderStatsQueryModel
    {
        /// <summary>
        /// From order date (default is last 30 days)
        /// Format is mm-dd-yyyy or yyyy-mm-dd
        /// </summary>
        public DateTime? FromOrderDate { get; set; }
        /// <summary>
        /// To order date (default is now)
        /// Format is mm-dd-yyyy or yyyy-mm-dd
        /// </summary>
        public DateTime? ToOrderDate { get; set; }
    }
}
