using Microsoft.AspNetCore.Http;
using NET1814_MilkShop.Repositories.Models;

namespace NET1814_MilkShop.Services.Services.Interfaces;

public interface IProductImageService
{
    Task<ResponseModel> GetByProductIdAsync(Guid id, bool? isActive);

    /// <summary>
    /// Use imgur api to upload image and save image url to database
    /// </summary>
    /// <param name="id"></param>
    /// <param name="images"></param>
    /// <returns></returns>
    Task<ResponseModel> CreateProductImageAsync(Guid id, List<IFormFile> images);

    /// <summary>
    /// Delete product image by id (Hard delete)
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<ResponseModel> DeleteProductImageAsync(int id);

    Task<ResponseModel> UpdateProductImageAsync(int id, bool isActive);
}