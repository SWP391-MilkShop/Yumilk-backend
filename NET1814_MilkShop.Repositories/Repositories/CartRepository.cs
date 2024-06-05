using Microsoft.EntityFrameworkCore;
using NET1814_MilkShop.Repositories.Data;
using NET1814_MilkShop.Repositories.Data.Entities;

namespace NET1814_MilkShop.Repositories.Repositories
{
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
    }
    public class CartRepository : Repository<Cart>, ICartRepository
    {
        public CartRepository(AppDbContext context) : base(context)
        {
        }

        public IQueryable<Cart> GetCartQuery()
        {
            return _query;
        }
        public async override Task<Cart?> GetByIdAsync(int id)
        {
            return await _query.Include(x => x.CartDetails).FirstOrDefaultAsync(x => x.Id == id);
        }

        public Task<Cart?> GetByCustomerIdAsync(Guid customerId, bool includeProduct)
        {
            if (includeProduct)
            {
                return _query.Include(x => x.CartDetails).ThenInclude(x => x.Product).FirstOrDefaultAsync(x => x.CustomerId == customerId);
            }
            return _query.Include(x => x.CartDetails).FirstOrDefaultAsync(x => x.CustomerId == customerId);
        }
    }
}
