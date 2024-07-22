using Microsoft.EntityFrameworkCore;
using NET1814_MilkShop.Repositories.Data;
using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Repositories.Interfaces;

namespace NET1814_MilkShop.Repositories.Repositories.Implementations;

public class ReportRepository : Repository<Report>, IReportRepository
{
    public ReportRepository(AppDbContext context) : base(context)
    {
    }

    public IQueryable<Report> GetReportQuery()
    {
        return _query;
    }

    public async Task<bool> IsExistAsync(Guid userId, Guid productId, int reportTypeId)
    {
        return await _query.AnyAsync(x =>
            x.CustomerId == userId && x.ProductId == productId && x.ReportTypeId == reportTypeId);
    }
}