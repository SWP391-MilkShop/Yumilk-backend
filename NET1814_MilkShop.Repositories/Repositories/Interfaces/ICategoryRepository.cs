using NET1814_MilkShop.Repositories.Data.Entities;

namespace NET1814_MilkShop.Repositories.Repositories.Interfaces;

public interface ICategoryRepository
{
    IQueryable<Category> GetCategoriesQuery();
    Task<bool> IsExistAsync(string name, int? parentId);
    Task<Category?> GetByIdAsync(int id);

    /// <summary>
    /// This method is used to get all child category ids of a parent category (include parent category id)
    /// Must pass in a list of categories to avoid multiple queries to the database
    /// </summary>
    /// <param name="parentId"></param>
    /// <param name="categories"></param>
    /// <returns></returns>
    HashSet<int> GetChildCategoryIds(int parentId, List<Category> categories);
    /// <summary>
    /// This method is used to check if a category is an ancestor of another category
    /// </summary>
    /// <param name="childId"></param>
    /// <param name="ancestorId"></param>
    /// <param name="categories"></param>
    /// <returns></returns>
    bool IsAncestorOf(int childId, int ancestorId, List<Category> categories);
    void Add(Category category);
    void Update(Category category);
}