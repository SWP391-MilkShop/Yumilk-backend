using Microsoft.EntityFrameworkCore;
using NET1814_MilkShop.Repositories.Data;
using NET1814_MilkShop.Repositories.Data.Entities;
using System.Linq.Expressions;

namespace NET1814_MilkShop.Repositories.Repositories
{
    public interface IProductRepository
    {
        /// <summary>
        /// Get all products with corresponding brand, category, unit, product status
        /// And product reviews, order details if includeRating and includeOrderCount is true
        /// </summary>
        /// <param name="includeRating"></param>
        /// <param name="includeOrderCount"></param>
        /// <returns></returns>
        IQueryable<Product> GetProductsQuery(bool includeRating, bool includeOrderCount);
        IQueryable<Product> GetProductQueryNoInclude();
        /// <summary>
        /// Get product by id with corresponding brand, category, unit, product status
        /// And product reviews, order details if includeRating and includeOrderCount is true
        /// </summary>
        /// <param name="id"></param>
        /// <param name="includeRating"></param>
        /// <param name="includeOrderCount"></param>
        /// <returns></returns>
        Task<Product?> GetByIdAsync(Guid id, bool includeRating, bool includeOrderCount);

        /// <summary>
        ///  Get product by id without including brand, category, unit, product status
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Product?> GetByIdNoIncludeAsync(Guid id);

        /// <summary>
        /// Get product by name for checking duplicate
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<Product?> GetByNameAsync(string name);
        void Add(Product product);
        void Update(Product product);
        void Delete(Product product);
        Task<bool> IsExistAsync(Guid id);
    }

    public sealed class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context)
            : base(context) { }

        public IQueryable<Product> GetProductsQuery(bool includeRating, bool includeOrderCount)
        {
            var query = includeRating ? _query.Include(p => p.ProductReviews) : _query;
            query = includeOrderCount ? query.Include(p => p.OrderDetails) : query;
            query =  query.Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Unit)
                .Include(p => p.ProductStatus).AsSplitQuery();
            return query;
        }

        public Task<Product?> GetByIdAsync(Guid id, bool includeRating, bool includeOrderCount)
        {
            var query = includeRating ? _query.Include(p => p.ProductReviews) : _query;
            query = includeOrderCount ? query.Include(p => p.OrderDetails) : query;
            query = query.Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.Unit)
                .Include(p => p.ProductStatus).AsSplitQuery();
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

        public IQueryable<Product> GetProductQueryNoInclude()
        {
            return _query;
        }
    }
}
