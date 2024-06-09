namespace NET1814_MilkShop.Repositories.Models.ProductReviewModels
{
    public class ReviewModel
    {
        public int Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid ProductId { get; set; }
        public string Review { get; set; } = null!;
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
