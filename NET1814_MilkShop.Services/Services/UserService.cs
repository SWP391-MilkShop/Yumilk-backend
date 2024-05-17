using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Repositories;

namespace NET1814_MilkShop.Services.Services
{
    public interface IUserService
    {
        Task<List<UserModel>> GetUsersAsync();
    }

    public sealed class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<UserModel>> GetUsersAsync()
        {
            var users = await _userRepository.GetUsersAsync();
            var models = users.Select(users => ToUserModel(users)).ToList();
            return models;
        }

        private static UserModel ToUserModel(User user)
        {
            return new UserModel
            {
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                RoleId = user.RoleId,
                IsActive = user.IsActive
            };
        }
    }
}
