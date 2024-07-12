using NET1814_MilkShop.Repositories.Data;
using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Repositories.Interfaces;

namespace NET1814_MilkShop.Repositories.Repositories.Implementations;

public class ReportTypeRepository : Repository<ReportType>, IReportTypeRepository
{
    public ReportTypeRepository(AppDbContext context) : base(context)
    {
    }
}