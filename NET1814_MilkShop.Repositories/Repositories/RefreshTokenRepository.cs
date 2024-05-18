using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NET1814_MilkShop.Repositories.Repositories
{
    public interface IRefreshTokenRepository
    {
        void Add(RefreshToken token);
    }
    public class RefreshTokenRepository : Repository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(AppDbContext context) : base(context)
        {
        }

    }
}
