using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Models.BrandModels;

namespace NET1814_MilkShop.Services.Services.Interfaces;

public interface IBrandService
{
    Task<ResponseModel> GetBrandsAsync(BrandQueryModel queryModel);
    Task<ResponseModel> GetBrandByIdAsync(int id);
    Task<ResponseModel> CreateBrandAsync(CreateBrandModel model);
    Task<ResponseModel> UpdateBrandAsync(int id, UpdateBrandModel model);
    Task<ResponseModel> DeleteBrandAsync(int id);
}