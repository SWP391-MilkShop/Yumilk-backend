using Microsoft.EntityFrameworkCore;
using NET1814_MilkShop.Repositories.Data;
using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Repositories.Interfaces;

namespace NET1814_MilkShop.Repositories.Repositories.Implementations;

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


    public async Task<List<CartDetail>> GetCartDetails(int cartId)
    {
        return await _context
            .CartDetails.Include(x => x.Product)
            .Where(x => x.CartId == cartId)
            .ToListAsync();
    }
}