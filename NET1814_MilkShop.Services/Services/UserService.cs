using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Repositories;
using NET1814_MilkShop.Repositories.UnitOfWork;

namespace NET1814_MilkShop.Services.Services
{
    public interface IUserService
    {
        Task<ResponseModel> ChangePasswordAsync(Guid userId, ChangePasswordModel model);
        Task<ResponseModel> GetUsersAsync();
        Task<bool> IsExistAsync(Guid id);
    }

    public sealed class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        private static UserModel ToUserModel(User user)
        {
            return new UserModel
            {
                Id = user.Id.ToString(),
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                RoleId = user.RoleId,
                IsActive = user.IsActive
            };
        }
        public async Task<ResponseModel> GetUsersAsync()
        {
            var users = await _userRepository.GetUsersAsync();
            var models = users.Select(users => ToUserModel(users)).ToList();
            return new ResponseModel
            {
                Data = models,
                Message = "Get all users successfully",
                Status = "Success"
            };
        }

        public async Task<bool> IsExistAsync(Guid id)
        {
            return await _userRepository.IsExistAsync(id);
        }

        public async Task<ResponseModel> ChangePasswordAsync(Guid userId, ChangePasswordModel model)
        {
            if (string.Equals(model.OldPassword, model.NewPassword))
            {
                return new ResponseModel
                {
                    Message = "Old password and new password are the same",
                    Status = "Error"
                };
            }
            var user = await _userRepository.GetById(userId);
            if (user == null)
            {
                return new ResponseModel
                {
                    Message = "User not found",
                    Status = "Error"
                };
            }
            if (!BCrypt.Net.BCrypt.Verify(model.OldPassword, user.Password))
            {
                return new ResponseModel
                {
                    Message = "Old password is incorrect",
                    Status = "Error"
                };
            }
            user.Password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            _userRepository.Update(user);
            var result = await _unitOfWork.SaveChangesAsync();
            if (result > 0)
            {
                return new ResponseModel
                {
                    Message = "Change password successfully",
                    Status = "Success"
                };
            }
            return new ResponseModel
            {
                Message = "Change password failed",
                Status = "Error"
            };
        }
    }
}
