using Microsoft.EntityFrameworkCore;
using NET1814_MilkShop.Repositories.Data;
using NET1814_MilkShop.Repositories.Data.Entities;

namespace NET1814_MilkShop.Repositories.Repositories
{
    public interface ICategoryRepository
    {
        IQueryable<Category> GetCategoriesQuery();
    }
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context) : base(context)
        {
        }

        public IQueryable<Category> GetCategoriesQuery()
        {
            return _context.Categories.AsNoTracking();
        }

    }
}
