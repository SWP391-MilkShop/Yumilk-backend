using System.Linq.Expressions;
using Microsoft.IdentityModel.Tokens;
using NET1814_MilkShop.Repositories.CoreHelpers.Constants;
using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Models.UnitModels;
using NET1814_MilkShop.Repositories.Repositories.Interfaces;
using NET1814_MilkShop.Repositories.UnitOfWork.Interfaces;
using NET1814_MilkShop.Services.CoreHelpers;
using NET1814_MilkShop.Services.Services.Interfaces;

namespace NET1814_MilkShop.Services.Services.Implementations;

public class UnitService : IUnitService
{
    private readonly IUnitRepository _unitRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UnitService(IUnitRepository unitRepository, IUnitOfWork unitOfWork, IProductRepository productRepository)
    {
        _unitRepository = unitRepository;
        _unitOfWork = unitOfWork;
        _productRepository = productRepository;
    }

    public async Task<ResponseModel> GetUnitsAsync(UnitQueryModel request)
    {
        var query = _unitRepository.GetUnitsQuery();

        #region Filter, Search

        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            query = query.Where(u =>
                u.Name.Contains(request.SearchTerm) || u.Description!.Contains(request.SearchTerm)
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
            Gram = u.Gram,
            Description = u.Description!,
            IsActive = u.IsActive
        });

        #endregion

        #region page

        var units = await PagedList<UnitModel>.CreateAsync(result, request.Page, request.PageSize);

        #endregion

        return ResponseModel.Success(ResponseConstants.Get("đơn vị", units.TotalCount > 0), units);
    }

    public async Task<ResponseModel> GetUnitByIdAsync(int id)
    {
        var unit = await _unitRepository.GetExistIsActiveId(id);
        if (unit == null)
            return ResponseModel.Success(ResponseConstants.NotFound("Đơn vị"), null);
        var result = new UnitModel
        {
            Id = id,
            Name = unit.Name,
            Gram = unit.Gram,
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
            Gram = createUnitModel.Gram,
            IsActive = true
        };
        _unitRepository.Add(unit);
        var result = await _unitOfWork.SaveChangesAsync();
        return result > 0 ? ResponseModel.Success(ResponseConstants.Create("đơn vị", true), createUnitModel) 
            : ResponseModel.Error(ResponseConstants.Create("đơn vị", false));
    }

    public async Task<ResponseModel> UpdateUnitAsync(int id, UpdateUnitModel unitModel)
    {
        var isExistUnit = await _unitRepository.GetExistIsActiveId(id);
        if (isExistUnit == null)
        {
            return ResponseModel.Success(ResponseConstants.NotFound("đơn vị"), null);
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

        if (unitModel.Gram == 0)
        {
            isExistUnit.Gram = unitModel.Gram;
        }

        _unitRepository.Update(isExistUnit);
        var result = await _unitOfWork.SaveChangesAsync();
        return result > 0 ? ResponseModel.Success(ResponseConstants.Update("đơn vị", true), unitModel)
            : ResponseModel.Error(ResponseConstants.Update("đơn vị", false));
    }

    public async Task<ResponseModel> DeleteUnitAsync(int id)
    {
        var isExistUnit = await _unitRepository.GetExistIsActiveId(id);
        if (isExistUnit == null)
        {
            return ResponseModel.BadRequest(ResponseConstants.NotFound("đơn vị"), null);
        }
        var isInUsed = await _productRepository.IsExistIdByUnit(id);
        if (isInUsed)
        {
            return ResponseModel.BadRequest(ResponseConstants.InUsed("Đơn vị"));
        }
        _unitRepository.Delete(isExistUnit);
        var result = await _unitOfWork.SaveChangesAsync();
        return result > 0 ? ResponseModel.Success(ResponseConstants.Delete("đơn vị", true), null)
            : ResponseModel.Error(ResponseConstants.Delete("đơn vị", false));
    }

    /// <summary>
    /// Sort property for unit (name, description)
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    private static Expression<Func<Unit, object>> GetSortProperty(UnitQueryModel request)
    {
        return request.SortColumn?.ToLower().Replace(" ", "") switch
        {
            "name" => unit => unit.Name,
            "description" => unit => unit.Description!,
            _ => unit => unit.Id
        };
    }
}