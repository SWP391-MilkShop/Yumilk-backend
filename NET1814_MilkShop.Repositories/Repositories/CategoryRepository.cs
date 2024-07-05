using Microsoft.EntityFrameworkCore;
using NET1814_MilkShop.Repositories.Data;
using NET1814_MilkShop.Repositories.Data.Entities;

namespace NET1814_MilkShop.Repositories.Repositories
{
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

        void Add(Category category);
        void Update(Category category);
    }

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
        public HashSet<int> GetChildCategoryIds(int parentId, List<Category> categories)
        {
            if (parentId == 0) return new HashSet<int>();

            // Convert to a lookup table for efficient child category lookup
            var categoriesLookup = categories.ToLookup(c => c.ParentId);

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
}