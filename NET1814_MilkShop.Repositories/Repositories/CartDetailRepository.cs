using Microsoft.EntityFrameworkCore;
using NET1814_MilkShop.Repositories.Data;
using NET1814_MilkShop.Repositories.Data.Entities;

namespace NET1814_MilkShop.Repositories.Repositories
{
    public interface ICartDetailRepository
    {
        IQueryable<CartDetail> GetCartDetailQuery();
        Task<CartDetail?> GetByCartIdAndProductId(int cartId, Guid productId);
        void Add(CartDetail cartDetail);
        void Update(CartDetail cartDetail);
        void Remove(CartDetail cartDetail);
    }
    public class CartDetailRepository : Repository<CartDetail>, ICartDetailRepository
    {
        public CartDetailRepository(AppDbContext context) : base(context)
        {
        }
        public IQueryable<CartDetail> GetCartDetailQuery()
        {
            return _query;
        }
        public Task<CartDetail?> GetByCartIdAndProductId(int cartId, Guid productId)
        {
            return _query.FirstOrDefaultAsync(x => x.CartId == cartId && x.ProductId == productId);
        }
    }
}
