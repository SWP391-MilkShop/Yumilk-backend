using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Models.ImageModels;

namespace NET1814_MilkShop.Services.Services.Interfaces;

public interface IImageService
{
    Task<ResponseModel> UploadImageAsync(ImageUploadModel model);
    Task<ResponseModel> GetImageAsync(string imageHash);
}