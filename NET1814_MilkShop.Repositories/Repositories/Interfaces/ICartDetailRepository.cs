using NET1814_MilkShop.Repositories.Data.Entities;

namespace NET1814_MilkShop.Repositories.Repositories.Interfaces;

public interface ICartDetailRepository
{
    IQueryable<CartDetail> GetCartDetailQuery();
    void Add(CartDetail cartDetail);
    void Update(CartDetail cartDetail);
    void Remove(CartDetail cartDetail);
    void RemoveRange(IEnumerable<CartDetail> cartDetails);
}