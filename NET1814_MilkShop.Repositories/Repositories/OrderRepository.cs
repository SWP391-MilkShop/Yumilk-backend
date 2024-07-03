using Microsoft.EntityFrameworkCore;
using NET1814_MilkShop.Repositories.CoreHelpers.Enum;
using NET1814_MilkShop.Repositories.Data;
using NET1814_MilkShop.Repositories.Data.Entities;

namespace NET1814_MilkShop.Repositories.Repositories
{
    public interface IOrderRepository
    {
        /// <summary>
        /// Get order query with status and customer and order details
        /// </summary>
        /// <returns></returns>
        IQueryable<Order> GetOrderQuery();

        /// <summary>
        /// Get order query with status
        /// </summary>
        /// <returns></returns>
        IQueryable<Order> GetOrderQueryWithStatus();

        IQueryable<Order> GetOrderHistory(Guid customerId);

        /// <summary>
        /// Get order by id include order details if includeDetails is true
        /// </summary>
        /// <param name="id"></param>
        /// <param name="includeDetails"></param>
        /// <returns></returns>
        Task<Order?> GetByIdAsync(Guid id, bool includeDetails);

        void Add(Order order);
        void Update(Order order);
        void AddRange(IEnumerable<OrderDetail> list);
        Task<Order?> GetByCodeAsync(int orderCode);
        Task<Order?> GetByIdNoInlcudeAsync(Guid id);
        Task<List<Order>?> GetAllCodeAsync();
        Task<Order?> GetByOrderIdAsync(Guid orderId, bool include);
        Task<bool> IsExistOrderCode(int id);
        void Add(OrderDetail orderDetail);
        Task<bool> IsExistPreorderProductAsync(Guid orderId);
    }

    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(AppDbContext context)
            : base(context)
        {
        }

        public IQueryable<Order> GetOrderQuery()
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

        public async Task<Order?> GetByIdNoInlcudeAsync(Guid id)
        {
            return await _query
                .FirstOrDefaultAsync(x => x.Id == id);
        }


        public async Task<List<Order>?> GetAllCodeAsync()
        {
            return await _query
                .Include(o => o.OrderDetails)
                .Where(x => x.OrderCode != null)
                .ToListAsync();
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

        public async Task<bool> IsExistOrderCode(int id)
        {
            return await _query.AnyAsync(x => x.OrderCode == id);
        }

        public void Add(OrderDetail orderDetail)
        {
            _context.OrderDetails.Add(orderDetail);
        }

        public async Task<bool> IsExistPreorderProductAsync(Guid orderId)
        {
            var order = await _query.Include(x => x.OrderDetails).ThenInclude(k => k.Product)
                .FirstOrDefaultAsync(x => x.Id == orderId);
            return order!.OrderDetails.Any(o => o.Product.StatusId == (int)ProductStatusId.PREORDER);
        }

        public Task<Order?> GetByIdAsync(Guid id, bool includeDetails)
        {
            var query = includeDetails ? _query.Include(o => o.OrderDetails) : _query;
            return query.FirstOrDefaultAsync(o => o.Id == id);
        }

        public IQueryable<Order> GetOrderQueryWithStatus()
        {
            return _query.Include(o => o.Status);
        }
    }
}