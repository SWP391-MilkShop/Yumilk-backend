using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Models.AddressModels;

namespace NET1814_MilkShop.Services.Services.Interfaces;

public interface IAddressService
{
    Task<ResponseModel> CreateAddressAsync(Guid customerId, CreateAddressModel model);
    Task<ResponseModel> GetAddressesByCustomerId(Guid customerId);
    Task<ResponseModel> UpdateAddressAsync(Guid customerId, int id, UpdateAddressModel model);
    Task<ResponseModel> DeleteAddressAsync(Guid customerId, int id);
}