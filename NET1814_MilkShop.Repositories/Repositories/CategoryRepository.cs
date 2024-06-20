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
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        Task<HashSet<int>> GetChildCategoryIds(int parentId);
        void Add(Category category);
        void Update(Category category);
    }

    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context)
            : base(context) { }

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
        public async Task<HashSet<int>> GetChildCategoryIds(int parentId)
        {
            if(parentId == 0) return new HashSet<int>();
            var categoryIds = new HashSet<int> { parentId };
            var categories = await _query.ToListAsync();
            var childCategoryIds = GetChildCategoryIdsRecursive(categories, parentId);
            categoryIds.UnionWith(childCategoryIds);
            return categoryIds;
        }

        private HashSet<int> GetChildCategoryIdsRecursive(List<Category> categories, int parentId)
        {
            var childCategories = categories.Where(c => c.ParentId == parentId).ToList();
            var childCategoryIds = new HashSet<int>();

            foreach (var childCategory in childCategories)
            {
                childCategoryIds.Add(childCategory.Id);
                var grandChildCategoryIds = GetChildCategoryIdsRecursive(categories, childCategory.Id);
                childCategoryIds.UnionWith(grandChildCategoryIds);
            }

            return childCategoryIds;
        }
    }
}
