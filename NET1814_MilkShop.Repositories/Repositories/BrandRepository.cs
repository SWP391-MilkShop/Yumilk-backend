using Microsoft.EntityFrameworkCore;
using NET1814_MilkShop.Repositories.Data;
using NET1814_MilkShop.Repositories.Data.Entities;

namespace NET1814_MilkShop.Repositories.Repositories
{
    public interface IBrandRepository
    {
        IQueryable<Brand> GetBrandsQuery();
        void Add(Brand b);
        void Update(Brand b);
        void Remove(Brand b);
        Task<Brand?> GetById(int id);
        Task<Brand?> GetBrandByName(string name);
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

        public async Task<Brand?> GetBrandByName(string name)
        {
            return await _context.Brands.AsNoTracking().FirstOrDefaultAsync(x => string.Equals(x.Name, name));
        }
    }
}