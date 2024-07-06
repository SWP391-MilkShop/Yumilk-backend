using NET1814_MilkShop.Repositories.Data.Entities;

namespace NET1814_MilkShop.Repositories.Repositories.Interfaces;

public interface IProductAttributeValueRepository
{
    IQueryable<ProductAttributeValue> GetProductAttributeValue();
    void Add(ProductAttributeValue pav);
    void Update(ProductAttributeValue pav);
    void Remove(ProductAttributeValue pav);
    Task<Product?> GetProductById(Guid id);
    Task<ProductAttribute?> GetAttributeById(int aid);
    Task<ProductAttributeValue?> GetProdAttValue(Guid id, int aid);
}