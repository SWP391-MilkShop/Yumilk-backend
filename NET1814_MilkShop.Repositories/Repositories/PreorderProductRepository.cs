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
    }
    public class PreorderProductRepository : Repository<PreorderProduct>, IPreorderProductRepository
    {
        public PreorderProductRepository(AppDbContext context) : base(context)
        {
        }
    }
}
