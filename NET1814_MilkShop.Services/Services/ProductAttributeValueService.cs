using System.Linq.Expressions;
using System.Runtime.Intrinsics.X86;
using Microsoft.EntityFrameworkCore;
using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Models.ProductAttributeValueModels;
using NET1814_MilkShop.Repositories.Repositories;
using NET1814_MilkShop.Repositories.UnitOfWork;
using NET1814_MilkShop.Services.CoreHelpers;

namespace NET1814_MilkShop.Services.Services;

public interface IProductAttributeValueService
{
    Task<ResponseModel> GetProductAttributeValue(ProductAttributeValueQueryModel queryModel);
    Task<ResponseModel> AddProductAttributeValue(Guid pid, int aid, CreateUpdatePavModel model);
    Task<ResponseModel> UpdateProductAttributeValue(Guid pid, int aid, CreateUpdatePavModel model);
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

    public async Task<ResponseModel> GetProductAttributeValue(ProductAttributeValueQueryModel queryModel)
    {
        var query = _proAttValueRepository.GetProductAttributeValue();

        #region filter

        if (!string.IsNullOrEmpty(queryModel.ProductId.ToString()))
        {
            query = query.Where(x => x.ProductId == queryModel.ProductId);
        }

        if (!string.IsNullOrEmpty(queryModel.AttributeId.ToString()))
        {
            query = query.Where(x => x.AttributeId == queryModel.AttributeId);
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

        return new ResponseModel
        {
            Data = pPage,
            Message = pPage.TotalCount > 0
                ? "Tìm kiếm thành công"
                : "Danh sách rỗng",
            Status = "Success"
        };
    }

    public async Task<ResponseModel> AddProductAttributeValue(Guid pid, int aid, CreateUpdatePavModel model)
    {
        var isExistpid = await _proAttValueRepository.GetProductById(pid);
        if (isExistpid == null)
        {
            return new ResponseModel
            {
                Message = "Không tồn tại sản phẩm",
                Status = "Error"
            };
        }

        var isExistAttributeId = await _proAttValueRepository.GetAttributeById(aid);
        if (isExistAttributeId == null)
        {
            return new ResponseModel
            {
                Message = "Không tồn tại thuộc tính",
                Status = "Error"
            };
        }

        var isExistBoth = await _proAttValueRepository.GetProdAttValue(pid, aid);
        if (isExistBoth != null)
        {
            return new ResponseModel
            {
                Message = "Đã tồn tại giá trị ứng với thuộc tính của sản phẩm",
                Status = "Error"
            };
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
            return new ResponseModel
            {
                Message = "Thêm giá trị của thuộc tính sản phẩm thành công",
                Status = "Success"
            };
        }

        return new ResponseModel
        {
            Message = "Thêm giá trị của thuộc tính sản phẩm thất bại",
            Status = "Success"
        };
    }

    public async Task<ResponseModel> UpdateProductAttributeValue(Guid pid, int aid, CreateUpdatePavModel model)
    {
        var isExist = await _proAttValueRepository.GetProdAttValue(pid, aid);
        if (isExist == null)
        {
            return new ResponseModel
            {
                Status = "Error",
                Message = "Không tồn tại sản phẩm và thuộc tính"
            };
        }

        if (!string.IsNullOrEmpty(model.Value))
        {
            var isExistValue = await _proAttValueRepository.GetProductAttributeValue()
                .FirstOrDefaultAsync(x => x.Value!.Equals(model.Value));
            if (isExistValue != null)
            {
                return new ResponseModel
                {
                    Message = "Tồn tại giá trị thuộc tính ứng với sản phẩm",
                    Status = "Error"
                };
            }

            isExist.Value = model.Value;
        }

        isExist.Value = isExist.Value;

        _proAttValueRepository.Update(isExist);
        var res = await _unitOfWork.SaveChangesAsync();
        if (res > 0)
        {
            return new ResponseModel
            {
                Message = "Cập nhật giá trị thuộc tính thành công",
                Status = "Success"
            };
        }

        return new ResponseModel
        {
            Message = "Cập nhật giá trị thuộc tính thất bại",
            Status = "Success"
        };
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