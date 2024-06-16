using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NET1814_MilkShop.Repositories.CoreHelpers.Constants;
using NET1814_MilkShop.Repositories.CoreHelpers.Enum;
using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Models.PreorderModels;
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
        Task<ResponseModel> GetProductByIdAsync(Guid id);
        Task<ResponseModel> CreateProductAsync(CreateProductModel model);
        Task<ResponseModel> UpdateProductAsync(Guid id, UpdateProductModel model);
        /// <summary>
        /// Delete product by id
        /// <para>Also delete related preorder product if exists</para>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ResponseModel> DeleteProductAsync(Guid id);
        Task<ResponseModel> GetProductStatsAsync(ProductStatsQueryModel queryModel);
        Task<ResponseModel> UpdatePreorderProductAsync(Guid productId, UpdatePreorderProductModel model);
    }

    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IBrandRepository _brandRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUnitRepository _unitRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IPreorderProductRepository _preorderProductRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(
            IProductRepository productRepository,
            IBrandRepository brandRepository,
            ICategoryRepository categoryRepository,
            IUnitRepository unitRepository,
            IUnitOfWork unitOfWork,
            IOrderDetailRepository orderDetailRepository,
            IPreorderProductRepository preorderProductRepository)
        {
            _productRepository = productRepository;
            _brandRepository = brandRepository;
            _categoryRepository = categoryRepository;
            _unitRepository = unitRepository;
            _orderDetailRepository = orderDetailRepository;
            _preorderProductRepository = preorderProductRepository;
            _unitOfWork = unitOfWork;
        }

        private static ProductModel ToProductModel(Product product) =>
            new ProductModel
            {
                Id = product.Id.ToString(),
                Name = product.Name,
                Description = product.Description,
                Quantity = product.Quantity,
                BrandId = product.BrandId,
                Brand = product.Brand!.Name,
                CategoryId = product.CategoryId,
                Category = product.Category!.Name,
                UnitId = product.UnitId,
                Unit = product.Unit!.Name,
                OriginalPrice = product.OriginalPrice,
                SalePrice = product.SalePrice,
                StatusId = product.StatusId,
                Status = product.ProductStatus!.Name,
                Thumbnail = product.Thumbnail,
                AverageRating = product.ProductReviews.IsNullOrEmpty() ? 0 : product.ProductReviews.Average(pr => (double)pr.Rating),
                OrderCount = product.OrderDetails.IsNullOrEmpty() ? 0 : product.OrderDetails.Sum(od => od.Quantity),
                IsActive = product.IsActive,
                CreatedAt = product.CreatedAt
            };
        private static PreorderProductModel ToPreorderProductModel(Product product) =>
            new PreorderProductModel
            {
                Id = product.Id.ToString(),
                Name = product.Name,
                Description = product.Description,
                Quantity = product.Quantity,
                BrandId = product.BrandId,
                Brand = product.Brand!.Name,
                CategoryId = product.CategoryId,
                Category = product.Category!.Name,
                UnitId = product.UnitId,
                Unit = product.Unit!.Name,
                OriginalPrice = product.OriginalPrice,
                SalePrice = product.SalePrice,
                StatusId = product.StatusId,
                Status = product.ProductStatus!.Name,
                Thumbnail = product.Thumbnail,
                AverageRating = product.ProductReviews.IsNullOrEmpty() ? 0 : product.ProductReviews.Average(pr => (double)pr.Rating),
                OrderCount = product.OrderDetails.IsNullOrEmpty() ? 0 : product.OrderDetails.Sum(od => od.Quantity),
                MaxPreOrderQuantity = product.PreorderProduct.MaxPreOrderQuantity,
                StartDate = product.PreorderProduct.StartDate,
                EndDate = product.PreorderProduct.EndDate,
                ExpectedPreOrderDays = product.PreorderProduct.ExpectedPreOrderDays,
                IsActive = product.IsActive,
                CreatedAt = product.CreatedAt
            };
        public async Task<ResponseModel> GetProductsAsync(ProductQueryModel queryModel)
        {
            //normalize search term, brand, category, unit, status
            var searchTerm = StringExtension.Normalize(queryModel.SearchTerm);
            var brand = StringExtension.Normalize(queryModel.Brand);
            var category = StringExtension.Normalize(queryModel.Category);
            var unit = StringExtension.Normalize(queryModel.Unit);
            var minPrice = queryModel.MinPrice <= 0 ? 0 : queryModel.MinPrice;
            var maxPrice = queryModel.MaxPrice <= 0 ? 0 : queryModel.MaxPrice;
            var status = StringExtension.Normalize(queryModel.Status);
            status = status?.RemoveSpace();
            var query = _productRepository.GetProductsQuery(includeRating: true, includeOrderCount: true);
            if (status == "preorder")
            {
                query = query.Include(p => p.PreorderProduct);
            }
            #region Filter, Search
            //thu gọn thành 1 where thôi
            //filter theo status (default is selling)
            query = query.Where(GetStatusProperty(status));
            query = query.Where(p =>
                (queryModel.IsActive.HasValue ? p.IsActive == queryModel.IsActive.Value : true)
                //search theo name, description, brand, unit, category
                && (
                    string.IsNullOrEmpty(searchTerm)
                    || p.Name.Contains(searchTerm)
                    || p.Description!.Contains(searchTerm)
                    || p.Brand!.Name.Contains(searchTerm)
                    || p.Unit!.Name.Contains(searchTerm)
                    || p.Category!.Name.Contains(searchTerm)
                )
                //filter theo brand, category, unit, status, minPrice, maxPrice
                && (string.IsNullOrEmpty(brand) || p.Brand!.Name == brand)
                && (string.IsNullOrEmpty(category) || p.Category!.Name == category)
                && (string.IsNullOrEmpty(unit) || p.Unit!.Name == unit)
                && (minPrice == 0 || (p.SalePrice == 0 ? p.OriginalPrice >= minPrice : p.SalePrice >= minPrice))
                && (maxPrice == 0 || (p.SalePrice == 0 ? p.OriginalPrice <= maxPrice : p.SalePrice <= maxPrice))
            );

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
            #region Pagination
            if (status == "preorder")
            {
                var productModelQuery = query.Select(p => ToPreorderProductModel(p));
                var products = await PagedList<PreorderProductModel>.CreateAsync(
                    productModelQuery,
                    queryModel.Page,
                    queryModel.PageSize
                );
                return ResponseModel.Success(ResponseConstants.Get("sản phẩm đặt trước", products.TotalCount > 0), products);
            }
            else
            {
                var productModelQuery = query.Select(p => ToProductModel(p));
                var products = await PagedList<ProductModel>.CreateAsync(
                    productModelQuery,
                    queryModel.Page,
                    queryModel.PageSize
                );
                return ResponseModel.Success(ResponseConstants.Get("sản phẩm", products.TotalCount > 0), products);
            }
            #endregion
        }

        /// <summary>
        /// Get sort property as expression
        /// </summary>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        private static Expression<Func<Product, object>> GetSortProperty(
            ProductQueryModel queryModel
        ) =>
            queryModel.SortColumn?.ToLower().Replace(" ", "") switch
            {
                "name" => p => p.Name,
                "saleprice" => p => p.SalePrice == 0 ? p.OriginalPrice : p.SalePrice,
                "quantity" => p => p.Quantity,
                "createdat" => p => p.CreatedAt,
                "rating" => p => p.ProductReviews.Average(pr => (double)pr.Rating),
                "ordercount" => p => p.OrderDetails.Sum(od => od.Quantity),
                _ => product => product.Id,
            };
        private static Expression<Func<Product, bool>> GetStatusProperty(
            string? status
        )
        {
            // If status is null, default to an empty string to match the default case in the switch expression
            var normalizedStatus = status ?? "";
            return normalizedStatus switch
            {
                "selling" => p => p.StatusId == (int)ProductStatusId.SELLING,
                "preorder" => p => p.StatusId == (int)ProductStatusId.PRE_ORDER,
                "outofstock" => p => p.StatusId == (int)ProductStatusId.OUT_OF_STOCK,
                _ => p => p.StatusId == (int)ProductStatusId.SELLING,
            };
        }
        public async Task<ResponseModel> GetProductByIdAsync(Guid id)
        {
            var product = await _productRepository.GetByIdAsync(id, includeRating: true, includeOrderCount: true);
            if (product == null)
            {
                return ResponseModel.BadRequest(ResponseConstants.NotFound("Sản phẩm"));
            }
            return ResponseModel.Success(
                ResponseConstants.Get("sản phẩm", true),
                ToProductModel(product)
            );
        }

        public async Task<ResponseModel> CreateProductAsync(CreateProductModel model)
        {
            var validateCommonResponse = ValidateCommon(model.SalePrice, model.OriginalPrice, model.Quantity, model.StatusId, model.Thumbnail);
            if (validateCommonResponse != null)
            {
                return validateCommonResponse;
            }
            var validateIdResponse = await ValidateId(model.BrandId, model.CategoryId, model.UnitId);
            if (validateIdResponse != null)
            {
                return validateIdResponse;
            }
            var existing = await _productRepository.GetByNameAsync(model.Name);
            if (existing != null)
            {
                return ResponseModel.BadRequest(ResponseConstants.Exist("Tên sản phẩm"));
            }
            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = model.Name,
                Description = model.Description,
                Quantity = model.Quantity,
                OriginalPrice = model.OriginalPrice,
                SalePrice = model.SalePrice,
                BrandId = model.BrandId,
                CategoryId = model.CategoryId,
                UnitId = model.UnitId,
                StatusId = model.StatusId,
                IsActive = false, // default is unpublished
                Thumbnail = model.Thumbnail
            };
            //add preorder product if status is preorder
            if (model.StatusId == (int)ProductStatusId.PRE_ORDER)
            {
                var preorderProduct = new PreorderProduct
                {
                    ProductId = product.Id,
                };
                _preorderProductRepository.Add(preorderProduct);
            }
            _productRepository.Add(product);
            var result = await _unitOfWork.SaveChangesAsync();
            if (result > 0)
            {
                return ResponseModel.Success(ResponseConstants.Create("sản phẩm", true), new
                {
                    ProductId = product.Id
                });
            }
            return ResponseModel.Error(ResponseConstants.Create("sản phẩm", false));
        }

        public async Task<ResponseModel> UpdateProductAsync(Guid id, UpdateProductModel model)
        {
            var product = await _productRepository.GetByIdNoIncludeAsync(id);
            if (product == null)
            {
                return ResponseModel.BadRequest(ResponseConstants.NotFound("Sản phẩm"));
            }
            //add preorder product if status is preorder and no preorder product exists
            if (model.StatusId == (int)ProductStatusId.PRE_ORDER)
            {
                var existing = await _preorderProductRepository.GetByIdAsync(id);
                if (existing == null)
                {
                    var preorderProduct = new PreorderProduct
                    {
                        ProductId = product.Id
                    };
                    _preorderProductRepository.Add(preorderProduct);
                }
            }
            if (!string.IsNullOrEmpty(model.Name))
            {
                var productByName = await _productRepository.GetByNameAsync(model.Name);
                // check if product with the same name exists and not the current product
                if (productByName != null && productByName.Id != id)
                {
                    return ResponseModel.BadRequest(ResponseConstants.Exist("Tên sản phẩm"));
                }
                product.Name = model.Name;
            }
            var validateResponse = await ValidateId(model.BrandId, model.CategoryId, model.UnitId);
            if (validateResponse != null)
            {
                return validateResponse;
            }
            product.BrandId = model.BrandId == 0 ? product.BrandId : model.BrandId;
            product.CategoryId = model.CategoryId == 0 ? product.CategoryId : model.CategoryId;
            product.UnitId = model.UnitId == 0 ? product.UnitId : model.UnitId;
            product.StatusId = model.StatusId == 0 ? product.StatusId : model.StatusId;
            product.Description = string.IsNullOrEmpty(model.Description)
                ? product.Description
                : model.Description;
            product.Quantity = model.Quantity ?? product.Quantity;
            product.OriginalPrice = model.OriginalPrice ?? product.OriginalPrice;
            product.SalePrice = model.SalePrice ?? product.SalePrice;
            var validateCommonResponse = ValidateCommon(product.SalePrice, product.OriginalPrice, product.Quantity, product.StatusId, model.Thumbnail);
            if (validateCommonResponse != null)
            {
                return validateCommonResponse;
            }
            product.Thumbnail = string.IsNullOrEmpty(model.Thumbnail) ? product.Thumbnail : model.Thumbnail;
            product.IsActive = model.IsActive;
            _productRepository.Update(product);
            var result = await _unitOfWork.SaveChangesAsync();
            if (result > 0)
            {
                return ResponseModel.Success(ResponseConstants.Update("sản phẩm", true), null);
            }
            return ResponseModel.Error(ResponseConstants.Update("sản phẩm", false));
        }

        public async Task<ResponseModel> DeleteProductAsync(Guid id)
        {
            var product = await _productRepository.GetByIdNoIncludeAsync(id);
            if (product == null)
            {
                return ResponseModel.Success(ResponseConstants.NotFound("Sản phẩm"), null);
            }
            var preorderProduct = await _preorderProductRepository.GetByIdAsync(id);
            //delete preorder product if exists
            if (preorderProduct != null)
            {
                _preorderProductRepository.Delete(preorderProduct);
            }
            _productRepository.Delete(product);
            var result = await _unitOfWork.SaveChangesAsync();
            if (result > 0)
            {
                return ResponseModel.Success(ResponseConstants.Delete("sản phẩm", true), null);
            }
            return ResponseModel.Error(ResponseConstants.Delete("sản phẩm", false));
        }
        public async Task<ResponseModel> GetProductStatsAsync(ProductStatsQueryModel queryModel)
        {
            var from = queryModel.From ?? DateTime.Now.AddDays(-30);
            var to = queryModel.To ?? DateTime.Now;
            var query = _productRepository.GetProductQueryNoInclude().Where(p => p.CreatedAt >= from && p.CreatedAt <= to);
            var orderDetailQuery = _orderDetailRepository.GetOrderDetailQuery()
                .Where(od => od.CreatedAt >= from && od.CreatedAt <= to
                && od.Order.StatusId == (int)OrderStatusId.DELIVERED);
            //get total products sold
            var totalProductsSold = await orderDetailQuery.SumAsync(o => o.Quantity);
            //get total products sold per category
            var categoryQuery = query.Include(p => p.Category);
            var totalSoldPerCategory = await categoryQuery.Join(orderDetailQuery, p => p.Id, od => od.ProductId, (p, od) => new { p.Category!.Name, od.Quantity })
            .GroupBy(x => x.Name)
            .Select(g => new { Category = g.Key, TotalSold = g.Sum(x => x.Quantity) })
            .ToDictionaryAsync(x => x.Category, x => x.TotalSold);
            //get total products sold per brand
            var brandQuery = query.Include(p => p.Brand);
            var totalSoldPerBrand = await brandQuery.Join(orderDetailQuery, p => p.Id, od => od.ProductId, (p, od) => new { p.Brand!.Name, od.Quantity })
            .GroupBy(x => x.Name)
            .Select(g => new { Brand = g.Key, TotalSold = g.Sum(x => x.Quantity) })
            .ToDictionaryAsync(x => x.Brand, x => x.TotalSold);
            var stats = new ProductStatsModel
            {
                TotalSold = totalProductsSold,
                TotalSoldPerCategory = totalSoldPerCategory,
                TotalSoldPerBrand = totalSoldPerBrand
            };
            return ResponseModel.Success(ResponseConstants.Get("thống kê sản phẩm", true), stats);
        }

        public async Task<ResponseModel> UpdatePreorderProductAsync(Guid productId, UpdatePreorderProductModel model)
        {
            if (model.StartDate > model.EndDate)
            {
                return ResponseModel.BadRequest(ResponseConstants.InvalidFilterDate);
            }
            var isExist = await _productRepository.IsExistAsync(productId);
            if (!isExist)
            {
                return ResponseModel.BadRequest(ResponseConstants.NotFound("Sản phẩm"));
            }
            var preorderProduct = await _preorderProductRepository.GetByIdAsync(productId);
            if (preorderProduct == null)
            {
                return ResponseModel.BadRequest(ResponseConstants.NotFound("Sản phẩm đặt trước"));
            }
            // update only if there is a value
            preorderProduct.MaxPreOrderQuantity = model.MaxPreOrderQuantity;
            preorderProduct.StartDate = model.StartDate;
            preorderProduct.EndDate = model.EndDate;
            preorderProduct.ExpectedPreOrderDays = model.ExpectedPreOrderDays;
            _preorderProductRepository.Update(preorderProduct);
            var result = await _unitOfWork.SaveChangesAsync();
            if (result > 0)
            {
                return ResponseModel.Success(ResponseConstants.Update("sản phẩm đặt trước", true), null);
            }
            return ResponseModel.Error(ResponseConstants.Update("sản phẩm đặt trước", false));
        }
        #region Validation
        /// <summary>
        /// Validate brand, category, unit id if they are valid
        /// </summary>
        /// <param name="brandId"></param>
        /// <param name="categoryId"></param>
        /// <param name="unitId"></param>
        /// <returns></returns>
        private async Task<ResponseModel?> ValidateId(int brandId, int categoryId, int unitId)
        {
            if (brandId != 0)
            {
                var brand = await _brandRepository.GetByIdAsync(brandId);
                if (brand == null)
                {
                    return ResponseModel.BadRequest(ResponseConstants.NotFound("Thương hiệu"));
                }
            }
            if (categoryId != 0)
            {
                var category = await _categoryRepository.GetByIdAsync(categoryId);
                if (category == null)
                {
                    return ResponseModel.BadRequest(ResponseConstants.NotFound("Danh mục"));
                }
            }
            if (unitId != 0)
            {
                var unit = await _unitRepository.GetByIdAsync(unitId);
                if (unit == null)
                {
                    return ResponseModel.BadRequest(ResponseConstants.NotFound("Đơn vị"));
                }
            }
            return null;
        }
        /// <summary>
        /// Validate common fields and logic
        /// </summary>
        /// <param name="salePrice"></param>
        /// <param name="originalPrice"></param>
        /// <param name="quantity"></param>
        /// <param name="statusId"></param>
        /// <param name="thumbnail"></param>
        /// <returns></returns>
        private ResponseModel? ValidateCommon(int salePrice, int originalPrice, int quantity, int statusId, string? thumbnail)
        {
            if (!string.IsNullOrEmpty(thumbnail) && !Uri.IsWellFormedUriString(thumbnail, UriKind.Absolute))
            {
                return ResponseModel.BadRequest(ResponseConstants.WrongFormat("URL"));
            }
            if (salePrice > originalPrice)
            {
                return ResponseModel.BadRequest(ResponseConstants.InvalidSalePrice);
            }
            if (statusId == (int)ProductStatusId.SELLING && quantity == 0)
            {
                return ResponseModel.BadRequest(ResponseConstants.InvalidQuantity);
            }
            if (statusId == (int)ProductStatusId.PRE_ORDER && quantity != 0)
            {
                return ResponseModel.BadRequest(ResponseConstants.NoQuantityPreorder);
            }
            return null;
        }
        #endregion

    }
}
