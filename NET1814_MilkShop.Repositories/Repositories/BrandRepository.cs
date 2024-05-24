using Microsoft.EntityFrameworkCore;
using NET1814_MilkShop.Repositories.Data;
using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Repositories;

namespace NET1814_MilkShop.Repositories.Repositories
{
    public interface IBrandRepository
    {
        IQueryable<Brand> GetBrandsQuery();
        void Add(Brand b);
        void Update(Brand b);
        void Remove(Brand b);
    }

    public class BrandRepository : Repository<Brand>, IBrandRepository
    {
        public BrandRepository(AppDbContext context) : base(context)
        {
        }

        public IQueryable<Brand> GetBrandsQuery()
        {
            return _context.Brands.AsNoTracking();
        }
    }
}