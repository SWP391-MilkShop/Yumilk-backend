using NET1814_MilkShop.Repositories.CoreHelpers.Constants;
using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Models.BrandModels;
using NET1814_MilkShop.Repositories.Repositories;
using NET1814_MilkShop.Repositories.UnitOfWork;
using NET1814_MilkShop.Services.CoreHelpers;
using System.Linq.Expressions;

namespace NET1814_MilkShop.Services.Services;

public interface IBrandService
{
    Task<ResponseModel> GetBrandsAsync(BrandQueryModel queryModel);
    Task<ResponseModel> GetBrandByIdAsync(int id);
    Task<ResponseModel> CreateBrandAsync(CreateBrandModel model);
    Task<ResponseModel> UpdateBrandAsync(int id, UpdateBrandModel model);
    Task<ResponseModel> DeleteBrandAsync(int id);
}

public class BrandService : IBrandService
{
    private readonly IBrandRepository _brandRepository;
    private readonly IUnitOfWork _unitOfWork;

    public BrandService(IBrandRepository brandRepository, IUnitOfWork unitOfWork)
    {
        _brandRepository = brandRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseModel> GetBrandsAsync(BrandQueryModel queryModel)
    {
        var query = _brandRepository.GetBrandsQuery();

        #region filter

        if (queryModel.IsActive.HasValue)
        {
            query = query.Where(x => x.IsActive == queryModel.IsActive);
        }

        if (!string.IsNullOrEmpty(queryModel.SearchTerm))
        {
            query = query.Where(x =>
                x.Name.Contains(queryModel.SearchTerm)
                || x.Description.Contains(queryModel.SearchTerm)
            );
        }

        #endregion

        #region sort

        if ("desc".Equals(queryModel.SortOrder))
        {
            query = query.OrderByDescending(GetSortBrandProperty(queryModel));
        }
        else
        {
            query = query.OrderBy(GetSortBrandProperty(queryModel));
        }

        #endregion

        var model = query.Select(x => new BrandModel()
        {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description
        });

        #region paging

        var brands = await PagedList<Brand>.CreateAsync(
            query,
            queryModel.Page,
            queryModel.PageSize
        );

        #endregion

        return ResponseModel.Success(
            ResponseConstants.Get("thương hiệu", brands.TotalCount > 0),
            brands
        );
    }

    public async Task<ResponseModel> GetBrandByIdAsync(int id)
    {
        var brand = await _brandRepository.GetByIdAsync(id);
        if (brand == null)
        {
            return ResponseModel.Success(ResponseConstants.NotFound("Thương hiệu"), null);
        }

        var model = new BrandModel
        {
            Id = brand.Id,
            Name = brand.Name,
            Description = brand.Description
        };

        return ResponseModel.Success(ResponseConstants.Get("thương hiệu", true), model);
    }

    public async Task<ResponseModel> CreateBrandAsync(CreateBrandModel model)
    {
        // var isExistId = await _brandRepository.GetByIdAsync(model.Id);
        // if (isExistId != null) //không cần check vì brandid tự tăng và không được nhập
        // {
        //     return new ResponseModel
        //     {
        //         Message = "BrandId is existed",
        //         Status = "Error"
        //     };
        // }
        var isExistName = await _brandRepository.GetBrandByName(model.Name);
        if (isExistName != null)
        {
            return ResponseModel.BadRequest(ResponseConstants.Exist("Thương hiệu"));
        }
        var entity = new Brand
        {
            Name = model.Name,
            Description = model.Description,
            IsActive = true
        };
        _brandRepository.Add(entity);
        var result = await _unitOfWork.SaveChangesAsync();
        if (result > 0)
            return ResponseModel.Success(ResponseConstants.Create("thương hiệu", true), null);
        return ResponseModel.Error(ResponseConstants.Create("thương hiệu", false));
    }

    public async Task<ResponseModel> UpdateBrandAsync(int id, UpdateBrandModel model)
    {
        var existingBrand = await _brandRepository.GetByIdAsync(id);
        if (existingBrand == null)
        {
            return ResponseModel.Success(ResponseConstants.NotFound("Thương hiệu"), null);
        }

        if (!string.IsNullOrEmpty(model.Name))
        {
            var isExistName = await _brandRepository.GetBrandByName(model.Name);
            if (isExistName != null)
            {
                return ResponseModel.BadRequest(ResponseConstants.Exist("Tên thương hiệu"));
            }
            existingBrand.Name = model.Name;
        }

        existingBrand.Description = string.IsNullOrEmpty(model.Description)
            ? existingBrand.Description
            : model.Description;
        existingBrand.IsActive = model.IsActive;
        _brandRepository.Update(existingBrand);
        var result = await _unitOfWork.SaveChangesAsync();
        if (result > 0)
        {
            return ResponseModel.Success(ResponseConstants.Update("thương hiệu", true), null);
        }
        return ResponseModel.Error(ResponseConstants.Update("thương hiệu", false));
    }

    public async Task<ResponseModel> DeleteBrandAsync(int id)
    {
        var isExist = await _brandRepository.GetByIdAsync(id);
        if (isExist == null)
        {
            return ResponseModel.Success(ResponseConstants.NotFound("Thương hiệu"), null);
        }

        _brandRepository.Delete(isExist);
        var result = await _unitOfWork.SaveChangesAsync();
        if (result > 0)
        {
            return ResponseModel.Success(ResponseConstants.Delete("thương hiệu", true), null);
        }
        return ResponseModel.Error(ResponseConstants.Delete("thương hiệu", false));
    }

    private static Expression<Func<Brand, object>> GetSortBrandProperty(
        BrandQueryModel queryModel
    ) => queryModel.SortColumn?.ToLower().Replace(" ", "") switch
    {
        "name" => product => product.Name,
        _ => product => product.Id
    };
}