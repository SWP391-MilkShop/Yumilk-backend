using Microsoft.EntityFrameworkCore;
using NET1814_MilkShop.Repositories.Data;
using NET1814_MilkShop.Repositories.Data.Entities;

namespace NET1814_MilkShop.Repositories.Repositories
{
    public interface IOrderRepository
    {
        IQueryable<Order> GetOrdersQuery();
        /// <summary>
        /// Get order by id include order details if includeDetails is true
        /// </summary>
        /// <param name="id"></param>
        /// <param name="includeDetails"></param>
        /// <returns></returns>
        Task<Order?> GetByIdAsync(Guid id, bool includeDetails);
        void Add(Order order);
        void AddRange(IEnumerable<OrderDetail> list);
        void Update(Order order);
        Task<Order?> GetByCodeAsync(int orderCode);
    }

    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(AppDbContext context)
            : base(context) { }

        public IQueryable<Order> GetOrdersQuery()
        {
            //return _context.Orders.Include(o => o.Status).Include(o => o.Customer).AsNoTracking();
            return _query.Include(o => o.Status).Include(o => o.Customer);
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
        public Task<Order?> GetByIdAsync(Guid id, bool includeDetails)
        {
            var query = includeDetails ? _query.Include(o => o.OrderDetails) : _query;
            return query.FirstOrDefaultAsync(o => o.Id == id);
        }
    }
}
