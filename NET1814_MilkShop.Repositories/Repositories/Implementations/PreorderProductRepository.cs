using Microsoft.EntityFrameworkCore;
using NET1814_MilkShop.Repositories.Data;
using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Repositories.Interfaces;

namespace NET1814_MilkShop.Repositories.Repositories.Implementations;

public class PreorderProductRepository : Repository<PreorderProduct>, IPreorderProductRepository
{
    public PreorderProductRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<PreorderProduct?> GetByProductIdAsync(Guid productId)
    {
        return await _query.Include(x => x.Product).ThenInclude(x => x.Unit)
            .FirstOrDefaultAsync(x => x.ProductId == productId);
    }
}