using SWP391_DEMO.Data;
using SWP391_DEMO.Entities;

namespace SWP391_DEMO.Repository
{
    public interface IUserRepository
    {
        List<User> GetAllUser();
        User? GetUserById(Guid id);
        List<User> GetUserByRoleId(int roleId);
    }
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<User> GetAllUser()
        {
            try
            {
                return _context.Users.ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public User? GetUserById(Guid id)
        {
            try
            {
                return _context.Users.FirstOrDefault(x => x.Id == id);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public List<User> GetUserByRoleId(int roleId)
        {
            try
            {
                return _context.Users.Where(x => x.RoleId == roleId).ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
