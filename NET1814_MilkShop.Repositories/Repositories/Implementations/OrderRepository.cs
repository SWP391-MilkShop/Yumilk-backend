using Microsoft.EntityFrameworkCore;
using NET1814_MilkShop.Repositories.CoreHelpers.Enum;
using NET1814_MilkShop.Repositories.Data;
using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Repositories.Interfaces;

namespace NET1814_MilkShop.Repositories.Repositories.Implementations;

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

    public async Task<Order?> GetByIdNoIncludeAsync(Guid id)
    {
        return await _query
            .FirstOrDefaultAsync(x => x.Id == id);
    }


    public async Task<List<Order>?> GetAllCodeAsync()
    {
        return await _query
            .Include(o => o.OrderDetails)
            .Where(x => x.OrderCode != null && (x.StatusId == (int)OrderStatusId.Pending))
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
        return order!.OrderDetails.Any(o => o.Product.StatusId == (int)ProductStatusId.Preorder);
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