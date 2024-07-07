namespace NET1814_MilkShop.Repositories.Models.ProductReviewModels;

public class ReviewQueryModel : ProductReviewQueryModel
{
    public Guid ProductId { get; set; }
    public Guid OrderId { get; set; }
    public Guid CustomerId { get; set; }
}