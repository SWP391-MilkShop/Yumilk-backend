using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Models.ProductModels;
using NET1814_MilkShop.Repositories.Repositories;
using NET1814_MilkShop.Repositories.UnitOfWork;
using NET1814_MilkShop.Services.CoreHelpers;
using NET1814_MilkShop.Services.CoreHelpers.Extensions;
using System.Linq.Expressions;

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

        public ProductService(IProductRepository productRepository,
            IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseModel> GetProductsAsync(ProductQueryModel queryModel)
        {
            var query = _productRepository.GetProductsQuery();
            //Normalize search term, brand, category, unit, status
            var searchTerm = StringExtension.Normalize(queryModel.SearchTerm);
            var brand = StringExtension.Normalize(queryModel.Brand);
            var category = StringExtension.Normalize(queryModel.Category);
            var unit = StringExtension.Normalize(queryModel.Unit);
            var status = StringExtension.Normalize(queryModel.Status);
            #region Filter, Search

            //thu gọn thành 1 where thôi
            query = query.Where(p =>
                p.IsActive == queryModel.IsActive
                //search theo name, description, brand, unit, category
                && (string.IsNullOrEmpty(searchTerm) || p.Name.ToLower().Contains(searchTerm)
                                                                || p.Description!.ToLower().Contains(searchTerm)
                                                                || p.Brand!.Name.ToLower().Contains(searchTerm)
                                                                || p.Unit!.Name.ToLower().Contains(searchTerm)
                                                                || p.Category!.Name.ToLower().Contains(searchTerm))
                //filter theo brand, category, unit, status, minPrice, maxPrice
                && (string.IsNullOrEmpty(brand) || string.Equals(p.Brand!.Name.ToLower(), brand))
                && (string.IsNullOrEmpty(category) || string.Equals(p.Category!.Name.ToLower(), category))
                && (string.IsNullOrEmpty(unit) || string.Equals(p.Unit!.Name.ToLower(), unit))
                && (string.IsNullOrEmpty(status) || string.Equals(p.ProductStatus!.Name.ToLower(), status))
                && (queryModel.MinPrice <= 0 || p.SalePrice >= queryModel.MinPrice)
                && (queryModel.MaxPrice <= 0 || p.SalePrice <= queryModel.MaxPrice));
            /*if (!string.IsNullOrEmpty(queryModel.SearchTerm))
            {
                query = query.Where(p => p.Name.Contains(queryModel.SearchTerm)
                        || p.Description!.Contains(queryModel.SearchTerm)
                        || p.Brand!.Name.Contains(queryModel.SearchTerm)
                        || p.Unit!.Name.Contains(queryModel.SearchTerm)
                        || p.Category!.Name.Contains(queryModel.SearchTerm));
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
            }*/

            #endregion

            #region Sort

            if ("desc".Equals(queryModel.SortOrder?.ToLower()))
            {
                query = query.OrderByDescending(GetSortProperty(queryModel));
            }
            else
            {
                query = query.OrderBy(GetSortProperty(queryModel));
            }

            #endregion

            #region Convert to ProductModel

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

            #endregion

            #region Pagination

            var products = await PagedList<ProductModel>.CreateAsync(
                productModelQuery,
                queryModel.Page,
                queryModel.PageSize
            );

            #endregion

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