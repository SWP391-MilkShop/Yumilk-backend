using Microsoft.EntityFrameworkCore;
using NET1814_MilkShop.Repositories.Data;
using NET1814_MilkShop.Repositories.Data.Entities;

namespace NET1814_MilkShop.Repositories.Repositories
{
    public interface IUserRepository
    {
        Task<List<User>> GetUsersAsync();
        Task<User?> GetByUsernameAsync(string username);
        Task<string?> GetVerificationTokenAsync(string username);
        Task<User?> GetById(Guid id);
        void Add(User user);
        void Update(User user);
        void Remove(User user);
    }

    public sealed class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context)
            : base(context) { }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => username.Equals(x.Username));
        }

        /// <summary>
        /// Get all active users
        /// </summary>
        /// <returns></returns>
        public async Task<List<User>> GetUsersAsync()
        {
            return await _context.Users.Where(x => x.IsActive).ToListAsync();
        }

        public async Task<string?> GetVerificationTokenAsync(string username)
        {
            var user = await GetByUsernameAsync(username);
            if (user == null)
            {
                return null;
            }
            return user.VerificationToken;
        }
    }
}
