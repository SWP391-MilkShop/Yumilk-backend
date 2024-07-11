using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Models.ProductAttributeModels;

namespace NET1814_MilkShop.Services.Services.Interfaces;

public interface IProductAttributeService
{
    Task<ResponseModel> GetProductAttributesByIdAsync(int id);
    Task<ResponseModel> GetProductAttributesAsync(ProductAttributeQueryModel queryModel);
    Task<ResponseModel> AddProductAttributeAsync(CreateProductAttributeModel model);
    Task<ResponseModel> UpdateProductAttributeAsync(int id, UpdateProductAttributeModel model);
    Task<ResponseModel> DeleteProductAttributeAsync(int id);
}