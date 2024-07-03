using Microsoft.EntityFrameworkCore;
using NET1814_MilkShop.Repositories.Data;
using NET1814_MilkShop.Repositories.Data.Entities;

namespace NET1814_MilkShop.Repositories.Repositories;

public interface IProductReviewRepository
{
    IQueryable<ProductReview> GetProductReviewQuery(bool includeCustomer);
    Task<ProductReview?> GetByIdAsync(int id);
    Task<ProductReview?> GetByOrderIdAndProductIdAsync(Guid orderId, Guid productId);
    void Add(ProductReview productReview);
    void Update(ProductReview productReview);
    void Delete(ProductReview productReview);
}

public class ProductReviewRepository : Repository<ProductReview>, IProductReviewRepository
{
    public ProductReviewRepository(AppDbContext context) : base(context)
    {
    }

    public Task<ProductReview?> GetByOrderIdAndProductIdAsync(Guid orderId, Guid productId)
    {
        return _query.FirstOrDefaultAsync(x => x.OrderId == orderId && x.ProductId == productId);
    }

    public IQueryable<ProductReview> GetProductReviewQuery(bool includeCustomer)
    {
        return includeCustomer
            ? _query
                .Include(pr => pr.Customer)
                .ThenInclude(c => c!.User)
            : _query;
    }
}