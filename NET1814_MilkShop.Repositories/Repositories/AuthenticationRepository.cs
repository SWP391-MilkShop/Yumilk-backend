using Microsoft.EntityFrameworkCore;
using NET1814_MilkShop.Repositories.Data;
using NET1814_MilkShop.Repositories.Data.Entities;

namespace NET1814_MilkShop.Repositories.Repositories
{
    public interface IAuthenticationRepository
    {
        Task<User?> GetUserByUserNameNPassword(string username, string password);
    }

    public sealed class AuthenticationRepository : Repository<User>, IAuthenticationRepository
    {
        public AuthenticationRepository(AppDbContext context)
            : base(context) { }
        /// <summary>
        /// Get by username and password for login
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<User?> GetUserByUserNameNPassword(string username, string password)
        {
            var user = await _context
                .Users.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Username.Equals(username));
            //check case sensitive
            if (username.Equals(user.Username, StringComparison.Ordinal))
            {
                if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password))
                {
                    return user;
                }
            }
            return null;
        }
    }
}
