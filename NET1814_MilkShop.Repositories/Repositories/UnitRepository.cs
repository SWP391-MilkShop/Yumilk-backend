using Microsoft.EntityFrameworkCore;
using NET1814_MilkShop.Repositories.Data;
using NET1814_MilkShop.Repositories.Data.Entities;

namespace NET1814_MilkShop.Repositories.Repositories
{
    public interface IUnitRepository
    {
        IQueryable<Unit> GetUnitsQuery();
        Task<Unit?> GetByIdAsync(int id);
        void Add(Unit unit);
        void Update(Unit unit);
        void Delete(Unit unit);
        Task<Unit?> GetExistIsActiveId(int id);
    }

    public class UnitRepository : Repository<Unit>, IUnitRepository
    {
        public UnitRepository(AppDbContext context)
            : base(context) { }

        public IQueryable<Unit> GetUnitsQuery()
        {
            return _query;
        }

        public Task<Unit?> GetExistIsActiveId(int id)
        {
            return _query.FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
