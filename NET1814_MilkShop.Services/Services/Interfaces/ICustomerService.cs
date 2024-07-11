using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Models.UserModels;

namespace NET1814_MilkShop.Services.Services.Interfaces;

public interface ICustomerService
{
    /*Task<ResponseModel> GetCustomersAsync();*/
    Task<ResponseModel> GetCustomersAsync(CustomerQueryModel request);
    Task<ResponseModel> GetCustomersStatsAsync(CustomersStatsQueryModel model);
    Task<ResponseModel> GetByEmailAsync(string email);
    Task<ResponseModel> GetByIdAsync(Guid id);
    Task<ResponseModel> ChangeInfoAsync(Guid userId, ChangeUserInfoModel changeUserInfoModel);
    Task<bool> IsExistAsync(Guid id);

    /*Task<bool> IsCustomerExistAsync(string email, string phoneNumber);*/
    Task<bool> IsExistPhoneNumberAsync(string phoneNumber);
    Task<bool> IsExistEmailAsync(string email);
    Task<ResponseModel> GetReturnCustomerStatsAsync(int year);
    Task<ResponseModel> GetTotalPurchaseAsync();
}