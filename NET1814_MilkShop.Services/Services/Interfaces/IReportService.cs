using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Models.ReportModels;
using NET1814_MilkShop.Repositories.Models.ReportTypeModels;

namespace NET1814_MilkShop.Services.Services.Interfaces;

public interface IReportService
{
    Task<ResponseModel> GetReportAsync(ReportQueryModel model);
    Task<ResponseModel> GetReportByIdAsync(Guid id);
    Task<ResponseModel> CreateReportAsync(Guid userId, CreateReportModel model);
    Task<ResponseModel> UpdateResolveStatusAsync(Guid userId, Guid id, bool isResolved);
    Task<ResponseModel> DeleteReportAsync(Guid id);
    Task<ResponseModel> GetReportTypes(ReportTypePageModel model);
    Task<ResponseModel> CreateReportType(CreateReportTypeModel model);
    Task<ResponseModel> UpdateReportType(int id, UpdateReportTypeModel model);
    Task<ResponseModel> DeleteReportType(int id);
}