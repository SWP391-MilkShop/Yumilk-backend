using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Models.ReportModels;

namespace NET1814_MilkShop.Services.Services.Interfaces;

public interface IReportService
{
    Task<ResponseModel> GetReportAsync(ReportQueryModel model);
    Task<ResponseModel> GetReportByIdAsync(Guid id);
    Task<ResponseModel> CreateReportAsync(Guid userId, CreateReportModel model);
    Task<ResponseModel> UpdateResolveStatusAsync(Guid userId, Guid id, bool isResolved);
    Task<ResponseModel> DeleteReportAsync(Guid id);
}