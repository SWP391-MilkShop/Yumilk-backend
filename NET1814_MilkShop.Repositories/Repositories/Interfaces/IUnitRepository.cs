using NET1814_MilkShop.Repositories.Data.Entities;

namespace NET1814_MilkShop.Repositories.Repositories.Interfaces;

public interface IUnitRepository
{
    IQueryable<Unit> GetUnitsQuery();
    Task<Unit?> GetByIdAsync(int id);
    void Add(Unit unit);
    void Update(Unit unit);
    void Delete(Unit unit);
    Task<Unit?> GetExistIsActiveId(int id);
}