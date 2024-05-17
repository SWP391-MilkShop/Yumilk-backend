using Microsoft.EntityFrameworkCore;
using NET1814_MilkShop.Repositories.Data;
using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var userName = await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Username.Equals(username));
            if (userName != null && BCrypt.Net.BCrypt.Verify(password, userName.Password))
            {
                return userName;
            }
            return null;
        }
    }

}
