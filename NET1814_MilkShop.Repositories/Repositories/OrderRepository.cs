using Microsoft.EntityFrameworkCore;
using NET1814_MilkShop.Repositories.Data;
using NET1814_MilkShop.Repositories.Data.Entities;

namespace NET1814_MilkShop.Repositories.Repositories
{
    public interface IOrderRepository
    {
        IQueryable<Order> GetOrdersQuery();

        void Add(Order order);
        void AddRange(IEnumerable<OrderDetail> list);
        void RemoveRange(IEnumerable<CartDetail> list);
        Task<Cart?> GetCartByUserId(Guid userId);
        Task<List<CartDetail>> GetCartDetails(int cartId);
        Task<Order?> GetByCodeAsync(int orderCode);

    }

    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(AppDbContext context)
            : base(context)
        {
        }

        public IQueryable<Order> GetOrdersQuery()
        {
            //return _context.Orders.Include(o => o.Status).Include(o => o.Customer).AsNoTracking();
            return _query.Include(o => o.Status)
                .Include(o => o.Customer);
        }

        public void AddRange(IEnumerable<OrderDetail> list)
        {
            _context.OrderDetails.AddRange(list);
        }

        public void RemoveRange(IEnumerable<CartDetail> list)
        {
            _context.CartDetails.RemoveRange(list);
        }

        public async Task<Cart?> GetCartByUserId(Guid userId)
        {
            return await _context.Carts.FirstOrDefaultAsync(c => c.CustomerId == userId);
        }

        public async Task<List<CartDetail>> GetCartDetails(int cartId)
        {
            return await _context.CartDetails.Include(x => x.Product).Where(x => x.CartId == cartId).ToListAsync();
        }

        public async Task<Order?> GetByCodeAsync(int orderCode)
        {
            return await _query.Include(o => o.Status)
                               .Include(o => o.Customer)
                               .ThenInclude(o => o.User)
                               .Include(o => o.OrderDetails)
                               .ThenInclude(o => o.Product)
                               .FirstOrDefaultAsync(o => o.OrderCode == orderCode);
        }
    }
}