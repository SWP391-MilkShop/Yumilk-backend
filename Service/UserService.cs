using SWP391_DEMO.Entities;
using SWP391_DEMO.Model;
using SWP391_DEMO.Repository;

namespace SWP391_DEMO.Service
{
    public interface IUserService
    {
        List<UserModel> GetAllUser();
    }
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public List<UserModel> GetAllUser()
        {
            var users = _userRepository.GetAllUser();
            var models = new List<UserModel>();
            foreach (var user in users)
            {
                var model = ToUserModel(user);
                models.Add(model);
            }
            return models;
        }
        /// <summary>
        /// This method is used to convert User to UserModel
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private static UserModel ToUserModel(User user)
        {
            var model = new UserModel
            {
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                VerificationToken = user.VerificationToken,
                RoleId = user.RoleId
            };
            return model;
        }
    }
}
