using NET1814_MilkShop.Repositories.Data.Entities;

namespace NET1814_MilkShop.Repositories.Repositories.Interfaces;

public interface IBrandRepository
{
    IQueryable<Brand> GetBrandsQuery();
    void Add(Brand b);
    void Update(Brand b);
    void Delete(Brand b);
    Task<Brand?> GetByIdAsync(int id);
    Task<Brand?> GetBrandByName(int id, string name);
    Task<Brand?> GetBrandByName(string name);
}