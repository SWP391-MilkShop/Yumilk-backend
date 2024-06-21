namespace NET1814_MilkShop.Repositories.Models.ProductModels;

public class ProductSearchResultModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Brand { get; set; } = null!;
    public int OriginalPrice { get; set; }
    public int SalePrice { get; set; }
    
    public bool IsPreOrder { get; set; }
    
    public double AverageRating { get; set; }
    
    public int RatingCount { get; set; }
    
    public string? Thumbnail { get; set; }
}