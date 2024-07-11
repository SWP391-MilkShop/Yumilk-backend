using Microsoft.EntityFrameworkCore;
using NET1814_MilkShop.Repositories.CoreHelpers.Enum;
using NET1814_MilkShop.Repositories.Data;
using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Repositories.Interfaces;

namespace NET1814_MilkShop.Repositories.Repositories.Implementations;

public sealed class AuthenticationRepository : Repository<User>, IAuthenticationRepository
{
    public AuthenticationRepository(AppDbContext context)
        : base(context)
    {
    }

    public async Task<User?> GetUserByUserNameNPassword(string username, string password, bool isCustomer)
    {
        var user = isCustomer
            ? await _query.Include(u => u.Role)
                .FirstOrDefaultAsync(x => username.Equals(x.Username) && x.RoleId == (int)RoleId.Customer)
            : await _query.Include(u => u.Role)
                .FirstOrDefaultAsync(x => username.Equals(x.Username) && x.RoleId != (int)RoleId.Customer);
        if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password))
        {
            return user;
        }

        return null;
    }
}