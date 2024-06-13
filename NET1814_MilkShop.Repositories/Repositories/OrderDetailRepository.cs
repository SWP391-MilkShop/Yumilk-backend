using Microsoft.EntityFrameworkCore;
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
    }
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
    {
        public OrderDetailRepository(AppDbContext context)
            : base(context)
        {
        }
        public IQueryable<OrderDetail> GetOrderDetailQuery()
        {
            return _query.Include(x => x.Order);
        }
    }
}
