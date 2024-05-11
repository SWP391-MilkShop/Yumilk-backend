using SWP391_DEMO.Repository;

namespace SWP391_DEMO.Service
{
    public interface IUserService
    {
        List<UserModel> GetAllUser();
    }
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
    }
}
