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

        public void AddRange(IEnumerable<OrderDetail> list)
        {
            _context.OrderDetails.AddRange(list);
        }

        public void Remove(OrderDetail orderDetail)
        {
            _context.OrderDetails.RemoveRange();
        }
    }
}
