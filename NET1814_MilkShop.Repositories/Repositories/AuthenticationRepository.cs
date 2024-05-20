using Microsoft.EntityFrameworkCore;
using NET1814_MilkShop.Repositories.Data;
using NET1814_MilkShop.Repositories.Data.Entities;

namespace NET1814_MilkShop.Repositories.Repositories
{
    public interface IAuthenticationRepository
    {
        Task<User?> GetUserByUserNameNPassword(string username, string password);

    }
    public class AuthenticationRepository : Repository<User>, IAuthenticationRepository
    {
        public AuthenticationRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<User?> GetUserByUserNameNPassword(string username, string password)
        {
            var userName = await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Username.Equals(username) && x.IsActive == true);
            if (userName != null && BCrypt.Net.BCrypt.Verify(password, userName.Password))
            {
                return userName;
            }
            return null;
        }
    }
}
