using Microsoft.EntityFrameworkCore;
using NET1814_MilkShop.Repositories.Data;
using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Models;

namespace NET1814_MilkShop.Repositories.Repositories
{
    public interface IProductRepository
    {
        IQueryable<Product> GetProductsQuery();
        Task<Product?> GetById(Guid id);
        Task<Product?> GetByNameAsync(string name);
        void Add(Product product);
        void Update(Product product);
        void Delete(Product product);
    }

    public sealed class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context)
            : base(context) { }
        /// <summary>
        /// Get all products with corresponding brand, category, unit
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// Get product by id with corresponding brand, category, unit
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override Task<Product?> GetById(Guid id)
        {
            return _query.Include(p => p.Brand)
                         .Include(p => p.Category)
                         .Include(p => p.Unit)
                         .Include(p => p.ProductStatus)
                         .FirstOrDefaultAsync(p => p.Id == id);
        }
        /// <summary>
        /// Get product by name for checking duplicate
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<Product?> GetByNameAsync(string name)
        {
            return await _query.FirstOrDefaultAsync(p => p.Name == name);
        }
    }
}
