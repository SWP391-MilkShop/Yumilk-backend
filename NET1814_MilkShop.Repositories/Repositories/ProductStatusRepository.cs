using NET1814_MilkShop.Repositories.Data;
using NET1814_MilkShop.Repositories.Data.Entities;

namespace NET1814_MilkShop.Repositories.Repositories
{
    public interface IProductStatusRepository
    {
        Task<ProductStatus?> GetByIdAsync(int id);
    }

    public class ProductStatusRepository : Repository<ProductStatus>, IProductStatusRepository
    {
        public ProductStatusRepository(AppDbContext context)
            : base(context) { }
    }
}
