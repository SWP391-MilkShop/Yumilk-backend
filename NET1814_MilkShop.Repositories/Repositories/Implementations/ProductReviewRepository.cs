using Microsoft.EntityFrameworkCore;
using NET1814_MilkShop.Repositories.Data;
using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Repositories.Interfaces;

namespace NET1814_MilkShop.Repositories.Repositories.Implementations;

public class ProductReviewRepository : Repository<ProductReview>, IProductReviewRepository
{
    public ProductReviewRepository(AppDbContext context) : base(context)
    {
    }

    public Task<ProductReview?> GetByOrderIdAndProductIdAsync(Guid orderId, Guid productId)
    {
        return _query.FirstOrDefaultAsync(x => x.OrderId == orderId && x.ProductId == productId);
    }

    public IQueryable<ProductReview> GetProductReviewQuery(bool isInclude)
    {
        return isInclude
            ? _query
                .Include(pr => pr.Customer)
                .ThenInclude(c => c!.User).Include(p => p.Product)
            : _query;
    }
}