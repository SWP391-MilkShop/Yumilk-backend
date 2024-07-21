using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NET1814_MilkShop.Repositories.CoreHelpers.Constants;
using NET1814_MilkShop.Repositories.CoreHelpers.Enum;
using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Models.ReportModels;
using NET1814_MilkShop.Repositories.Models.ReportTypeModels;
using NET1814_MilkShop.Repositories.Repositories.Interfaces;
using NET1814_MilkShop.Repositories.UnitOfWork.Interfaces;
using NET1814_MilkShop.Services.CoreHelpers;
using NET1814_MilkShop.Services.CoreHelpers.Extensions;
using NET1814_MilkShop.Services.Services.Interfaces;

namespace NET1814_MilkShop.Services.Services.Implementations;

public class ReportService : IReportService
{
    private readonly IReportRepository _reportRepository;
    private readonly IReportTypeRepository _reportTypeRepository;
    private readonly IUserRepository _userRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ReportService(IUnitOfWork unitOfWork, IReportRepository reportRepository,
        IReportTypeRepository reportTypeRepository, IUserRepository userRepository, IProductRepository productRepository)
    {
        _unitOfWork = unitOfWork;
        _reportRepository = reportRepository;
        _reportTypeRepository = reportTypeRepository;
        _userRepository = userRepository;
        _productRepository = productRepository;
    }

    public async Task<ResponseModel> GetReportAsync(ReportQueryModel model)
    {
        var searchTerm = StringExtension.Normalize(model.SearchTerm);
        var query = _reportRepository.GetReportQuery()
            .Include(x => x.Product)
            .Include(x => x.ReportType).AsQueryable();
        //filter
        query = query.Where(x => (model.CustomerId == Guid.Empty || x.CustomerId == model.CustomerId)
                                 && (model.ProductId == Guid.Empty || x.ProductId == model.ProductId)
                                 && (model.IsResolved == null || x.ResolvedBy != Guid.Empty)
                                 && (model.ReportTypeIds.Length == 0 || model.ReportTypeIds.Contains(x.ReportTypeId))
                                 && (string.IsNullOrEmpty(searchTerm) || x.ReportType.Name.Contains(searchTerm)));
        //sort
        if ("desc".Equals(model.SortOrder?.ToLower()))
            query = query.OrderByDescending(GetSortProperty(model.SortColumn));
        else
            query = query.OrderBy(GetSortProperty(model.SortColumn));
        // to model
        var reportModel = query.Select(x => new ReportModel
        {
            Id = x.Id,
            CustomerId = x.CustomerId,
            ReportTypeId = x.ReportTypeId,
            ReportTypeName = x.ReportType.Name,
            ProductId = x.ProductId,
            ProductName = x.Product.Name,
            ResolvedAt = x.ResolvedAt,
            ResolvedBy = x.ResolvedBy,
            CreatedAt = x.CreatedAt
        });
        //paging
        var reports = await PagedList<ReportModel>.CreateAsync(reportModel, model.Page, model.PageSize);
        return ResponseModel.Success(ResponseConstants.Get("báo cáo về sản phẩm", reports.TotalCount > 0), reports);
    }

    private Expression<Func<Report, object>> GetSortProperty(string? modelSortColumn)
    {
        return modelSortColumn switch
        {
            "resolvedAt" => x => x.ResolvedAt ?? x.CreatedAt,
            _ => x => x.CreatedAt
        };
    }


    public async Task<ResponseModel> GetReportByIdAsync(Guid id)
    {
        var report = await _reportRepository.GetReportQuery()
            .Include(x => x.Product)
            .Include(x => x.ReportType)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (report == null)
        {
            return ResponseModel.BadRequest(ResponseConstants.NotFound("Báo cáo từ người dùng"));
        }

        var model = new ReportDetailModel
        {
            Id = report.Id,
            CustomerId = report.CustomerId,
            ReportTypeId = report.ReportTypeId,
            ReportTypeName = report.ReportType.Name,
            ReportTypeDescription = report.ReportType.Description,
            ResolvedAt = report.ResolvedAt,
            ResolvedBy = report.ResolvedBy,
            ProductId = report.ProductId,
            ProductName = report.Product.Name,
            CreatedAt = report.CreatedAt,
            ModifiedAt = report.ModifiedAt
        };
        // get resolver
        if (report.ResolvedBy != Guid.Empty)
        {
            var user = await _userRepository.GetByIdAsync(report.ResolvedBy);
            if (user != null)
                model.ResolverName = user.FirstName + " " + user.LastName;
        }

        return ResponseModel.Success(ResponseConstants.Get("báo cáo về sản phẩm", true), model);
    }

    public async Task<ResponseModel> CreateReportAsync(Guid userId, CreateReportModel model)
    {
        var reportType = await _reportTypeRepository.GetByIdAsync(model.ReportTypeId);
        if (reportType == null)
        {
            return ResponseModel.BadRequest(ResponseConstants.NotFound("Loại báo cáo"));
        }
        var isExistProduct = await _productRepository.IsExistAsync(model.ProductId);
        if(!isExistProduct)
        {
            return ResponseModel.BadRequest(ResponseConstants.NotFound("Sản phẩm"));
        }
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is not { RoleId: (int)RoleId.Customer })
        {
            return ResponseModel.BadRequest(ResponseConstants.NotFound("Khách hàng"));
        }

        var report = new Report
        {
            Id = Guid.NewGuid(),
            CustomerId = userId,
            ReportTypeId = model.ReportTypeId,
            ProductId = model.ProductId,
        };
        _reportRepository.Add(report);
        var result = await _unitOfWork.SaveChangesAsync();
        if (result > 0)
        {
            return ResponseModel.Success(ResponseConstants.Create("báo cáo về sản phẩm", true), null);
        }

        return ResponseModel.Error(ResponseConstants.Create("báo cáo về sản phẩm", false));
    }

    public async Task<ResponseModel> UpdateResolveStatusAsync(Guid userId, Guid id, bool isResolved)
    {
        var report = await _reportRepository.GetByIdAsync(id);
        if (report == null)
        {
            return ResponseModel.BadRequest(ResponseConstants.NotFound("Báo cáo từ người dùng"));
        }

        var user = await _userRepository.GetByIdAsync(userId);
        if (user is not { RoleId: (int)RoleId.Admin or (int)RoleId.Staff })
        {
            return ResponseModel.BadRequest(ResponseConstants.NotFound("Nhân viên"));
        }

        if (isResolved)
        {
            report.ResolvedAt = DateTime.UtcNow;
            report.ResolvedBy = userId;
        }
        else
        {
            report.ResolvedAt = null;
            report.ResolvedBy = Guid.Empty;
        }

        _reportRepository.Update(report);
        var result = await _unitOfWork.SaveChangesAsync();
        if (result > 0)
        {
            return ResponseModel.Success(ResponseConstants.Update("báo cáo về sản phẩm", true), null);
        }

        return ResponseModel.Error(ResponseConstants.Update("báo cáo về sản phẩm", false));
    }

    public async Task<ResponseModel> DeleteReportAsync(Guid id)
    {
        var report = await _reportRepository.GetByIdAsync(id);
        if (report == null)
        {
            return ResponseModel.BadRequest(ResponseConstants.NotFound("Báo cáo từ người dùng"));
        }

        _reportRepository.Delete(report);
        var result = await _unitOfWork.SaveChangesAsync();
        if (result > 0)
        {
            return ResponseModel.Success(ResponseConstants.Delete("báo cáo về sản phẩm", true), null);
        }

        return ResponseModel.Error(ResponseConstants.Delete("báo cáo về sản phẩm", false));
    }

    public async Task<ResponseModel> GetReportTypes(ReportTypePageModel model)
    {
        var query = _reportTypeRepository.GetReportTypeQuery();
        var reportTypes = query.Select(x => new ReportTypeModel
        {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description
        });
        // paging
        var reportTypesList = await PagedList<ReportTypeModel>.CreateAsync(reportTypes, model.Page, model.PageSize);
        return ResponseModel.Success(ResponseConstants.Get("loại báo cáo", reportTypesList.TotalCount > 0),
            reportTypesList);
    }

    public async Task<ResponseModel> CreateReportType(CreateReportTypeModel model)
    {
        var reportType = new ReportType
        {
            Name = model.Name,
            Description = model.Description
        };
        _reportTypeRepository.Add(reportType);
        var result = await _unitOfWork.SaveChangesAsync();
        if (result > 0)
        {
            return ResponseModel.Success(ResponseConstants.Create("loại báo cáo", true), null);
        }

        return ResponseModel.Error(ResponseConstants.Create("loại báo cáo", false));
    }

    public async Task<ResponseModel> UpdateReportType(int id, UpdateReportTypeModel model)
    {
        var reportType = await _reportTypeRepository.GetByIdAsync(id);
        if (reportType == null)
        {
            return ResponseModel.BadRequest(ResponseConstants.NotFound("Loại báo cáo"));
        }

        reportType.Name = model.Name.IsNullOrEmpty() ? reportType.Name : model.Name;
        reportType.Description = model.Description.IsNullOrEmpty() ? reportType.Description : model.Description;
        _reportTypeRepository.Update(reportType);
        var result = await _unitOfWork.SaveChangesAsync();
        if (result > 0)
        {
            return ResponseModel.Success(ResponseConstants.Update("loại báo cáo", true), null);
        }

        return ResponseModel.Error(ResponseConstants.Update("loại báo cáo", false));
    }

    public async Task<ResponseModel> DeleteReportType(int id)
    {
        var reportType = await _reportTypeRepository.GetByIdAsync(id);
        if (reportType == null)
        {
            return ResponseModel.BadRequest(ResponseConstants.NotFound("Loại báo cáo"));
        }

        var isExistReport = await _reportRepository.GetReportQuery().AnyAsync(x => x.ReportTypeId == id);
        if (isExistReport)
        {
            return ResponseModel.BadRequest("Không thể xóa loại báo cáo đang được sử dụng bởi 1 báo cáo khác");
        }

        _reportTypeRepository.Remove(reportType);
        var result = await _unitOfWork.SaveChangesAsync();
        if (result > 0)
        {
            return ResponseModel.Success(ResponseConstants.Delete("loại báo cáo", true), null);
        }

        return ResponseModel.Error(ResponseConstants.Delete("loại báo cáo", false));
    }
}