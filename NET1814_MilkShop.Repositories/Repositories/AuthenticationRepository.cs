using Microsoft.EntityFrameworkCore;
using NET1814_MilkShop.Repositories.CoreHelpers.Enum;
using NET1814_MilkShop.Repositories.Data;
using NET1814_MilkShop.Repositories.Data.Entities;

namespace NET1814_MilkShop.Repositories.Repositories
{
    public interface IAuthenticationRepository
    {
        /// <summary>
        /// Login with username and password
        /// isCustomer = true for customer, false for admin, staff
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="isCustomer"></param>
        /// <returns></returns>
        Task<User?> GetUserByUserNameNPassword(string username, string password, bool isCustomer);
    }

    public sealed class AuthenticationRepository : Repository<User>, IAuthenticationRepository
    {
        public AuthenticationRepository(AppDbContext context)
            : base(context)
        {
        }

        public async Task<User?> GetUserByUserNameNPassword(string username, string password, bool isCustomer)
        {
            var user = isCustomer ? 
                await _query.Include(u => u.Role) 
                    .FirstOrDefaultAsync(x => username.Equals(x.Username) && x.RoleId == (int) RoleId.CUSTOMER) :
                await _query.Include(u => u.Role)
                    .FirstOrDefaultAsync(x => username.Equals(x.Username) && x.RoleId != (int) RoleId.CUSTOMER);
            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                return user;
            }

            return null;
        }
    }
}