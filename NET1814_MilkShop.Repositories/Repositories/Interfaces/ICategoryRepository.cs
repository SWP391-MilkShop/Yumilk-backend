using NET1814_MilkShop.Repositories.Data.Entities;

namespace NET1814_MilkShop.Repositories.Repositories.Interfaces;

public interface ICategoryRepository
{
    IQueryable<Category> GetCategoriesQuery();
    Task<bool> IsExistAsync(string name, int? parentId);
    Task<Category?> GetByIdAsync(int id);

    /// <summary>
    /// This method is used to get all child category ids of a parent category (include parent category id)
    /// </summary>
    /// <param name="parentId"></param>
    /// <returns></returns>
    Task<HashSet<int>> GetChildCategoryIds(int parentId);

    void Add(Category category);
    void Update(Category category);
}