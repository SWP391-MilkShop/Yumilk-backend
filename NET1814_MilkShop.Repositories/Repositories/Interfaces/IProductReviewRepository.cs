using NET1814_MilkShop.Repositories.Data.Entities;

namespace NET1814_MilkShop.Repositories.Repositories.Interfaces;

public interface IProductReviewRepository
{
    IQueryable<ProductReview> GetProductReviewQuery(bool includeCustomer);
    Task<ProductReview?> GetByIdAsync(int id);
    Task<ProductReview?> GetByOrderIdAndProductIdAsync(Guid orderId, Guid productId);
    void Add(ProductReview productReview);
    void Update(ProductReview productReview);
    void Delete(ProductReview productReview);
}