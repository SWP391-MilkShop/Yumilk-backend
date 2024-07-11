using NET1814_MilkShop.Repositories.Data.Entities;

namespace NET1814_MilkShop.Repositories.Repositories.Interfaces;

public interface IOrderLogRepository
{
    IQueryable<OrderLog> GetOrderLogQuery(Guid orderId);
    void Add(OrderLog orderLog);
}
