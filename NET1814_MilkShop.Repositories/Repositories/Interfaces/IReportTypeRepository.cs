using NET1814_MilkShop.Repositories.Data.Entities;

namespace NET1814_MilkShop.Repositories.Repositories.Interfaces;

public interface IReportTypeRepository
{
    Task<ReportType?> GetByIdAsync(int id);
}