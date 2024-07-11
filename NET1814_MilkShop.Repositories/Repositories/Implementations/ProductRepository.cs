using Microsoft.EntityFrameworkCore;
using NET1814_MilkShop.Repositories.Data;
using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Repositories.Interfaces;

namespace NET1814_MilkShop.Repositories.Repositories.Implementations;

public sealed class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(AppDbContext context)
        : base(context)
    {
    }

    public IQueryable<Product> GetProductsQuery(bool includeRating, bool includeOrderCount)
    {
        var query = includeRating ? _query.Include(p => p.ProductReviews) : _query;
        query = includeOrderCount ? query.Include(p => p.OrderDetails).ThenInclude(od => od.Order) : query;
        query = query.Include(p => p.Brand)
            .Include(p => p.Category)
            .Include(p => p.Unit)
            .Include(p => p.ProductStatus).AsSplitQuery();
        return query;
    }

    public Task<Product?> GetByIdAsync(Guid id, bool includeRating, bool includeOrderCount)
    {
        var query = GetProductsQuery(includeRating, includeOrderCount);
        return query.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Product?> GetByIdNoIncludeAsync(Guid id)
    {
        return await _query.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Product?> GetByNameAsync(string name)
    {
        return await _query.FirstOrDefaultAsync(p => p.Name == name);
    }

    public Task<bool> IsExistAsync(Guid id)
    {
        return _query.AnyAsync(x => x.Id == id);
    }

    public async Task<Product?> GetByIdIncludePreorder(Guid id)
    {
        return await _query.Include(x => x.PreorderProduct)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public IQueryable<Product> GetProductQueryNoInclude()
    {
        return _query;
    }

    public override void Update(Product entity)
    {
        var tracker = _context.Attach(entity);
        tracker.State = EntityState.Modified;
    }
}