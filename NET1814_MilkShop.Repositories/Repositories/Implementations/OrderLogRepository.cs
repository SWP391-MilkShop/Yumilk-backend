using NET1814_MilkShop.Repositories.Data;
using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Repositories.Interfaces;

namespace NET1814_MilkShop.Repositories.Repositories.Implementations;

public class OrderLogRepository : Repository<OrderLog>, IOrderLogRepository
{
    public OrderLogRepository(AppDbContext context)
        : base(context)
    {
        
    }

    public IQueryable<OrderLog> GetOrderLogQuery(Guid orderId)
    {
        return _query.Where(x=> x.OrderId == orderId).OrderByDescending(x=> x.CreatedAt);
    }
}