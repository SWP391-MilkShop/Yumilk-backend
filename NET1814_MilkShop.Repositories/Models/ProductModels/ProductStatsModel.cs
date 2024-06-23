namespace NET1814_MilkShop.Repositories.Models.ProductModels
{
    public class ProductStatsModel
    {
        public int TotalSold { get; set; }
        public int TotalRevenue { get; set; }
        public IDictionary<string, CategoryBrandStats> StatsPerCategory { get; set; } = 
            new Dictionary<string, CategoryBrandStats>();

        public IDictionary<string, CategoryBrandStats> StatsPerBrand { get; set; } =
            new Dictionary<string, CategoryBrandStats>();
    }

    public class CategoryBrandStats
    {
        public int TotalSold { get; set; }
        public int TotalRevenue { get; set; }
    }
}
