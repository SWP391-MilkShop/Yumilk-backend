namespace NET1814_MilkShop.Repositories.Models.ProductReviewModels;

public class ReviewModel
{
    public int Id { get; set; }
    public Guid CustomerId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? ProfilePicture { get; set; }
    public Guid ProductId { get; set; }
    public string? ProductName { get; set; }
    public string Review { get; set; } = null!;
    public int Rating { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}