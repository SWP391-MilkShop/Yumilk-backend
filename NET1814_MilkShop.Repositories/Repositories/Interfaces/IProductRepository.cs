using NET1814_MilkShop.Repositories.Data.Entities;

namespace NET1814_MilkShop.Repositories.Repositories.Interfaces;

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
    Task<Product?> GetByIdIncludePreorder(Guid id);
    Task<bool> IsExistIdByBrand(int id);
    Task<bool> IsExistIdByUnit(int id);
    Task<bool> IsExistIdByCategory(int id);
}