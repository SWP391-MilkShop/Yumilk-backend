using NET1814_MilkShop.Repositories.Data.Entities;

namespace NET1814_MilkShop.Repositories.Repositories.Interfaces;

public interface IUserRepository
{
    /*Task<List<User>> GetUsersAsync();*/
    IQueryable<User> GetUsersQuery();
    Task<User?> GetByUsernameAsync(string username, int roleId);
    Task<User?> GetByIdAsync(Guid id);
    Task<bool> IsExistAsync(Guid id);
    void Add(User user);
    void Update(User user);
}