namespace NET1814_MilkShop.Repositories.Models.ProductModels;

public class ProductModel
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int Quantity { get; set; }

    public int OriginalPrice { get; set; }

    public int SalePrice { get; set; }

    public string? Thumbnail { get; set; }

    public int CategoryId { get; set; }

    public string Category { get; set; } = null!;

    public int BrandId { get; set; }

    public string Brand { get; set; } = null!;

    public int UnitId { get; set; }

    public string Unit { get; set; } = null!;

    public int StatusId { get; set; }

    public string Status { get; set; } = null!;

    public double AverageRating { get; set; }

    public int RatingCount { get; set; }

    public int OrderCount { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }
}