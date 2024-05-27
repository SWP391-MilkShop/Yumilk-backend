using Microsoft.EntityFrameworkCore;
using NET1814_MilkShop.Repositories.Data;
using NET1814_MilkShop.Repositories.Data.Entities;

namespace NET1814_MilkShop.Repositories.Repositories
{
    public interface IUnitRepository
    {
        IQueryable<Unit> GetUnitsQuery();
        void Add(Unit unit);
        void Update(Unit unit);
        Task<Unit?> GetExistIsActiveId(int id);
    }

    public class UnitRepository : Repository<Unit>, IUnitRepository
    {
        public UnitRepository(AppDbContext context)
            : base(context) { }

        public IQueryable<Unit> GetUnitsQuery() =>
            _context.Units.AsNoTracking();

        public Task<Unit?> GetExistIsActiveId(int id) =>
            _context.Units.FirstOrDefaultAsync(x => x.Id == id);
    }
}
