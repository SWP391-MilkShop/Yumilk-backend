using System.Linq.Expressions;
using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Models.ProductModels;
using NET1814_MilkShop.Repositories.Repositories;
using NET1814_MilkShop.Repositories.UnitOfWork;
using NET1814_MilkShop.Services.CoreHelpers;

namespace NET1814_MilkShop.Services.Services
{
    public interface IProductService
    {
        Task<ResponseModel> GetProductsAsync(ProductQueryModel queryModel);
    }

    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IProductRepository productRepository, IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseModel> GetProductsAsync(ProductQueryModel queryModel)
        {
            var query = _productRepository.GetProductsQuery().Where(p => p.IsActive == queryModel.IsActive);
            // filter
            if (!string.IsNullOrEmpty(queryModel.SearchTerm))
            {
                query = query.Where(p => p.Name.Contains(queryModel.SearchTerm));
            }
            if (!string.IsNullOrEmpty(queryModel.Brand))
            {
                query = query.Where(p => string.Equals(p.Brand!.Name, queryModel.Brand));
            }
            if (!string.IsNullOrEmpty(queryModel.Category))
            {
                query = query.Where(p => string.Equals(p.Category!.Name, queryModel.Category));
            }
            if (!string.IsNullOrEmpty(queryModel.Unit))
            {
                query = query.Where(p => string.Equals(p.Unit!.Name, queryModel.Unit));
            }
            if (!string.IsNullOrEmpty(queryModel.Status))
            {
                query = query.Where(p => string.Equals(p.ProductStatus!.Name, queryModel.Status));
            }
            // sort
            if ("desc".Equals(queryModel.SortOrder?.ToLower()))
            {
                query = query.OrderByDescending(GetSortProperty(queryModel));
            }
            else
            {
                query = query.OrderBy(GetSortProperty(queryModel));
            }
            // convert to ProductModel
            var productModelQuery = query.Select(p => new ProductModel
            {
                Id = p.Id.ToString(),
                Name = p.Name,
                Description = p.Description,
                Quantity = p.Quantity,
                Brand = p.Brand!.Name,
                Category = p.Category!.Name,
                Unit = p.Unit!.Name,
                OriginalPrice = p.OriginalPrice,
                SalePrice = p.SalePrice,
                Status = p.ProductStatus!.Name,
            });
            // paging
            var products = await PagedList<ProductModel>.CreateAsync(
                productModelQuery,
                queryModel.Page,
                queryModel.PageSize
            );
            return new ResponseModel
            {
                Data = products,
                Message =
                    products.TotalCount > 0 ? "Get products successfully" : "No products found",
                Status = "Success"
            };
        }

        /// <summary>
        /// Get sort property as expression
        /// </summary>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        private static Expression<Func<Product, object>> GetSortProperty(
            ProductQueryModel queryModel
        ) =>
            queryModel.SortColumn?.ToLower() switch
            {
                "name" => product => product.Name,
                "saleprice" => product => product.SalePrice,
                "quantity" => product => product.Quantity,
                _ => product => product.Id,
            };
    }
}
