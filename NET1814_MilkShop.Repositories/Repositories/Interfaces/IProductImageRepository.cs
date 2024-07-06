using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Models.ProductModels;

namespace NET1814_MilkShop.Repositories.Repositories.Interfaces;

public interface IProductImageRepository
{
    /// <summary>
    /// Return list of product images by product id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="isActive"></param>
    /// <returns></returns>
    Task<List<ProductImageModel>> GetByProductIdAsync(Guid id, bool? isActive);

    Task<ProductImage?> GetByIdAsync(int id);
    void Add(ProductImage productImage);
    void Update(ProductImage productImage);

    //void Delete(ProductImage productImage);
    void Remove(ProductImage productImage);
}