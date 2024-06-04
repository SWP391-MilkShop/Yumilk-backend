using Microsoft.EntityFrameworkCore;
using NET1814_MilkShop.Repositories.Data;
using NET1814_MilkShop.Repositories.Data.Entities;

namespace NET1814_MilkShop.Repositories.Repositories
{
    public interface IProductRepository
    {
        /// <summary>
        /// Get all products with corresponding brand, category, unit, product status
        /// </summary>
        /// <returns></returns>
        IQueryable<Product> GetProductsQuery();
        /// <summary>
        /// Get product by id with corresponding brand, category, unit, product status
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Product?> GetByIdAsync(Guid id);
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

        public IQueryable<Product> GetProductsQuery()
        {
            //return _context
            //    .Products.Include(p => p.Brand)
            //    .Include(p => p.Category)
            //    .Include(p => p.Unit)
            //    .AsNoTracking();
            return _query.Include(p => p.Brand)
                         .Include(p => p.Category)
                         .Include(p => p.Unit)
                         .Include(p => p.ProductStatus);
        }

        public override Task<Product?> GetByIdAsync(Guid id)
        {
            return _query.Include(p => p.Brand)
                         .Include(p => p.Category)
                         .Include(p => p.Unit)
                         .Include(p => p.ProductStatus)
                         .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Product?> GetByNameAsync(string name)
        {
            return await _query.FirstOrDefaultAsync(p => p.Name == name);
        }

        public Task<bool> IsExistAsync(Guid id)
        {
            return _query.AnyAsync(x => x.Id == id);
        }
    }
}
