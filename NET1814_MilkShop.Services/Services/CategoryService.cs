using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Models.ProductModels;
using NET1814_MilkShop.Repositories.Repositories;
using NET1814_MilkShop.Repositories.UnitOfWork;

namespace NET1814_MilkShop.Services.Services
{
    public interface ICategoryService
    {
        Task<ResponseModel> GetCategoriesAsync(CategoryQueryModel queryModel);
        Task<ResponseModel> GetCategoryByIdAsync(Guid id);
        Task<ResponseModel> CreateCategoryAsync(CategoryModel model);
        Task<ResponseModel> UpdateCategoryAsync(CategoryModel model);
        Task<ResponseModel> DeleteCategoryAsync(Guid id);

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

        public Task<ResponseModel> CreateCategoryAsync(CategoryModel model)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel> DeleteCategoryAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel> GetCategoriesAsync(CategoryQueryModel queryModel)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel> GetCategoryByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<ResponseModel> UpdateCategoryAsync(CategoryModel model)
        {
            throw new NotImplementedException();
        }
    }
}
