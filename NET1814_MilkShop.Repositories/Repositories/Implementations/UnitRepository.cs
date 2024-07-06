using Microsoft.EntityFrameworkCore;
using NET1814_MilkShop.Repositories.Data;
using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Repositories.Interfaces;

namespace NET1814_MilkShop.Repositories.Repositories.Implementations;

public class UnitRepository : Repository<Unit>, IUnitRepository
{
    public UnitRepository(AppDbContext context)
        : base(context)
    {
    }

    public IQueryable<Unit> GetUnitsQuery()
    {
        return _query;
    }

    public Task<Unit?> GetExistIsActiveId(int id)
    {
        return _query.FirstOrDefaultAsync(x => x.Id == id);
    }
}