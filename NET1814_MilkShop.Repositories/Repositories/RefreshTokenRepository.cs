using Microsoft.EntityFrameworkCore;
using NET1814_MilkShop.Repositories.Data;
using NET1814_MilkShop.Repositories.Data.Entities;

namespace NET1814_MilkShop.Repositories.Repositories
{
    public interface IRefreshTokenRepository
    {
        void AddToken(RefreshToken token);
        Task<RefreshToken?> GetRefreshTokenAsync(string token);
    }
    public class RefreshTokenRepository : Repository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(AppDbContext context) : base(context)
        {
        }

        public void AddToken(RefreshToken token)
        {
            _context.RefreshTokens.Add(token);
            _context.SaveChanges();
        }

        public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
        {
            SetActiveToken(token);
            return await _context.RefreshTokens.AsNoTracking().FirstOrDefaultAsync(x => x.Token == token && x.IsActive == true);
        }

        private int SetActiveToken(string token)
        {
            var isExist = _context.RefreshTokens.AsNoTracking().FirstOrDefault(x => x.Token == token);
            if (isExist == null)
            {
                return 0;
            }
            if (isExist.Expires < DateTime.UtcNow)
            {
                isExist.IsActive = false;
            }
            return _context.SaveChanges();
        }
    }
}
