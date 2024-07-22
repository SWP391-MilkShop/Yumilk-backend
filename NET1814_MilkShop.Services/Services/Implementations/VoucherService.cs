using System.Linq.Expressions;
using NET1814_MilkShop.Repositories.CoreHelpers.Constants;
using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Models.VoucherModels;
using NET1814_MilkShop.Repositories.Repositories.Interfaces;
using NET1814_MilkShop.Repositories.UnitOfWork.Interfaces;
using NET1814_MilkShop.Services.CoreHelpers;
using NET1814_MilkShop.Services.CoreHelpers.Extensions;
using NET1814_MilkShop.Services.Services.Interfaces;

namespace NET1814_MilkShop.Services.Services.Implementations;

public class VoucherService : IVoucherService
{
    private readonly IVoucherRepository _voucherRepository;
    private readonly IUnitOfWork _unitOfWork;

    public VoucherService(IVoucherRepository voucherRepository, IUnitOfWork unitOfWork)
    {
        _voucherRepository = voucherRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseModel> GetVouchersAsync(VoucherQueryModel model)
    {
        if (model is { StartDate: not null, EndDate: not null } && model.StartDate > model.EndDate)
        {
            return ResponseModel.BadRequest(ResponseConstants.InvalidFilterDate);
        }

        var searchTerm = StringExtension.Normalize(model.SearchTerm);
        var query = _voucherRepository.GetVouchersQuery();
        // Filter
        query = query.Where(v => (!model.IsActive.HasValue || v.IsActive == model.IsActive)
                                 && (!model.MinPriceCondition.HasValue ||
                                     v.MinPriceCondition <= model.MinPriceCondition) // Filter by min price condition
                                 && (!model.StartDate.HasValue || v.StartDate >= model.StartDate)
                                 && (!model.EndDate.HasValue || v.EndDate <= model.EndDate)
                                 && (string.IsNullOrEmpty(searchTerm) || v.Code.Contains(searchTerm) ||
                                     v.Description.Contains(searchTerm)));
        // Sort
        if ("desc".Equals(model.SortOrder?.ToLower()))
            query = query.OrderByDescending(GetSortProperty(model.SortColumn));
        else
            query = query.OrderBy(GetSortProperty(model.SortColumn));
        var vouchersModel = query.Select(v => ToVoucherModel(v));
        // Paging
        var vouchers = await PagedList<VoucherModel>.CreateAsync(vouchersModel, model.Page, model.PageSize);
        return ResponseModel.Success(ResponseConstants.Get("danh sách voucher", vouchers.TotalCount > 0), vouchers);
    }

    public async Task<ResponseModel> GetByIdAsync(Guid id)
    {
        var voucher = await _voucherRepository.GetByIdAsync(id);
        if (voucher == null)
        {
            return ResponseModel.BadRequest(ResponseConstants.NotFound("Voucher"));
        }

        var model = ToVoucherModel(voucher);
        return ResponseModel.Success(ResponseConstants.Get("voucher", true), model);
    }

    private Expression<Func<Voucher, object>> GetSortProperty(string? sortColumn)
    {
        return sortColumn?.ToLower().Replace(" ", "") switch
        {
            "startdate" => v => v.StartDate,
            "enddate" => v => v.EndDate,
            "minpricecondition" => v => v.Percent,
            "quantity" => v => v.Quantity,
            "maxdiscount" => v => v.MaxDiscount,
            _ => v => v.Percent // Default sort by created_at
        };
    }

    public async Task<ResponseModel> CreateVoucherAsync(CreateVoucherModel model)
    {
        if (model.StartDate > model.EndDate)
        {
            return ResponseModel.BadRequest(ResponseConstants.InvalidFilterDate);
        }

        // Generate random code
        var code = CodeExtension.GenerateRandomString(10);
        while (await _voucherRepository.IsCodeExistAsync(code))
        {
            code = CodeExtension.GenerateRandomString(10);
        }

        var voucher = new Voucher
        {
            Code = code,
            Description = model.Description ?? "",
            StartDate = model.StartDate,
            EndDate = model.EndDate,
            Quantity = model.Quantity,
            Percent = model.Percent,
            IsActive = false, // Default is unpublished
            MaxDiscount = model.MaxDiscount,
            MinPriceCondition = model.MinPriceCondition
        };
        _voucherRepository.Add(voucher);
        var result = await _unitOfWork.SaveChangesAsync();
        if (result > 0)
        {
            return ResponseModel.Success(ResponseConstants.Create("voucher", true), ToVoucherModel(voucher));
        }

        return ResponseModel.Error(ResponseConstants.Create("voucher", false));
    }

    public async Task<ResponseModel> UpdateVoucherAsync(Guid id, UpdateVoucherModel model)
    {
        var voucher = await _voucherRepository.GetByIdAsync(id);
        if (voucher == null)
        {
            return ResponseModel.BadRequest(ResponseConstants.NotFound("Voucher"));
        }

        voucher.Description = model.Description ?? voucher.Description;
        voucher.StartDate = model.StartDate ?? voucher.StartDate;
        voucher.EndDate = model.EndDate ?? voucher.EndDate;
        voucher.Quantity = model.Quantity ?? voucher.Quantity;
        if (model.Percent is > 50 or < 5)
        {
            return ResponseModel.BadRequest("Phần trăm giảm giá phải từ 5% đến 50%");
        }
        voucher.Percent = model.Percent ?? voucher.Percent;
        voucher.IsActive = model.IsActive;
        voucher.MaxDiscount = model.MaxDiscount ?? voucher.MaxDiscount;
        voucher.MinPriceCondition = model.MinPriceCondition ?? voucher.MinPriceCondition;
        if (voucher.IsActive)
        {
            // if (voucher.StartDate <= DateTime.UtcNow)
            // {
            //     return ResponseModel.BadRequest("Ngày bắt đầu phải lớn hơn hoặc bằng ngày hiện tại");
            // }

            if (voucher.StartDate > voucher.EndDate)
            {
                return ResponseModel.BadRequest(ResponseConstants.InvalidFilterDate);
            }

            if (voucher.Percent is > 50 or < 5)
            {
                return ResponseModel.BadRequest("Phần trăm giảm giá phải từ 5% đến 50%");
            }

            if (voucher.MaxDiscount < 0)
            {
                return ResponseModel.BadRequest("Giá trị giảm giá tối đa không được nhỏ hơn 0");
            }

            if (voucher.Quantity <= 0)
            {
                return ResponseModel.BadRequest("Số lượng voucher phải lớn hơn 0");
            }

            if (voucher.MinPriceCondition < 0)
            {
                return ResponseModel.BadRequest("Giá trị đơn hàng tối thiểu phải lớn hơn 0");
            }
        }

        _voucherRepository.Update(voucher);
        var result = await _unitOfWork.SaveChangesAsync();
        if (result > 0)
        {
            return ResponseModel.Success(ResponseConstants.Update("voucher", true), null);
        }

        return ResponseModel.Error(ResponseConstants.Update("voucher", false));
    }

    public async Task<ResponseModel> DeleteVoucherAsync(Guid id)
    {
        var voucher = await _voucherRepository.GetByIdAsync(id);
        if (voucher == null)
        {
            return ResponseModel.BadRequest(ResponseConstants.NotFound("Voucher"));
        }

        _voucherRepository.Delete(voucher);
        var result = await _unitOfWork.SaveChangesAsync();
        if (result > 0)
        {
            return ResponseModel.Success(ResponseConstants.Delete("voucher", true), null);
        }

        return ResponseModel.Error(ResponseConstants.Delete("voucher", false));
    }

    public async Task<ResponseModel> GetByCodeAsync(string code)
    {
        var voucher = await _voucherRepository.GetByCodeAsync(code);
        if (voucher == null)
        {
            return ResponseModel.BadRequest(ResponseConstants.NotFound("Voucher"));
        }

        var model = ToVoucherModel(voucher);
        return ResponseModel.Success(ResponseConstants.Get("voucher", true), model);
    }

    private static VoucherModel ToVoucherModel(Voucher voucher)
    {
        return new VoucherModel
        {
            Id = voucher.Id,
            Code = voucher.Code,
            Description = voucher.Description,
            StartDate = voucher.StartDate,
            EndDate = voucher.EndDate,
            Quantity = voucher.Quantity,
            Percent = voucher.Percent,
            IsActive = voucher.IsActive,
            IsAvailable = IsVoucherValid(voucher),
            MaxDiscount = voucher.MaxDiscount,
            CreatedAt = voucher.CreatedAt,
            MinPriceCondition = voucher.MinPriceCondition
        };
    }

    /// <summary>
    /// Check if voucher is active, not expired and still have quantity
    /// </summary>
    /// <param name="voucher"></param>
    /// <returns></returns>
    private static bool IsVoucherValid(Voucher voucher)
    {
        return voucher.IsActive
               && voucher.StartDate <= DateTime.UtcNow
               && voucher.EndDate >= DateTime.UtcNow
               && voucher.Quantity > 0;
    }
}