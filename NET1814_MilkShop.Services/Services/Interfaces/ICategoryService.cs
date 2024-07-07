using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Models.CategoryModels;

namespace NET1814_MilkShop.Services.Services.Interfaces;

public interface ICategoryService
{
    Task<ResponseModel> GetCategoriesAsync(CategoryQueryModel queryModel);
    Task<ResponseModel> GetCategoryByIdAsync(int id);
    Task<ResponseModel> CreateCategoryAsync(CreateCategoryModel model);
    Task<ResponseModel> UpdateCategoryAsync(int id, UpdateCategoryModel model);
    Task<ResponseModel> DeleteCategoryAsync(int id);
}