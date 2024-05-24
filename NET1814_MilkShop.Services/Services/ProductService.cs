using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Models.ProductModels;
using NET1814_MilkShop.Repositories.Repositories;
using NET1814_MilkShop.Repositories.UnitOfWork;
using NET1814_MilkShop.Services.CoreHelpers;
using System.Linq.Expressions;
using NET1814_MilkShop.Repositories.Models.BrandModels;

namespace NET1814_MilkShop.Services.Services
{
    public interface IProductService
    {
        Task<ResponseModel> GetProductsAsync(ProductQueryModel queryModel);
        Task<ResponseModel> GetBrandsAsync(BrandQueryModel queryModel);
        Task<ResponseModel> AddBrandAsync(BrandModel model);
    }

    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IBrandRepository _brandRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUnitRepository _unitRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IProductRepository productRepository,
            IBrandRepository brandRepository,
            ICategoryRepository categoryRepository,
            IUnitRepository unitRepository,
            IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _brandRepository = brandRepository;
            _categoryRepository = categoryRepository;
            _unitRepository = unitRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseModel> GetProductsAsync(ProductQueryModel queryModel)
        {
            var query = _productRepository.GetProductsQuery();

            #region Filter, Search

            //thu gọn thành 1 where thôi
            query = query.Where(p =>
                p.IsActive == queryModel.IsActive
                //search theo name, description, brand, unit, category
                && (string.IsNullOrEmpty(queryModel.SearchTerm) || p.Name.Contains(queryModel.SearchTerm)
                                                                || p.Description.Contains(queryModel.SearchTerm)
                                                                || p.Brand.Name.Contains(queryModel.SearchTerm)
                                                                || p.Unit.Name.Contains(queryModel.SearchTerm)
                                                                || p.Category.Name.Contains(queryModel.SearchTerm))
                //filter theo brand, category, unit, status, minPrice, maxPrice
                && (string.IsNullOrEmpty(queryModel.Brand) || string.Equals(p.Brand.Name, queryModel.Brand))
                && (string.IsNullOrEmpty(queryModel.Category) || string.Equals(p.Category.Name, queryModel.Category))
                && (string.IsNullOrEmpty(queryModel.Unit) || string.Equals(p.Unit.Name, queryModel.Unit))
                && (string.IsNullOrEmpty(queryModel.Status) || string.Equals(p.ProductStatus.Name, queryModel.Status))
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
                Message = products.TotalCount > 0 ? "Get products successfully" : "No products found",
                Status = "Success"
            };
        }

        public async Task<ResponseModel> GetBrandsAsync(BrandQueryModel queryModel)
        {
            var query = _brandRepository.GetBrandsQuery();

            #region filter

            if (!string.IsNullOrEmpty(queryModel.SearchTerm))
            {
                query = query.Where(x => x.Name.Contains(queryModel.SearchTerm));
            }

            if (!string.IsNullOrEmpty(queryModel.Description))
            {
                query = query.Where(x => x.Description.Contains(queryModel.Description));
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

            var brands = await PagedList<Brand>.CreateAsync(query, queryModel.Page, queryModel.PageSize);

            #endregion

            return new ResponseModel()
            {
                Data = brands,
                Message = brands.TotalCount > 0 ? "Get brands successfully" : "No brands found",
                Status = "Success"
            };
        }

        public async Task<ResponseModel> AddBrandAsync(BrandModel model)
        {
            // var isExistId = await _brandRepository.GetById(model.Id);
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
                return new ResponseModel
                {
                    Message = "Brand name is existed! Add new brand fail!",
                    Status = "Error"
                };
            }

            var entity = new Brand
            {
                Name = model.Name,
                Description = model.Description,
                IsActive = true
            };
            _brandRepository.Add(entity);
            await _unitOfWork.SaveChangesAsync();
            return new ResponseModel
            {
                Status = "Success",
                Data = entity,
                Message = "Add new brand successfully"
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

        private static Expression<Func<Brand, object>> GetSortBrandProperty(
            BrandQueryModel queryModel
        ) => queryModel.SortColumn?.ToLower() switch
        {
            "description" => product => product.Description,
            _ => product => product.Name
        };
    }
}