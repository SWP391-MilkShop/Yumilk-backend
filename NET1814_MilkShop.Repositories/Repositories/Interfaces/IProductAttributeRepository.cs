using NET1814_MilkShop.Repositories.Data.Entities;

namespace NET1814_MilkShop.Repositories.Repositories.Interfaces;

public interface IProductAttributeRepository
{
    IQueryable<ProductAttribute> GetProductAttributes();
    void Add(ProductAttribute p);
    void Update(ProductAttribute p);
    void Remove(ProductAttribute p);
    Task<ProductAttribute?> GetProductAttributeByName(string name, int id);
    Task<ProductAttribute?> GetProductAttributeByName(string name);

    Task<ProductAttribute?> GetProductAttributeById(int id);
}