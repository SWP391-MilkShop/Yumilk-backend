using Microsoft.EntityFrameworkCore;
using NET1814_MilkShop.Repositories.Data;
using NET1814_MilkShop.Repositories.Data.Entities;

namespace NET1814_MilkShop.Repositories.Repositories;

public interface ICategoryRepository
{
    IQueryable<Category> GetCategoriesQuery();
    Task<bool> IsExistAsync(string name);
    Task<Category?> GetById(int id);
    void Add(Category category);
    void Update(Category category);
}

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(AppDbContext context) : base(context)
    {
    }

    public IQueryable<Category> GetCategoriesQuery()
    {
        return _query;
    }

    /// <summary>
    ///     Check if category name is exist
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public Task<bool> IsExistAsync(string name)
    {
        return _query.AnyAsync(x => x.Name.Equals(name));
    }
}