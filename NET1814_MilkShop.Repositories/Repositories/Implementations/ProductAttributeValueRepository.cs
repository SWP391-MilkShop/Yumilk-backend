using Microsoft.EntityFrameworkCore;
using NET1814_MilkShop.Repositories.Data;
using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Repositories.Interfaces;

namespace NET1814_MilkShop.Repositories.Repositories.Implementations;

public class ProductAttributeValueRepository
    : Repository<ProductAttributeValue>,
        IProductAttributeValueRepository
{
    public ProductAttributeValueRepository(AppDbContext context)
        : base(context)
    {
    }

    public IQueryable<ProductAttributeValue> GetProductAttributeValue()
    {
        return _query.Include(x => x.Attribute);
    }

    public async Task<Product?> GetProductById(Guid id)
    {
        var entity = await _context.Products.FirstOrDefaultAsync(x =>
            x.Id == id && x.DeletedAt == null
        );
        if (entity != null)
        {
            return entity;
        }

        return null;
    }

    public async Task<ProductAttribute?> GetAttributeById(int aid)
    {
        var entity = await _context.ProductAttributes.FirstOrDefaultAsync(x =>
            x.Id == aid && x.DeletedAt == null
        );
        if (entity != null)
        {
            return entity;
        }

        return null;
    }

    public async Task<ProductAttributeValue?> GetProdAttValue(Guid id, int aid)
    {
        return await _context.ProductAttributeValues.FirstOrDefaultAsync(x =>
            x.ProductId == id && x.AttributeId == aid && x.DeletedAt == null
        );
    }
}