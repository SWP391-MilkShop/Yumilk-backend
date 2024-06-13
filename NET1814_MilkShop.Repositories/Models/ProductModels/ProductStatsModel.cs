namespace NET1814_MilkShop.Repositories.Models.ProductModels
{
    public class ProductStatsModel
    {
        public int TotalSold { get; set; }
        public IDictionary<string, int> TotalSoldPerCategory { get; set; } = new Dictionary<string, int>();
        public IDictionary<string, int> TotalSoldPerBrand { get; set; } = new Dictionary<string, int>();
    }
}
