using NET1814_MilkShop.Repositories.Data;
using NET1814_MilkShop.Repositories.Data.Entities;

namespace NET1814_MilkShop.Repositories.Repositories
{
    public interface ICartDetailRepository
    {
        IQueryable<CartDetail> GetCartDetailQuery();
        void Add(CartDetail cartDetail);
        void Update(CartDetail cartDetail);
        void Remove(CartDetail cartDetail);
        void RemoveRange(IEnumerable<CartDetail> cartDetails);
    }

    public class CartDetailRepository : Repository<CartDetail>, ICartDetailRepository
    {
        public CartDetailRepository(AppDbContext context)
            : base(context) { }

        public IQueryable<CartDetail> GetCartDetailQuery()
        {
            return _query;
        }

        public void RemoveRange(IEnumerable<CartDetail> cartDetails)
        {
            _context.Set<CartDetail>().RemoveRange(cartDetails);
        }
    }
}
