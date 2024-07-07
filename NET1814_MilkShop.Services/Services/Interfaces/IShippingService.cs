using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Models.ShippingModels;

namespace NET1814_MilkShop.Services.Services.Interfaces;

public interface IShippingService
{
    Task<ResponseModel> GetProvinceAsync();
    Task<ResponseModel> GetDistrictAsync(int provinceId);
    Task<ResponseModel> GetWardAsync(int districtId);
    Task<ResponseModel> GetShippingFeeAsync(ShippingFeeRequestModel model);
    Task<ResponseModel> CreateOrderShippingAsync(Guid orderId);
    Task<ResponseModel> PreviewOrderShippingAsync(Guid orderId);
    Task<ResponseModel> GetOrderDetailAsync(Guid orderId);
    Task<ResponseModel> CancelOrderShippingAsync(Guid orderId);
}