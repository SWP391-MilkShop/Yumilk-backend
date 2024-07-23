using Microsoft.EntityFrameworkCore;
using NET1814_MilkShop.Repositories.CoreHelpers.Enum;
using NET1814_MilkShop.Repositories.Data;
using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Repositories.Interfaces;

namespace NET1814_MilkShop.Repositories.Repositories.Implementations;

public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
{
    public OrderDetailRepository(AppDbContext context)
        : base(context)
    {
    }

    public IQueryable<OrderDetail> GetOrderDetailQuery()
    {
        return _query;
    }

    public Task<bool> CheckActiveOrderProduct(Guid productId)
    {
        return _query.Include(od => od.Order)
            .AnyAsync(od => od.ProductId == productId
                            && (od.Order.StatusId != (int)OrderStatusId.Delivered
                                && od.Order.StatusId != (int)OrderStatusId.Cancelled));
    }

    public Task<List<Order>> GetActiveOrdersByProductId(Guid productId)
    {
        return _query.Include(od => od.Order)
            .Where(od => od.ProductId == productId
                         && (od.Order.StatusId != (int)OrderStatusId.Delivered
                             && od.Order.StatusId != (int)OrderStatusId.Cancelled))
            .Select(od => od.Order)
            .ToListAsync();
    }
}