using System.Linq.Expressions;
using System.Runtime.Intrinsics.X86;
using Microsoft.EntityFrameworkCore;
using NET1814_MilkShop.Repositories.CoreHelpers.Constants;
using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Models.ProductAttributeValueModels;
using NET1814_MilkShop.Repositories.Repositories;
using NET1814_MilkShop.Repositories.UnitOfWork;
using NET1814_MilkShop.Services.CoreHelpers;

namespace NET1814_MilkShop.Services.Services;

public interface IProductAttributeValueService
{
    Task<ResponseModel> GetProductAttributeValue(Guid id, ProductAttributeValueQueryModel queryModel);
    Task<ResponseModel> AddProductAttributeValue(Guid pid, int aid, CreateUpdatePavModel model);
    Task<ResponseModel> UpdateProductAttributeValue(Guid pid, int aid, CreateUpdatePavModel model);
    Task<ResponseModel> DeleteProductAttributeValue(Guid pid, int aid);
}

public class ProductAttributeValueService : IProductAttributeValueService
{
    private readonly IProductAttributeValueRepository _proAttValueRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProductAttributeValueService(IProductAttributeValueRepository proAttValue, IUnitOfWork unitOfWork)
    {
        _proAttValueRepository = proAttValue;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseModel> GetProductAttributeValue(Guid id, ProductAttributeValueQueryModel queryModel)
    {
        var query = _proAttValueRepository.GetProductAttributeValue();

        #region filter

        if (!string.IsNullOrEmpty(id.ToString()))
        {
            query = query.Where(x => x.ProductId == id);
        }

        if (!string.IsNullOrEmpty(queryModel.SearchTerm))
        {
            query = query.Where(x => x.Value.Contains(queryModel.SearchTerm));
        }

        #endregion

        #region sort

        query = "desc".Equals(queryModel.SortOrder?.ToLower())
            ? query.OrderByDescending(GetSortProperty(queryModel))
            : query.OrderBy(GetSortProperty(queryModel));

        #endregion

        var model = query.Select(x => new ProductAttributeValueModel
        {
            ProductId = x.ProductId,
            AttributeId = x.AttributeId,
            Value = x.Value
        });

        #region paging

        var pPage = await PagedList<ProductAttributeValueModel>.CreateAsync(model, queryModel.Page,
            queryModel.PageSize);

        #endregion


        return ResponseModel.Success(ResponseConstants.Get("giá trị thuộc tính sản phẩm", pPage.TotalCount > 0), pPage);
    }

    public async Task<ResponseModel> AddProductAttributeValue(Guid pid, int aid, CreateUpdatePavModel model)
    {
        var isExistpid = await _proAttValueRepository.GetProductById(pid);
        if (isExistpid == null)
        {
            return ResponseModel.Success(ResponseConstants.NotFound("Sản phẩm"), null);
        }

        var isExistAttributeId = await _proAttValueRepository.GetAttributeById(aid);
        if (isExistAttributeId == null)
        {
            return ResponseModel.Success(ResponseConstants.NotFound("Thuộc tính"), null);
        }

        var isExistBoth = await _proAttValueRepository.GetProdAttValue(pid, aid);
        if (isExistBoth != null)
        {
            return ResponseModel.Success(ResponseConstants.Exist("Giá trị ứng với thuộc tính của sản phẩm"), null);
        }

        var entity = new ProductAttributeValue
        {
            ProductId = isExistpid.Id,
            AttributeId = isExistAttributeId.Id,
            Value = model.Value
        };
        _proAttValueRepository.Add(entity);
        var res = await _unitOfWork.SaveChangesAsync();
        if (res > 0)
        {
            return ResponseModel.Success(ResponseConstants.Create("giá trị của thuộc tính sản phẩm", true), null);
        }

        return ResponseModel.Error(ResponseConstants.Create("giá trị của thuộc tính sản phẩm", false));
    }

    public async Task<ResponseModel> UpdateProductAttributeValue(Guid pid, int aid, CreateUpdatePavModel model)
    {
        var isExist = await _proAttValueRepository.GetProdAttValue(pid, aid);
        if (isExist == null)
        {
            return ResponseModel.Success(ResponseConstants.NotFound("Sản phẩm và thuộc tính"), null);
        }

        if (!string.IsNullOrEmpty(model.Value))
        {
            var isExistValue = await _proAttValueRepository.GetProductAttributeValue()
                .FirstOrDefaultAsync(x => x.Value!.Equals(model.Value));
            if (isExistValue != null)
            {
                return ResponseModel.Success(ResponseConstants.Exist("Giá trị thuộc tính ứng với sản phẩm"), null);
            }

            isExist.Value = model.Value;
        }

        isExist.Value = isExist.Value;

        _proAttValueRepository.Update(isExist);
        var res = await _unitOfWork.SaveChangesAsync();
        if (res > 0)
        {
            return ResponseModel.Success(ResponseConstants.Update("giá trị thuộc tính", true), null);
        }

        return ResponseModel.Error(ResponseConstants.Update("giá trị thuộc tính", false));
    }

    public async Task<ResponseModel> DeleteProductAttributeValue(Guid pid, int aid)
    {
        var isExist = await _proAttValueRepository.GetProdAttValue(pid, aid);
        if (isExist == null)
        {
            return ResponseModel.Success(ResponseConstants.NotFound("Sản phẩm và thuộc tính"), null);
        }

        isExist.DeletedAt = DateTime.Now;
        _proAttValueRepository.Update(isExist);
        var res = await _unitOfWork.SaveChangesAsync();
        if (res > 0)
        {
            return ResponseModel.Success(ResponseConstants.Delete("giá trị của thuộc tính sản phẩm", true), null);
        }

        return ResponseModel.Error(ResponseConstants.Delete("giá trị của thuộc tính sản phẩm", false));
    }

    private Expression<Func<ProductAttributeValue, object>> GetSortProperty(
        ProductAttributeValueQueryModel queryModel)
        => queryModel.SortColumn?.ToLower() switch
        {
            "product_id" => s => s.ProductId,
            "attribute_id" => s => s.AttributeId,
            _ => s => s.Value!
        };
}