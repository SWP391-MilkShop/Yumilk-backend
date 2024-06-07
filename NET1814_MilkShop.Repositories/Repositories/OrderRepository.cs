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
        Task<Order?> GetByCodeAsync(int orderCode);
        Task<Order?> GetByIdNoInlcudeAsync(Guid id);
        Task<List<Order>?> GetAllCodeAsync();
        void Update(Order order);
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

        public async Task<Order?> GetByIdNoInlcudeAsync(Guid id)
        {
            return await _query.FirstOrDefaultAsync(x => x.Id == id);
        }
        

        public async Task<List<Order>?> GetAllCodeAsync()
        {
            return await _query
                .Include(o => o.OrderDetails)
                .ThenInclude(o => o.Product)
                .Where(x=>x.OrderCode != null)
                .ToListAsync();
        }
    }
}
