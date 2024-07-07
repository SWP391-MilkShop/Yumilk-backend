using NET1814_MilkShop.Repositories.Data.Entities;

namespace NET1814_MilkShop.Repositories.Repositories.Interfaces;

public interface IPreorderProductRepository
{
    Task<PreorderProduct?> GetByIdAsync(Guid id);
    void Add(PreorderProduct preorderProduct);
    void Update(PreorderProduct preorderProduct);
    void Delete(PreorderProduct preorderProduct);
    Task<PreorderProduct?> GetByProductIdAsync(Guid productId);
}