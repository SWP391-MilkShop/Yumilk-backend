using NET1814_MilkShop.Repositories.CoreHelpers.Constants;
using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Models.UserModels;
using NET1814_MilkShop.Repositories.Repositories;
using NET1814_MilkShop.Repositories.UnitOfWork;
using NET1814_MilkShop.Services.CoreHelpers;
using NET1814_MilkShop.Services.CoreHelpers.Extensions;
using System.Linq.Expressions;

namespace NET1814_MilkShop.Services.Services
{
    public interface IUserService
    {
        Task<ResponseModel> ChangePasswordAsync(Guid userId, ChangePasswordModel model);

        /*Task<ResponseModel> GetUsersAsync();*/
        Task<ResponseModel> GetUsersAsync(UserQueryModel request);
        Task<ResponseModel> UpdateUserAsync(Guid id, UpdateUserModel model);
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

        /*private static UserModel ToUserModel(User user)
        {
            return new UserModel
            {
                Id = user.Id.ToString(),
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role.Name,
                IsActive = user.IsActive
            };
        }*/
        /*public async Task<ResponseModel> GetUsersAsync()
        {
            var users = await _userRepository.GetUsersAsync();
            var models = users.Select(users => ToUserModel(users)).ToList();
            return new ResponseModel
            {
                Data = models,
                Message = "Get all users successfully",
                Status = "Success"
            };
        }*/

        public async Task<ResponseModel> GetUsersAsync(UserQueryModel request)
        {
            var query = _userRepository.GetUsersQuery();
            //filter
            var searchTerm = StringExtension.Normalize(request.SearchTerm);
            query = query.Where(u => string.IsNullOrEmpty(searchTerm)
                || u.Username.ToLower().Contains(searchTerm)
                || u.FirstName.Contains(searchTerm)
                || u.LastName.Contains(searchTerm));

            if (!string.IsNullOrEmpty(request.Role))
            {
                var roleIds = request.Role.Split(',').Select(int.Parse).ToList();
                query = query.Where(u => roleIds.Contains(u.RoleId));
            }

            if (request.IsActive.HasValue || request.IsBanned.HasValue)
            {
                query = query.Where(u => (!request.IsActive.HasValue || u.IsActive == request.IsActive.Value)
                                      && (!request.IsBanned.HasValue || u.IsBanned == request.IsBanned.Value));
            }
            //sort
            query = "desc".Equals(request.SortOrder?.ToLower())
                ? query.OrderByDescending(GetSortProperty(request))
                : query.OrderBy(GetSortProperty(request));
            var result = query.Select(u => new UserModel
            {
                Id = u.Id.ToString(),
                Username = u.Username,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Role = u.Role!.Name,
                IsActive = u.IsActive,
                IsBanned = u.IsBanned
            });
            //page
            var users = await PagedList<UserModel>.CreateAsync(
                result,
                request.Page,
                request.PageSize
            );
            return ResponseModel.Success(ResponseConstants.Get("người dùng", users.TotalCount>0), users);
        }

        private static Expression<Func<User, object>> GetSortProperty(UserQueryModel request)
        {
            Expression<Func<User, object>> keySelector = request.SortColumn?.ToLower() switch
            {
                "username" => user => user.Username,
                "firstName" => user => user.FirstName!,
                "lastName" => user => user.LastName!,
                "role" => user => user.Role!.Name,
                "isActive" => user => user.IsActive,
                _ => user => user.Id
            };
            return keySelector;
        }

        public async Task<bool> IsExistAsync(Guid id)
        {
            return await _userRepository.IsExistAsync(id);
        }

        public async Task<ResponseModel> ChangePasswordAsync(Guid userId, ChangePasswordModel model)
        {
            if (string.Equals(model.OldPassword, model.NewPassword))
            {
                return ResponseModel.BadRequest(ResponseConstants.PassSameNewPass);
            }

            var user = await _userRepository.GetById(userId);
            if (user == null)
            {
                return ResponseModel.Success(ResponseConstants.NotFound("Người dùng"), null);
            }

            if (!BCrypt.Net.BCrypt.Verify(model.OldPassword, user.Password))
            {
                return ResponseModel.BadRequest(ResponseConstants.WrongPassword);
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            _userRepository.Update(user);
            var result = await _unitOfWork.SaveChangesAsync();
            if (result > 0)
            {
                return ResponseModel.Success(ResponseConstants.ChangePassword(true), null);
            }

            return ResponseModel.Error(ResponseConstants.ChangePassword(false));
        }

        public async Task<ResponseModel> UpdateUserAsync(Guid id, UpdateUserModel model)
        {
            var user = await _userRepository.GetById(id);
            if (user == null)
            {
                return ResponseModel.Success(ResponseConstants.NotFound("Người dùng"), null);
            }
            user.IsBanned = model.IsBanned;
            _userRepository.Update(user);
            var result = await _unitOfWork.SaveChangesAsync();
            if (result > 0)
            {
                return ResponseModel.Success(ResponseConstants.Update("người dùng", true), null);
            }
            return ResponseModel.Error(ResponseConstants.Update("người dùng", false));

        }
    }
}