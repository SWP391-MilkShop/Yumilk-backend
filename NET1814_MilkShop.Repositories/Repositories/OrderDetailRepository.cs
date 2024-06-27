using Microsoft.EntityFrameworkCore;
using NET1814_MilkShop.Repositories.CoreHelpers.Enum;
using NET1814_MilkShop.Repositories.Data;
using NET1814_MilkShop.Repositories.Data.Entities;

namespace NET1814_MilkShop.Repositories.Repositories
{
    public interface IOrderDetailRepository
    {
        /// <summary>
        /// Get order detail query
        /// </summary>
        /// <returns></returns>
        IQueryable<OrderDetail> GetOrderDetailQuery();
        /// <summary>
        /// Check if a product is in an active order 
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        Task<bool> CheckActiveOrderProduct(Guid productId);
    }

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
                                && (od.Order.StatusId != (int)OrderStatusId.DELIVERED
                                && od.Order.StatusId != (int)OrderStatusId.CANCELLED));
        }
    }
}