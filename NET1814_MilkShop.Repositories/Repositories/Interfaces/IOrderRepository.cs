using NET1814_MilkShop.Repositories.Data.Entities;

namespace NET1814_MilkShop.Repositories.Repositories.Interfaces;

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
    Task<Order?> GetByIdNoIncludeAsync(Guid id);
    Task<List<Order>?> GetAllCodeAsync();
    Task<Order?> GetByOrderIdAsync(Guid orderId, bool include);
    Task<bool> IsExistOrderCode(int id);
    void Add(OrderDetail orderDetail);
    Task<bool> IsExistPreorderProductAsync(Guid orderId);
}