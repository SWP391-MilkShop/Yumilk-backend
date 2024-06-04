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
        void Delete(Brand b);
        Task<Brand?> GetByIdAsync(int id);
        Task<Brand?> GetBrandByName(string name);
    }

    public class BrandRepository : Repository<Brand>, IBrandRepository
    {
        public BrandRepository(AppDbContext context) : base(context)
        {
        }

        public IQueryable<Brand> GetBrandsQuery()
        {
            return _query;
        }

        public async Task<Brand?> GetBrandByName(string name)
        {
            return await _query.FirstOrDefaultAsync(x => x.Name.Equals(name));
        }
    }
}