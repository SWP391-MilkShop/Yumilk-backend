namespace NET1814_MilkShop.Repositories.Models.ProductModels;

public class ProductStatsModel
{
    public int TotalSold { get; set; }
    public int TotalRevenue { get; set; }
    public List<CategoryBrandStats> StatsPerCategory { get; set; } = [];

    public List<CategoryBrandStats> StatsPerBrand { get; set; } = [];
    public List<BestSellerModel> BestSellers { get; set; } = [];
}

public class CategoryBrandStats
{
    public string Name { get; set; } = null!;
    public int TotalSold { get; set; }
    public int TotalRevenue { get; set; }
}

public class BestSellerModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public int TotalSold { get; set; }
    public int TotalRevenue { get; set; }
}