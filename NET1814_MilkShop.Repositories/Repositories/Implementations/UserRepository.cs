using Microsoft.EntityFrameworkCore;
using NET1814_MilkShop.Repositories.Data;
using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Repositories.Interfaces;

namespace NET1814_MilkShop.Repositories.Repositories.Implementations;

public sealed class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(AppDbContext context)
        : base(context)
    {
    }

    public IQueryable<User> GetUsersQuery()
    {
        //var query = _context.Users.Include(u => u.Role).AsNoTracking();
        return _query.Include(u => u.Role);
    }

    public async Task<User?> GetByUsernameAsync(string username, int roleId)
    {
        //var user = await _context
        //    .Users.AsNoTracking()
        //    .FirstOrDefaultAsync(x => username.Equals(x.Username));
        var user = await _query.FirstOrDefaultAsync(x => username.Equals(x.Username) && x.RoleId == roleId);
        if (user != null && username.Equals(user.Username, StringComparison.Ordinal))
        {
            return user;
        }

        return null;
    }

    public async Task<bool> IsExistAsync(Guid id)
    {
        //return await _context.Users.AnyAsync(e => e.Id == id && e.IsActive);
        return await _query.AnyAsync(e => e.Id == id);
    }
}