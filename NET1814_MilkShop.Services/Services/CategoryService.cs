using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Repositories;
using NET1814_MilkShop.Repositories.UnitOfWork;
using NET1814_MilkShop.Services.CoreHelpers;
using System.Linq.Expressions;
using NET1814_MilkShop.Repositories.Models.CategoryModels;

namespace NET1814_MilkShop.Services.Services
{
    public interface ICategoryService
    {
        Task<ResponseModel> GetCategoriesAsync(CategoryQueryModel queryModel);
        Task<ResponseModel> GetCategoryByIdAsync(int id);
        Task<ResponseModel> CreateCategoryAsync(CreateCategoryModel model);
        Task<ResponseModel> UpdateCategoryAsync(int id, UpdateCategoryModel model);
        Task<ResponseModel> DeleteCategoryAsync(int id);

    }
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CategoryService(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
        {
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseModel> CreateCategoryAsync(CreateCategoryModel model)
        {
            var isExist = await _categoryRepository.IsExistAsync(model.Name);
            if (isExist)
            {
                return new ResponseModel
                {
                    Status = "Error",
                    Message = "Category already exists"
                };
            }
            var category = new Category
            {
                Name = model.Name,
                Description = model.Description,
                IsActive = true
            };
            _categoryRepository.Add(category);
            var result = await _unitOfWork.SaveChangesAsync();
            if (result > 0)
            {
                return new ResponseModel
                {
                    Status = "Success",
                    Message = "Create category successfully"
                };
            }
            return new ResponseModel
            {
                Status = "Error",
                Message = "Create category failed"
            };
        }

        public async Task<ResponseModel> DeleteCategoryAsync(int id)
        {
            var category = await _categoryRepository.GetById(id);
            if (category == null)
            {
                return new ResponseModel
                {
                    Status = "Error",
                    Message = "Category not found"
                };
            }
            category.IsActive = false;
            category.DeletedAt = DateTime.Now;
            _categoryRepository.Update(category);
            var result = await _unitOfWork.SaveChangesAsync();
            if (result > 0)
            {
                return new ResponseModel
                {
                    Status = "Success",
                    Message = "Delete category successfully"
                };
            }
            return new ResponseModel
            {
                Status = "Error",
                Message = "Delete category failed"
            };
        }

        public async Task<ResponseModel> GetCategoriesAsync(CategoryQueryModel queryModel)
        {
            var query = _categoryRepository.GetCategoriesQuery();
            query = query.Where(p =>
            p.IsActive == queryModel.IsActive
            && (string.IsNullOrEmpty(queryModel.SearchTerm) || p.Name.Contains(queryModel.SearchTerm)));
            if ("desc".Equals(queryModel.SortOrder?.ToLower()))
            {
                query = query.OrderByDescending(GetSortProperty(queryModel));
            }
            else
            {
                query = query.OrderBy(GetSortProperty(queryModel));
            }
            var categoryModelQuery = query.Select(c => new CategoryModel
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                IsActive = c.IsActive
            });
            var categories = await PagedList<CategoryModel>.CreateAsync(
                categoryModelQuery,
                queryModel.Page,
                queryModel.PageSize
            );
            return new ResponseModel
            {
                Data = categories,
                Message = categories.TotalCount > 0 ? "Get products successfully" : "No products found",
                Status = "Success"
            };
        }
        private static Expression<Func<Category, object>> GetSortProperty(
            CategoryQueryModel queryModel
        ) =>
            queryModel.SortColumn?.ToLower() switch
            {
                "name" => category => category.Name,
                _ => category => category.Id,
            };
        public async Task<ResponseModel> GetCategoryByIdAsync(int id)
        {
            var category = await _categoryRepository.GetById(id);
            if (category == null || !category.IsActive)
            {
                return new ResponseModel
                {
                    Status = "Error",
                    Message = "Category not found"
                };
            }
            var categoryModel = new CategoryModel
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                IsActive = category.IsActive
            };
            return new ResponseModel
            {
                Data = categoryModel,
                Message = "Get category successfully",
                Status = "Success"
            };
        }

        public async Task<ResponseModel> UpdateCategoryAsync(int id, UpdateCategoryModel model)
        {
            
            var existingCategory = await _categoryRepository.GetById(id);
            if (existingCategory == null)
            {
                return new ResponseModel
                {
                    Status = "Error",
                    Message = "Category not found"
                };
            }
            if (!string.IsNullOrEmpty(model.Name))
            {
                // Check if category name is changed
                if (!string.Equals(model.Name, existingCategory.Name, StringComparison.OrdinalIgnoreCase))
                {
                    var isExist = await _categoryRepository.IsExistAsync(model.Name);
                    if (isExist)
                    {
                        return new ResponseModel
                        {
                            Status = "Error",
                            Message = "Category already exists"
                        };
                    }
                }
                existingCategory.Name = model.Name;
            }
            existingCategory.Description = string.IsNullOrEmpty(model.Description) ? existingCategory.Description : model.Description;
            existingCategory.IsActive = model.IsActive;
            _categoryRepository.Update(existingCategory);
            var result = await _unitOfWork.SaveChangesAsync();
            if (result > 0)
            {
                return new ResponseModel
                {
                    Status = "Success",
                    Message = "Update category successfully"
                };
            }
            return new ResponseModel
            {
                Status = "Error",
                Message = "Update category failed"
            };
        }
    }
}
