using Microsoft.EntityFrameworkCore;
using NET1814_MilkShop.Repositories.Data;
using NET1814_MilkShop.Repositories.Data.Entities;

namespace NET1814_MilkShop.Repositories.Repositories
{
    public interface IProductAttributeRepository
    {
        IQueryable<ProductAttribute> GetProductAttributes();
        void Add(ProductAttribute p);
        void Update(ProductAttribute p);
        void Remove(ProductAttribute p);
        Task<ProductAttribute?> GetProductAttributeByName(string name);
        Task<ProductAttribute?> GetProductAttributeById(int id);
    }

    public class ProductAttributeRepository : Repository<ProductAttribute>, IProductAttributeRepository
    {
        public ProductAttributeRepository(AppDbContext context) : base(context)
        {
        }

        public IQueryable<ProductAttribute> GetProductAttributes()
        {
            return _query;
        }

        public async Task<ProductAttribute?> GetProductAttributeByName(string name)
        {
            return await _query.FirstOrDefaultAsync(x => x.Name.Equals(name));
        }

        public async Task<ProductAttribute?> GetProductAttributeById(int id)
        {
            return await _query.FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}