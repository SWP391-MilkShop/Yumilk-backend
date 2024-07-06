using Microsoft.EntityFrameworkCore;
using NET1814_MilkShop.Repositories.Data;
using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Repositories.Interfaces;

namespace NET1814_MilkShop.Repositories.Repositories.Implementations;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(AppDbContext context)
        : base(context)
    {
    }

    public IQueryable<Category> GetCategoriesQuery()
    {
        return _query;
    }

    /// <summary>
    /// Check if category name exists under the same parent category
    /// </summary>
    /// <param name="name"></param>
    /// <param name="parentId"></param>
    /// <returns></returns>
    public Task<bool> IsExistAsync(string name, int? parentId)
    {
        return _query.AnyAsync(x => x.Name.Equals(name) && x.ParentId == parentId);
    }

    // public async Task<HashSet<int>> GetChildCategoryIds(int parentId)
    // {
    //     if(parentId == 0) return new HashSet<int>();
    //     var categoryIds = new HashSet<int> { parentId };
    //     var categories = await _query.ToListAsync();
    //     var childCategoryIds = GetChildCategoryIdsRecursive(categories, parentId);
    //     categoryIds.UnionWith(childCategoryIds);
    //     return categoryIds;
    // }
    //
    // private HashSet<int> GetChildCategoryIdsRecursive(List<Category> categories, int parentId)
    // {
    //     var childCategories = categories.Where(c => c.ParentId == parentId).ToList();
    //     var childCategoryIds = new HashSet<int>();
    //
    //     foreach (var childCategory in childCategories)
    //     {
    //         childCategoryIds.Add(childCategory.Id);
    //         var grandChildCategoryIds = GetChildCategoryIdsRecursive(categories, childCategory.Id);
    //         childCategoryIds.UnionWith(grandChildCategoryIds);
    //     }
    //
    //     return childCategoryIds;
    // }
    public async Task<HashSet<int>> GetChildCategoryIds(int parentId)
    {
        if (parentId == 0) return new HashSet<int>();

        // Load all categories once
        var allCategories = await _query.ToListAsync();

        // Convert to a lookup table for efficient child category lookup
        var categoriesLookup = allCategories.ToLookup(c => c.ParentId);

        // Initialize the result set with the parent ID
        var categoryIds = new HashSet<int> { parentId };

        // Recursively find child category IDs using the lookup table
        var childCategoryIds = GetChildCategoryIdsRecursive(categoriesLookup, parentId);

        // Union the results
        categoryIds.UnionWith(childCategoryIds);

        return categoryIds;
    }

    private HashSet<int> GetChildCategoryIdsRecursive(ILookup<int?, Category> categoriesLookup, int parentId)
    {
        var childCategoryIds = new HashSet<int>();
        // Directly access child categories using the parent ID
        var childCategories = categoriesLookup[parentId];
        foreach (var childCategory in childCategories)
        {
            childCategoryIds.Add(childCategory.Id);
            // Recursively find and add grandchild category IDs
            var grandChildCategoryIds = GetChildCategoryIdsRecursive(categoriesLookup, childCategory.Id);
            childCategoryIds.UnionWith(grandChildCategoryIds);
        }

        return childCategoryIds;
    }
}