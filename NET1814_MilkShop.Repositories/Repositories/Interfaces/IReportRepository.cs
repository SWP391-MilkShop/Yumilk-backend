using NET1814_MilkShop.Repositories.Data.Entities;

namespace NET1814_MilkShop.Repositories.Repositories.Interfaces;

public interface IReportRepository
{
    Task<Report?> GetByIdAsync(Guid id);
    IQueryable<Report> GetReportQuery();
    void Add(Report report);
    void Update(Report report);
    void Delete(Report report);

    /// <summary>
    /// Check if report by the same user, product and report type is exist
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="productId"></param>
    /// <param name="reportTypeId"></param>
    /// <returns></returns>
    Task<bool> IsExistAsync(Guid userId, Guid productId, int reportTypeId);
}