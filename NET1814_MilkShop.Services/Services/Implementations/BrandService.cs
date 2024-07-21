using System.Linq.Expressions;
using NET1814_MilkShop.Repositories.CoreHelpers.Constants;
using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Models.BrandModels;
using NET1814_MilkShop.Repositories.Repositories.Interfaces;
using NET1814_MilkShop.Repositories.UnitOfWork.Interfaces;
using NET1814_MilkShop.Services.CoreHelpers;
using NET1814_MilkShop.Services.Services.Interfaces;

namespace NET1814_MilkShop.Services.Services.Implementations;

public class BrandService : IBrandService
{
    private readonly IBrandRepository _brandRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public BrandService(IBrandRepository brandRepository, IUnitOfWork unitOfWork, IProductRepository productRepository)
    {
        _brandRepository = brandRepository;
        _unitOfWork = unitOfWork;
        _productRepository = productRepository;
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
                || (x.Description != null && x.Description.Contains(queryModel.SearchTerm))
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

        var model = query.Select(x => new BrandModel
        {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description,
            IsActive = x.IsActive,
            Logo = x.Logo
        });

        #region paging

        var brands = await PagedList<BrandModel>.CreateAsync(
            model,
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
            Description = brand.Description,
            IsActive = brand.IsActive,
            Logo = brand.Logo
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
            Logo = model.Logo,
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
            var isExistName = await _brandRepository.GetBrandByName(id, model.Name);
            if (isExistName != null)
            {
                return ResponseModel.BadRequest(ResponseConstants.Exist("Tên thương hiệu"));
            }

            existingBrand.Name = model.Name;
        }

        existingBrand.Description = string.IsNullOrEmpty(model.Description)
            ? existingBrand.Description
            : model.Description;
        existingBrand.Logo = string.IsNullOrEmpty(model.Logo)
            ? existingBrand.Logo
            : model.Logo;
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
        var isCurrentlyInUsed = await _productRepository.IsExistIdByBrand(id);
        if (isCurrentlyInUsed)
        {
            return ResponseModel.BadRequest(ResponseConstants.InUsed("thương hiệu"));
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