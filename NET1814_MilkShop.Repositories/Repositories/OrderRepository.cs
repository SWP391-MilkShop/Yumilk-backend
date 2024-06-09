using Microsoft.EntityFrameworkCore;
using NET1814_MilkShop.Repositories.Data;
using NET1814_MilkShop.Repositories.Data.Entities;

namespace NET1814_MilkShop.Repositories.Repositories
{
    public interface IOrderRepository
    {
        IQueryable<Order> GetOrdersQuery();
        IQueryable<Order> GetOrderHistory(Guid customerId);
        void Add(Order order);
        void Update(Order order);
        void AddRange(IEnumerable<OrderDetail> list);
        Task<Order?> GetByCodeAsync(int orderCode);
        Task<Order?> GetByOrderIdAsync(Guid orderId, bool include);
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
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails);
        }

        public IQueryable<Order> GetOrderHistory(Guid customerId)
        {
            return _query.Include(o => o.OrderDetails).ThenInclude(o => o.Product)
                .Include(o => o.Status)
                .Where(x => x.CustomerId == customerId);
        }

        public void AddRange(IEnumerable<OrderDetail> list)
        {
            _context.OrderDetails.AddRange(list);
        }

        public async Task<Order?> GetByCodeAsync(int orderCode)
        {
            return await _query
                .Include(o => o.Status)
                .Include(o => o.Customer)
                .ThenInclude(o => o.User)
                .Include(o => o.OrderDetails)
                .ThenInclude(o => o.Product)
                .FirstOrDefaultAsync(o => o.OrderCode == orderCode);
        }

        public async Task<Order?> GetByOrderIdAsync(Guid orderId, bool include)
        {
            return include
                    ? await _query
                        .Include(o => o.Status)
                        .Include(o => o.OrderDetails)
                        .ThenInclude(o => o.Product).FirstOrDefaultAsync(o => o.Id == orderId)
                    : await _query.Include(o => o.OrderDetails).ThenInclude(o => o.Product)
                        .FirstOrDefaultAsync(o => o.Id == orderId)
                ;
        }
    }
}