using System.Linq.Expressions;
using Microsoft.IdentityModel.Tokens;
using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Models.UnitModels;
using NET1814_MilkShop.Repositories.Repositories;
using NET1814_MilkShop.Repositories.UnitOfWork;
using NET1814_MilkShop.Services.CoreHelpers;

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

       public UnitService(IUnitRepository unitRepository,IUnitOfWork unitOfWork)
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
            if(request.IsActive.HasValue)
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
            return new ResponseModel
            {
                Data = units,
                Message = units.TotalCount > 0 ? "Get units successfully" : "No units found",
                Status = "Success"
            };
        }

        public async Task<ResponseModel> GetUnitByIdAsync(int id)
        {
            var unit = await _unitRepository.GetExistIsActiveId(id);
            if (unit == null)
                return new ResponseModel { Status = "failed", Message = "Unit not found" };
            var result = new UnitModel
            {
                Id = id,
                Name = unit.Name,
                Description = unit.Description!,
                IsActive = unit.IsActive
            };
            return new ResponseModel
            {
                Data = result,
                Status = "success",
                Message = "Get unit successfully",
            };
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
                return new ResponseModel
                {
                    Data = createUnitModel,
                    Status = "success",
                    Message = "Create unit successfully"
                };
            }

            return new ResponseModel
            {
                Status = "Error",
                Message = "An error occured while creating unit"
            };
        }

        public async Task<ResponseModel> UpdateUnitAsync(int id,UpdateUnitModel unitModel)
        {
            var isExistUnit = await _unitRepository.GetExistIsActiveId(id);
            if (isExistUnit == null)
            {
                return new ResponseModel { Status = "failed", Message = "Unit not found" };
            }

            if (!unitModel.Name.IsNullOrEmpty())
            {
                isExistUnit.Name = unitModel.Name;
            }

            if (!unitModel.Description.IsNullOrEmpty())
            {
                isExistUnit.Description = unitModel.Description;
            }
            if(unitModel.IsActive.HasValue)
            {
                isExistUnit.IsActive = unitModel.IsActive!.Value;
            }
            _unitRepository.Update(isExistUnit);
            var result = await _unitOfWork.SaveChangesAsync();
            if (result > 0)
            {
                return new ResponseModel()
                {
                    Data = unitModel,
                    Status = "success",
                    Message = "Update unit successfully"
                };
            }

            return new ResponseModel()
            {
                Status = "Error",
                Message = "An error occured while updating unit"
            };
        }

        public async Task<ResponseModel> DeleteUnitAsync(int id)
        {
            var isExistUnit = await _unitRepository.GetExistIsActiveId(id);
            if (isExistUnit == null)
            {
                return new ResponseModel { Status = "failed", Message = "Unit not found" };
            }

            isExistUnit.IsActive = false;
            isExistUnit.DeletedAt = DateTime.Now;
            _unitRepository.Update(isExistUnit);
            var result = await _unitOfWork.SaveChangesAsync();
            if (result > 0)
            {
                return new ResponseModel
                {
                    Status = "success",
                    Message = "Delete unit successfully"
                };
            }

            return new ResponseModel
            {
                Status = "Error",
                Message = "An error occured while deleting unit"
            };
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