using Microsoft.EntityFrameworkCore;
using NET1814_MilkShop.Repositories.Data;
using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Repositories.Interfaces;

namespace NET1814_MilkShop.Repositories.Repositories.Implementations;

public class BrandRepository : Repository<Brand>, IBrandRepository
{
    public BrandRepository(AppDbContext context)
        : base(context)
    {
    }

    public IQueryable<Brand> GetBrandsQuery()
    {
        return _query;
    }

    public async Task<Brand?> GetBrandByName(int id, string name)
    {
        return await _query.FirstOrDefaultAsync(x => x.Name.Equals(name) && x.Id != id);
    }

    public async Task<Brand?> GetBrandByName(string name)
    {
        return await _query.FirstOrDefaultAsync(x => x.Name.Equals(name));
    }
}