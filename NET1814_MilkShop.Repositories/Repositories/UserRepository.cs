using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using NET1814_MilkShop.Repositories.Data;
using NET1814_MilkShop.Repositories.Data.Entities;

namespace NET1814_MilkShop.Repositories.Repositories
{
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
            : base(context) { }

        public IQueryable<User> GetUsersQuery()
        {
            var query = _context.Users.Include(u => u.Role)
                .AsNoTracking();
            return query;
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => username.Equals(x.Username));
        }

        /// <summary>
        /// Get all active users
        /// </summary>
        /// <returns></returns>
        /*public async Task<List<User>> GetUsersAsync()
        {
            return await _context.Users.Where(x => x.IsActive).ToListAsync();
        }*/

        public async Task<string?> GetVerificationTokenAsync(string username)
        {
            var user = await GetByUsernameAsync(username);
            if (user == null)
            {
                return null;
            }
            return user.VerificationCode;
        }
        public async Task<bool> IsExistAsync(Guid id)
        {
            return await _context.Users.AnyAsync(e => e.Id == id);
        }
    }
}
