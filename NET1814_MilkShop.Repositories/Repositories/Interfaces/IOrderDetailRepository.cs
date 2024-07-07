using NET1814_MilkShop.Repositories.Data.Entities;

namespace NET1814_MilkShop.Repositories.Repositories.Interfaces;

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