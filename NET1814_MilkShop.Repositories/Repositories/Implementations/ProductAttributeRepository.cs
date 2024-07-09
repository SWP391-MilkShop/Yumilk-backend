using Microsoft.EntityFrameworkCore;
using NET1814_MilkShop.Repositories.Data;
using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Repositories.Interfaces;

namespace NET1814_MilkShop.Repositories.Repositories.Implementations;

public class ProductAttributeRepository
    : Repository<ProductAttribute>,
        IProductAttributeRepository
{
    public ProductAttributeRepository(AppDbContext context)
        : base(context)
    {
    }

    public IQueryable<ProductAttribute> GetProductAttributes()
    {
        return _query;
    }

    public async Task<ProductAttribute?> GetProductAttributeByName(string name, int id)
    {
        return await _query.FirstOrDefaultAsync(x => x.Name.Equals(name) && x.Id != id);
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