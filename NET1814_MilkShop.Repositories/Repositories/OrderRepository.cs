using Microsoft.EntityFrameworkCore;
using NET1814_MilkShop.Repositories.Data;
using NET1814_MilkShop.Repositories.Data.Entities;

namespace NET1814_MilkShop.Repositories.Repositories;

public interface IOrderRepository
{
    IQueryable<Order> GetOrdersQuery();
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
}