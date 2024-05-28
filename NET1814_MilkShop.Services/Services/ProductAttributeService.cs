using System.Linq.Expressions;
using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Models.ProductAttributeModels;
using NET1814_MilkShop.Repositories.Repositories;
using NET1814_MilkShop.Repositories.UnitOfWork;
using NET1814_MilkShop.Services.CoreHelpers;

namespace NET1814_MilkShop.Services.Services
{
    public interface IProductAttributeService
    {
        Task<ResponseModel> GetProductAttributesAsync(ProductAttributeQueryModel queryModel);
        Task<ResponseModel> AddProductAttributeAsync(CreateProductAttributeModel model);
        Task<ResponseModel> UpdateProductAttributeAsync(int id, CreateProductAttributeModel model);
    }

    public class ProductAttributeService : IProductAttributeService
    {
        private readonly IProductAttributeRepository _productAttribute;
        private readonly IUnitOfWork _unitOfWork;

        public ProductAttributeService(IProductAttributeRepository productAttribute, IUnitOfWork unitOfWork)
        {
            _productAttribute = productAttribute;
            _unitOfWork = unitOfWork;
        }


        public async Task<ResponseModel> GetProductAttributesAsync(ProductAttributeQueryModel queryModel)
        {
            var query = _productAttribute.GetProductAttributes();

            #region filter

            if (queryModel.IsActive == false)
            {
                query = query.Where(x => x.IsActive == false);
            }

            if (!string.IsNullOrEmpty(queryModel.SearchTerm))
            {
                query = query.Where(x =>
                    x.Name.Contains(queryModel.SearchTerm) || x.Description.Contains(queryModel.SearchTerm));
            }

            #endregion

            #region sort

            query = "desc".Equals(queryModel.SortOrder?.ToLower())
                ? query.OrderByDescending(GetSortProperty(queryModel))
                : query.OrderBy(GetSortProperty(queryModel));

            #endregion

            var model = query.Select(x => new ProductAttributeModel
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description
            });

            #region paging

            var pPage = await PagedList<ProductAttribute>.CreateAsync(query, queryModel.Page, queryModel.PageSize);

            #endregion

            return new ResponseModel()
            {
                Data = pPage,
                Message = pPage.TotalCount > 0 ? "Get brands successfully" : "No brands found",
                Status = "Success"
            };
        }

        public async Task<ResponseModel> AddProductAttributeAsync(CreateProductAttributeModel model)
        {
            var isExistName = await _productAttribute.GetProductAttributeByName(model.Name);
            if (isExistName != null)
            {
                return new ResponseModel
                {
                    Message = "Thuộc tính sản phẩm đã tồn tại! Thêm một thuộc tính mới thất bại!",
                    Status = "Error"
                };
            }

            var entity = new ProductAttribute
            {
                Name = model.Name,
                Description = model.Description,
                IsActive = true
            };
            _productAttribute.Add(entity);
            await _unitOfWork.SaveChangesAsync();
            return new ResponseModel
            {
                Status = "Success",
                Data = entity,
                Message = "Thêm mới thuộc tính sản phẩm thành công!"
            };
        }

        public Task<ResponseModel> UpdateProductAttributeAsync(int id, CreateProductAttributeModel model)
        {
            throw new NotImplementedException();
        }

        private Expression<Func<ProductAttribute, object>> GetSortProperty(ProductAttributeQueryModel queryModel)
            => queryModel.SortColumn?.ToLower() switch
            {
                "name" => p => p.Name,
                _ => p => p.Id
            };
    }
}