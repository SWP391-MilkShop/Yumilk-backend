using Microsoft.EntityFrameworkCore;
using NET1814_MilkShop.Repositories.Data;
using NET1814_MilkShop.Repositories.Data.Entities;

namespace NET1814_MilkShop.Repositories.Repositories
{
    public interface IUserRepository
    {
        Task<List<User>> GetUsersAsync();
    }
    public sealed class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context)
            : base(context)
        {
        }

        public async Task<List<User>> GetUsersAsync()
        {
            return await _context.Users.Where(x=> x.IsActive==true).ToListAsync();
        }
    }
}
