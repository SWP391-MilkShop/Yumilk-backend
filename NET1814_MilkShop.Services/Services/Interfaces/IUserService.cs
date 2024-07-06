using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Models.UserModels;

namespace NET1814_MilkShop.Services.Services.Interfaces;

public interface IUserService
{
    Task<ResponseModel> ChangePasswordAsync(Guid userId, ChangePasswordModel model);

    Task<ResponseModel> ChangeUsernameAsync(Guid userId, ChangeUsernameModel model);

    /*Task<ResponseModel> GetUsersAsync();*/
    Task<ResponseModel> CreateUserAsync(CreateUserModel model);
    Task<ResponseModel> GetUsersAsync(UserQueryModel request);
    Task<ResponseModel> UpdateUserAsync(Guid id, UpdateUserModel model);
    Task<bool> IsExistAsync(Guid id);
}