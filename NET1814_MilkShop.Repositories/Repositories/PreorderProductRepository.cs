using Microsoft.EntityFrameworkCore;
using NET1814_MilkShop.Repositories.Data;
using NET1814_MilkShop.Repositories.Data.Entities;

namespace NET1814_MilkShop.Repositories.Repositories
{
    public interface IPreorderProductRepository
    {
        Task<PreorderProduct?> GetByIdAsync(Guid id);
        void Add(PreorderProduct preorderProduct);
        void Update(PreorderProduct preorderProduct);
        void Delete(PreorderProduct preorderProduct);
        Task<PreorderProduct?> GetByProductIdAsync(Guid productId);
    }

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
}