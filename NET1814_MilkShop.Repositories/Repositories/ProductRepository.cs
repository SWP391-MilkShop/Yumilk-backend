using Microsoft.EntityFrameworkCore;
using NET1814_MilkShop.Repositories.Data;
using NET1814_MilkShop.Repositories.Data.Entities;

namespace NET1814_MilkShop.Repositories.Repositories
{
    public interface IProductRepository
    {
        IQueryable<Product> GetProductsQuery();
    }

    public sealed class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context)
            : base(context) { }

        public IQueryable<Product> GetProductsQuery()
        {
            return _context
                .Products.Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Unit)
                .AsNoTracking();
        }
    }
}
