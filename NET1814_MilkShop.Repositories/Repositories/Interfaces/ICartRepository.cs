using NET1814_MilkShop.Repositories.Data.Entities;

namespace NET1814_MilkShop.Repositories.Repositories.Interfaces;

public interface ICartRepository
{
    IQueryable<Cart> GetCartQuery();

    /// <summary>
    /// Get by cart id including CartDetails
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<Cart?> GetByIdAsync(int id);

    /// <summary>
    /// Get by customer id including CartDetails and include Product if includeProduct is true
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="includeProduct"></param>
    /// <returns></returns>
    Task<Cart?> GetByCustomerIdAsync(Guid customerId, bool includeProduct);

    void Add(Cart cart);
    void Remove(Cart cart);
    Task<List<CartDetail>> GetCartDetails(int cartId);
    Task<Cart?> GetCartByCustomerId(Guid customerId);
}