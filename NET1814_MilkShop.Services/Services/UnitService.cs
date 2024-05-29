using Microsoft.IdentityModel.Tokens;
using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Models.UnitModels;
using NET1814_MilkShop.Repositories.Repositories;
using NET1814_MilkShop.Repositories.UnitOfWork;
using NET1814_MilkShop.Services.CoreHelpers;
using System.Linq.Expressions;
using NET1814_MilkShop.Repositories.CoreHelpers.Constants;

namespace NET1814_MilkShop.Services.Services;

public interface IUnitService
{
    Task<ResponseModel> GetUnitsAsync(UnitQueryModel request);
    Task<ResponseModel> GetUnitByIdAsync(int id);
    Task<ResponseModel> CreateUnitAsync(CreateUnitModel createUnitModel);
    Task<ResponseModel> UpdateUnitAsync(int id, UpdateUnitModel unitModel);
    Task<ResponseModel> DeleteUnitAsync(int id);
}

public class UnitService : IUnitService
{
    private readonly IUnitRepository _unitRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UnitService(IUnitRepository unitRepository, IUnitOfWork unitOfWork)
    {
        _unitRepository = unitRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseModel> GetUnitsAsync(UnitQueryModel request)
    {
        var query = _unitRepository.GetUnitsQuery();

        #region Filter, Search

        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            query = query.Where(u =>
                u.Name.Contains(request.SearchTerm)
                || u.Description!.Contains(request.SearchTerm)
            );
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(u => u.IsActive == request.IsActive);
        }

        #endregion

        #region sort

        query = "desc".Equals(request.SortOrder?.ToLower())
            ? query.OrderByDescending(GetSortProperty(request))
            : query.OrderBy(GetSortProperty(request));
        var result = query.Select(u => new UnitModel
        {
            Id = u.Id,
            Name = u.Name,
            Description = u.Description!,
            IsActive = u.IsActive
        });

        #endregion

        #region page

        var units = await PagedList<UnitModel>.CreateAsync(
            result,
            request.Page,
            request.PageSize
        );

        #endregion

        if (units.TotalCount > 0)
        {
            return ResponseModel.Success(ResponseConstants.Get("đơn vị", true), units);
        }

        return ResponseModel.Error(ResponseConstants.NotFound("Đơn vị"));
    }

    public async Task<ResponseModel> GetUnitByIdAsync(int id)
    {
        var unit = await _unitRepository.GetExistIsActiveId(id);
        if (unit == null)
            return ResponseModel.Success(ResponseConstants.NotFound("Đơn vị"),null);
        var result = new UnitModel
        {
            Id = id,
            Name = unit.Name,
            Description = unit.Description!,
            IsActive = unit.IsActive
        };
        return ResponseModel.Success(ResponseConstants.Get("đơn vị", true), result);
    }

    public async Task<ResponseModel> CreateUnitAsync(CreateUnitModel createUnitModel)
    {
        var unit = new Unit
        {
            Name = createUnitModel.Name,
            Description = createUnitModel.Description,
            IsActive = true
        };
        _unitRepository.Add(unit);
        var result = await _unitOfWork.SaveChangesAsync();
        if (result > 0)
        {
            return ResponseModel.Success(ResponseConstants.Create("đơn vị", true), createUnitModel);
        }

        return ResponseModel.Error(ResponseConstants.Create("đơn vị", false));
    }

    public async Task<ResponseModel> UpdateUnitAsync(int id, UpdateUnitModel unitModel)
    {
        var isExistUnit = await _unitRepository.GetExistIsActiveId(id);
        if (isExistUnit == null)
        {
            return ResponseModel.Success(ResponseConstants.NotFound("đơn vị"),null);
        }

        if (!unitModel.Name.IsNullOrEmpty())
        {
            isExistUnit.Name = unitModel.Name;
        }

        if (!unitModel.Description.IsNullOrEmpty())
        {
            isExistUnit.Description = unitModel.Description;
        }

        if (unitModel.IsActive.HasValue)
        {
            isExistUnit.IsActive = unitModel.IsActive!.Value;
        }

        _unitRepository.Update(isExistUnit);
        var result = await _unitOfWork.SaveChangesAsync();
        if (result > 0)
        {
            return ResponseModel.Success(ResponseConstants.Update("đơn vị", true), unitModel);
        }

        return ResponseModel.Error(ResponseConstants.Update("đơn vị", false));
    }

    public async Task<ResponseModel> DeleteUnitAsync(int id)
    {
        var isExistUnit = await _unitRepository.GetExistIsActiveId(id);
        if (isExistUnit == null)
        {
            return ResponseModel.Success(ResponseConstants.NotFound("đơn vị"),null);
        }

        _unitRepository.Delete(isExistUnit);
        var result = await _unitOfWork.SaveChangesAsync();
        if (result > 0)
        {
            return ResponseModel.Success(ResponseConstants.Delete("đơn vị", true), null);
        }

        return ResponseModel.Error(ResponseConstants.Delete("đơn vị", false));
    }

    /// <summary>
    /// Sort property for unit (name, description)
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    private static Expression<Func<Unit, object>> GetSortProperty(UnitQueryModel request)
    {
        return request.SortColumn?.ToLower() switch
        {
            "name" => unit => unit.Name,
            "description" => unit => unit.Description!,
            _ => unit => unit.Id
        };
    }
}