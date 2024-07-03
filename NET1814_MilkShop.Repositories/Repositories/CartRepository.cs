using System.Collections;
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
        void RemoveRange(IEnumerable<CartDetail> cart);
        Task<List<CartDetail>> GetCartDetails(int cartId);
        Task<Cart?> GetCartByCustomerId(Guid customerId);
    }

    public class CartRepository : Repository<Cart>, ICartRepository
    {
        public CartRepository(AppDbContext context)
            : base(context)
        {
        }

        public IQueryable<Cart> GetCartQuery()
        {
            return _query;
        }

        public override async Task<Cart?> GetByIdAsync(int id)
        {
            return await _query.Include(x => x.CartDetails).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Cart?> GetByCustomerIdAsync(Guid customerId, bool includeProduct)
        {
            if (includeProduct)
            {
                return await _query
                    .Include(x => x.CartDetails)
                    .ThenInclude(x => x.Product)
                    .ThenInclude(x => x.Unit)
                    .FirstOrDefaultAsync(x => x.CustomerId == customerId);
            }

            return await _query
                .Include(x => x.CartDetails)
                .FirstOrDefaultAsync(x => x.CustomerId == customerId);
        }

        public async Task<Cart?> GetCartByCustomerId(Guid customerId)
        {
            return await _context.Carts.Include(x => x.CartDetails).ThenInclude(x => x.Product)
                .ThenInclude(x => x.Unit).FirstOrDefaultAsync(x => x.CustomerId == customerId);
        }

        public void RemoveRange(IEnumerable<CartDetail> cartDetails)
        {
            _context.RemoveRange(cartDetails);
        }

        public async Task<List<CartDetail>> GetCartDetails(int cartId)
        {
            return await _context
                .CartDetails.Include(x => x.Product)
                .Where(x => x.CartId == cartId)
                .ToListAsync();
        }
    }
}