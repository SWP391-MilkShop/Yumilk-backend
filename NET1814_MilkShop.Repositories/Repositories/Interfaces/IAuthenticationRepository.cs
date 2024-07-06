using NET1814_MilkShop.Repositories.Data.Entities;

namespace NET1814_MilkShop.Repositories.Repositories.Interfaces;

public interface IAuthenticationRepository
{
    /// <summary>
    /// Login with username and password
    /// isCustomer = true for customer, false for admin, staff
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <param name="isCustomer"></param>
    /// <returns></returns>
    Task<User?> GetUserByUserNameNPassword(string username, string password, bool isCustomer);
}