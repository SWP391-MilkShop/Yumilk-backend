using Microsoft.EntityFrameworkCore;
using NET1814_MilkShop.Repositories.Data;
using NET1814_MilkShop.Repositories.Data.Entities;

namespace NET1814_MilkShop.Repositories.Repositories;

public interface IUserRepository
{
    /*Task<List<User>> GetUsersAsync();*/
    IQueryable<User> GetUsersQuery();
    Task<User?> GetByUsernameAsync(string username);
    Task<string?> GetVerificationTokenAsync(string username);
    Task<User?> GetById(Guid id);
    Task<bool> IsExistAsync(Guid id);
    void Add(User user);
    void Update(User user);
    void Remove(User user);
}

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

    public async Task<User?> GetByUsernameAsync(string username)
    {
        //var user = await _context
        //    .Users.AsNoTracking()
        //    .FirstOrDefaultAsync(x => username.Equals(x.Username));
        var user = await _query.FirstOrDefaultAsync(x => username.Equals(x.Username));
        if (user != null && username.Equals(user.Username, StringComparison.Ordinal)) return user;
        return null;
    }

    public async Task<string?> GetVerificationTokenAsync(string username)
    {
        var user = await GetByUsernameAsync(username);
        if (user == null) return null;
        return user.VerificationCode;
    }

    public async Task<bool> IsExistAsync(Guid id)
    {
        //return await _context.Users.AnyAsync(e => e.Id == id && e.IsActive);
        return await _query.AnyAsync(e => e.Id == id && e.IsActive);
    }
}