using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Models.ProductAttributeValueModels;

namespace NET1814_MilkShop.Services.Services.Interfaces;

public interface IProductAttributeValueService
{
    Task<ResponseModel> GetProductAttributeValue(
        Guid id,
        ProductAttributeValueQueryModel queryModel
    );

    Task<ResponseModel> AddProductAttributeValue(Guid pid, int aid, CreateUpdatePavModel model);
    Task<ResponseModel> UpdateProductAttributeValue(Guid pid, int aid, CreateUpdatePavModel model);
    Task<ResponseModel> DeleteProductAttributeValue(Guid pid, int aid);
}