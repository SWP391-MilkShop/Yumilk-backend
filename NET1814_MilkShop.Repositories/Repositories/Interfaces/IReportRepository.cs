using NET1814_MilkShop.Repositories.Data.Entities;

namespace NET1814_MilkShop.Repositories.Repositories.Interfaces;

public interface IReportRepository
{
    Task<Report?> GetByIdAsync(Guid id);
    IQueryable<Report> GetReportQuery();
    void Add(Report report);
    void Update(Report report);
    void Delete(Report report);
}