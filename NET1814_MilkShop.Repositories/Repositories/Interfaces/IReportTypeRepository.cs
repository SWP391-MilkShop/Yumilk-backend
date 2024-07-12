using NET1814_MilkShop.Repositories.Data.Entities;

namespace NET1814_MilkShop.Repositories.Repositories.Interfaces;

public interface IReportTypeRepository
{
    Task<ReportType?> GetByIdAsync(int id);
    IQueryable<ReportType> GetReportTypeQuery();
    void Add(ReportType reportType);
    void Update(ReportType reportType);
    /// <summary>
    /// Hard delete the report type
    /// </summary>
    /// <param name="reportType"></param>
    void Remove(ReportType reportType);
    
}