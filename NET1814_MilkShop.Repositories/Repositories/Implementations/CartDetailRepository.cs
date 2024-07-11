using NET1814_MilkShop.Repositories.Data;
using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Repositories.Interfaces;

namespace NET1814_MilkShop.Repositories.Repositories.Implementations;

public class CartDetailRepository : Repository<CartDetail>, ICartDetailRepository
{
    public CartDetailRepository(AppDbContext context)
        : base(context)
    {
    }

    public IQueryable<CartDetail> GetCartDetailQuery()
    {
        return _query;
    }

    public void RemoveRange(IEnumerable<CartDetail> cartDetails)
    {
        _context.Set<CartDetail>().RemoveRange(cartDetails);
    }
}