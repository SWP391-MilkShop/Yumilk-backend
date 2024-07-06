using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Models.ProductReviewModels;

namespace NET1814_MilkShop.Services.Services.Interfaces;

public interface IProductReviewService
{
    Task<ResponseModel> GetProductReviewsByProductIdAsync(Guid productId, ProductReviewQueryModel queryModel);
    Task<ResponseModel> CreateProductReviewAsync(Guid productId, CreateReviewModel model);
    Task<ResponseModel> UpdateProductReviewAsync(int id, UpdateReviewModel model);
    Task<ResponseModel> DeleteProductReviewAsync(int id);
    Task<ResponseModel> GetProductReviewsAsync(ReviewQueryModel queryModel);
}