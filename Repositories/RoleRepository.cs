using SWP391_DEMO.Data;
using SWP391_DEMO.Entities;

namespace SWP391_DEMO.Repositories
{
    public interface IRoleRepository
    {
        List<Role> GetAllRole();
    }
    public class RoleRepository : IRoleRepository
    {
        private readonly AppDbContext _context;
        public RoleRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<Role> GetAllRole()
        {
            try
            {
                return _context.Roles.ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
