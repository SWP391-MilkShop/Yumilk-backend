using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Models.PreorderModels;
using NET1814_MilkShop.Repositories.Models.ProductModels;

namespace NET1814_MilkShop.Services.Services.Interfaces;

public interface IProductService
{
    Task<ResponseModel> GetProductsAsync(ProductQueryModel queryModel);
    Task<ResponseModel> GetProductByIdAsync(Guid id);
    Task<ResponseModel> CreateProductAsync(CreateProductModel model);
    Task<ResponseModel> UpdateProductAsync(Guid id, UpdateProductModel model);

    /// <summary>
    ///     Delete product by id
    ///     <para>Also delete related preorder product if exists</para>
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<ResponseModel> DeleteProductAsync(Guid id);

    Task<ResponseModel> GetProductStatsAsync(ProductStatsQueryModel queryModel);
    Task<ResponseModel> UpdatePreorderProductAsync(Guid productId, UpdatePreorderProductModel model);
    Task<ResponseModel> GetSearchResultsAsync(ProductSearchModel queryModel);
}