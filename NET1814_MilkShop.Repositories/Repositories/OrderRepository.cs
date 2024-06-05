using Microsoft.EntityFrameworkCore;
using NET1814_MilkShop.Repositories.Data;
using NET1814_MilkShop.Repositories.Data.Entities;

namespace NET1814_MilkShop.Repositories.Repositories
{
    public interface IOrderRepository
    {
        IQueryable<Order> GetOrdersQuery();
        Task<Order?> GetByCodeAsync(int orderCode);
    }

    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(AppDbContext context)
            : base(context) { }

        public IQueryable<Order> GetOrdersQuery()
        {
            //return _context.Orders.Include(o => o.Status).Include(o => o.Customer).AsNoTracking();
            return _query.Include(o => o.Status)
                         .Include(o => o.Customer);
        }
        
        public async Task<Order?> GetByCodeAsync(int orderCode)
        {
            return await _query.Include(o => o.Status)
                               .Include(o => o.Customer)
                               .ThenInclude(o=>o.User)
                               .Include(o => o.OrderDetails)
                               .ThenInclude(o=>o.Product)
                               .FirstOrDefaultAsync(o => o.OrderCode == orderCode);
        }
    }
}
