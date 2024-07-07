using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Models.UserModels;

namespace NET1814_MilkShop.Services.Services.Interfaces;

public interface IAuthenticationService
{
    Task<ResponseModel> SignUpAsync(SignUpModel model);

    /// <summary>
    /// Customer login
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    Task<ResponseModel> LoginAsync(RequestLoginModel model);

    Task<ResponseModel> VerifyAccountAsync(string token);
    Task<ResponseModel> ForgotPasswordAsync(ForgotPasswordModel request);
    Task<ResponseModel> ResetPasswordAsync(ResetPasswordModel request);
    Task<ResponseModel> RefreshTokenAsync(string token);
    Task<ResponseModel> ActivateAccountAsync(string email);

    /// <summary>
    /// Admin, Staff login
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    Task<ResponseModel> DashBoardLoginAsync(RequestLoginModel model);

    Task<ResponseModel> GoogleLoginAsync(string token);
}