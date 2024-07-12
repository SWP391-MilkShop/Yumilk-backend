using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NET1814_MilkShop.Repositories.CoreHelpers.Constants;
using NET1814_MilkShop.Repositories.CoreHelpers.Enum;
using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Models.PreorderModels;
using NET1814_MilkShop.Repositories.Models.ProductModels;
using NET1814_MilkShop.Repositories.Repositories.Interfaces;
using NET1814_MilkShop.Repositories.UnitOfWork.Interfaces;
using NET1814_MilkShop.Services.CoreHelpers;
using NET1814_MilkShop.Services.CoreHelpers.Extensions;
using NET1814_MilkShop.Services.Services.Interfaces;

namespace NET1814_MilkShop.Services.Services.Implementations;

public class ProductService : IProductService
{
    private readonly IBrandRepository _brandRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IOrderDetailRepository _orderDetailRepository;
    private readonly IPreorderProductRepository _preorderProductRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUnitRepository _unitRepository;

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

    public async Task<ResponseModel> GetProductsAsync(ProductQueryModel queryModel)
    {
        if (queryModel.MinPrice > queryModel.MaxPrice && queryModel.MaxPrice != 0)
        {
            return ResponseModel.BadRequest(" Giá nhỏ nhất phải nhỏ hơn giá lớn nhất");
        }

        //normalize search term, brand, category, unit, status
        var searchTerm = StringExtension.Normalize(queryModel.SearchTerm);
        HashSet<int> categoryIds = [];
        var categories = await _categoryRepository.GetCategoriesQuery()
            .Where(c => c.ParentId != null)
            .ToListAsync();
        foreach (var categoryId in queryModel.CategoryIds)
        {
            var childCategoryIds = _categoryRepository.GetChildCategoryIds(categoryId, categories);
            categoryIds.UnionWith(childCategoryIds);
        }

        var minPrice = queryModel.MinPrice;
        var maxPrice = queryModel.MaxPrice;
        var query = _productRepository.GetProductsQuery(true, true);
        if (queryModel.StatusIds.Contains((int)ProductStatusId.Preorder))
        {
            query = query.Include(p => p.PreorderProduct);
        }

        #region Filter, Search

        //thu gọn thành 1 where thôi
        query = query.Where(p =>
            (!queryModel.IsActive.HasValue || p.IsActive == queryModel.IsActive.Value)
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
            && (categoryIds.IsNullOrEmpty() || categoryIds.Contains(p.CategoryId))
            && (queryModel.BrandIds.IsNullOrEmpty() || queryModel.BrandIds.Contains(p.BrandId))
            && (queryModel.UnitIds.IsNullOrEmpty() || queryModel.UnitIds.Contains(p.UnitId))
            && (queryModel.StatusIds.IsNullOrEmpty() || queryModel.StatusIds.Contains(p.StatusId))
            && (minPrice == 0 || (p.SalePrice == 0 ? p.OriginalPrice >= minPrice : p.SalePrice >= minPrice))
            && (maxPrice == 0 || (p.SalePrice == 0 ? p.OriginalPrice <= maxPrice : p.SalePrice <= maxPrice))
            // filter product on sale
            && (!queryModel.OnSale || p.SalePrice != 0)
            //filter by active brand, category, unit
            && (p.Brand!.IsActive && p.Category!.IsActive && p.Unit!.IsActive)
        );

        #endregion

        #region Sort

        if ("desc".Equals(queryModel.SortOrder?.ToLower()))
            query = query.OrderByDescending(GetSortProperty(queryModel.SortColumn));
        else
            query = query.OrderBy(GetSortProperty(queryModel.SortColumn));

        #endregion

        #region Pagination

        var productModelQuery = query.Select(p => ToProductModel(p));
        var products = await PagedList<ProductModel>.CreateAsync(
            productModelQuery,
            queryModel.Page,
            queryModel.PageSize
        );
        return ResponseModel.Success(ResponseConstants.Get("sản phẩm", products.TotalCount > 0), products);

        #endregion
    }

    /// <summary>
    /// Get product by id
    /// <para>Return preorder product if status is preordered</para>
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<ResponseModel> GetProductByIdAsync(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id, true, true);
        if (product == null) return ResponseModel.BadRequest(ResponseConstants.NotFound("Sản phẩm"));
        // if (product.StatusId != (int)ProductStatusId.Preorder)
        //     return ResponseModel.Success(
        //         ResponseConstants.Get("sản phẩm", true),
        //         ToProductModel(product)
        //     );
        var preorderProduct = await _preorderProductRepository.GetByIdAsync(id);
        product.PreorderProduct = preorderProduct;
        return ResponseModel.Success(
            ResponseConstants.Get("sản phẩm đặt trước", true),
            ToProductModel(product)
        );
    }

    public async Task<ResponseModel> CreateProductAsync(CreateProductModel model)
    {
        var validateCommonResponse = ValidateCommon(model.SalePrice, model.OriginalPrice, model.Quantity,
            model.StatusId, model.Thumbnail);
        if (validateCommonResponse != null) return validateCommonResponse;

        var validateIdResponse = await ValidateId(model.BrandId, model.CategoryId, model.UnitId);
        if (validateIdResponse != null) return validateIdResponse;

        var existing = await _productRepository.GetByNameAsync(model.Name);
        if (existing != null) return ResponseModel.BadRequest(ResponseConstants.Exist("Tên sản phẩm"));

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
        if (model.StatusId is (int)ProductStatusId.Preorder or (int)ProductStatusId.OutOfStock)
        {
            // set quantity to 0 if status is preorder or out of stock
            product.Quantity = 0;
        }

        //add preorder product if status is preordered
        if (model.StatusId == (int)ProductStatusId.Preorder)
        {
            var preorderProduct = new PreorderProduct
            {
                ProductId = product.Id
            };
            _preorderProductRepository.Add(preorderProduct);
        }

        _productRepository.Add(product);
        var result = await _unitOfWork.SaveChangesAsync();
        if (result > 0)
            return ResponseModel.Success(ResponseConstants.Create("sản phẩm", true), new
            {
                ProductId = product.Id
            });

        return ResponseModel.Error(ResponseConstants.Create("sản phẩm", false));
    }

    /// <summary>
    /// If status is changed, check if product is ordered
    /// If status is preorder, add preorder product if not exists, set quantity to 0, set default max preorder quantity to 1000
    /// </summary>
    /// <param name="id"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    public async Task<ResponseModel> UpdateProductAsync(Guid id, UpdateProductModel model)
    {
        var product = await _productRepository.GetByIdNoIncludeAsync(id);
        if (product == null) return ResponseModel.BadRequest(ResponseConstants.NotFound("Sản phẩm"));

        var existingPreorder = await _preorderProductRepository.GetByIdAsync(id);
        if (model.StatusId == (int)ProductStatusId.Preorder)
        {
            if (existingPreorder == null)
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
                return ResponseModel.BadRequest(ResponseConstants.Exist("Tên sản phẩm"));
            product.Name = model.Name;
        }

        var validateResponse = await ValidateId(model.BrandId, model.CategoryId, model.UnitId);
        if (validateResponse != null) return validateResponse;

        product.BrandId = model.BrandId == 0 ? product.BrandId : model.BrandId;
        product.CategoryId = model.CategoryId == 0 ? product.CategoryId : model.CategoryId;
        product.UnitId = model.UnitId == 0 ? product.UnitId : model.UnitId;

        product.Description = string.IsNullOrEmpty(model.Description)
            ? product.Description
            : model.Description;
        product.Quantity = model.Quantity ?? product.Quantity;
        product.OriginalPrice = model.OriginalPrice ?? product.OriginalPrice;
        product.SalePrice = model.SalePrice ?? product.SalePrice;
        product.IsActive = model.IsActive;
        // check if product is ordered
        if (product.StatusId != (int)ProductStatusId.OutOfStock)
        {
            if (model.StatusId != 0 && product.StatusId != model.StatusId)
            {
                var isOrdered = await _orderDetailRepository.CheckActiveOrderProduct(id);
                if (isOrdered)
                {
                    return ResponseModel.BadRequest(ResponseConstants.ProductOrdered);
                }
            }
        }

        product.StatusId = model.StatusId == 0 ? product.StatusId : model.StatusId;

        // validate common fields and logic if product is active
        if (product.IsActive)
        {
            var validateCommonResponse = ValidateCommon(product.SalePrice, product.OriginalPrice, product.Quantity,
                product.StatusId, model.Thumbnail);
            if (validateCommonResponse != null) return validateCommonResponse;
            if (product.StatusId == (int)ProductStatusId.Preorder)
            {
                var validatePreorderProduct = ValidatePreorderProduct(existingPreorder!, product);
                if (validatePreorderProduct != null) return validatePreorderProduct;
            }
        }

        product.Thumbnail = string.IsNullOrEmpty(model.Thumbnail) ? product.Thumbnail : model.Thumbnail;

        _productRepository.Update(product);
        var result = await _unitOfWork.SaveChangesAsync();
        if (result > 0) return ResponseModel.Success(ResponseConstants.Update("sản phẩm", true), null);

        return ResponseModel.Error(ResponseConstants.Update("sản phẩm", false));
    }

    public async Task<ResponseModel> DeleteProductAsync(Guid id)
    {
        var product = await _productRepository.GetByIdNoIncludeAsync(id);
        if (product == null) return ResponseModel.Success(ResponseConstants.NotFound("Sản phẩm"), null);
        var isOrdered = await _orderDetailRepository.CheckActiveOrderProduct(id);
        if (isOrdered)
        {
            return ResponseModel.BadRequest(ResponseConstants.ProductOrdered);
        }

        var preorderProduct = await _preorderProductRepository.GetByIdAsync(id);
        //delete preorder product if exists
        if (preorderProduct != null) _preorderProductRepository.Delete(preorderProduct);

        _productRepository.Delete(product);
        var result = await _unitOfWork.SaveChangesAsync();
        if (result > 0) return ResponseModel.Success(ResponseConstants.Delete("sản phẩm", true), null);

        return ResponseModel.Error(ResponseConstants.Delete("sản phẩm", false));
    }

    public async Task<ResponseModel> GetProductStatsAsync(ProductStatsQueryModel queryModel)
    {
        var parentId =
            queryModel.ParentId == 0 ? (int?)null : queryModel.ParentId; //null if parent id is 0 (root category)
        var from = queryModel.From ?? DateTime.Now.AddDays(-30);
        var to = queryModel.To ?? DateTime.Now;
        var orderDetails = _orderDetailRepository.GetOrderDetailQuery()
            .Include(od => od.Product)
            .Where(od => od.CreatedAt >= from && od.CreatedAt <= to
                                              && od.Order.StatusId == (int)OrderStatusId.Delivered);
        //get total products sold per brand
        var brands = _brandRepository.GetBrandsQuery();
        var statsPerBrand = await GetBrandStats(brands, orderDetails);
        //get total products sold per category
        var categories = _categoryRepository.GetCategoriesQuery();
        var statsPerCategory = await GetCategoryStats(categories, orderDetails, parentId);
        var bestSeller = await GetBestSellerProductAsync(orderDetails);
        var stats = new ProductStatsModel
        {
            TotalSold = await orderDetails.SumAsync(o => o.Quantity),
            TotalRevenue = await orderDetails.SumAsync(o => o.ItemPrice),
            StatsPerBrand = statsPerBrand,
            StatsPerCategory = statsPerCategory,
            BestSellers = bestSeller
        };
        return ResponseModel.Success(ResponseConstants.Get("thống kê sản phẩm", true), stats);
    }

    private async Task<List<BestSellerModel>> GetBestSellerProductAsync(IQueryable<OrderDetail> orderDetails)
    {
        var query = from order in orderDetails
            group order by new { order.Product.Id, order.Product.Name }
            into g
            select new BestSellerModel
            {
                Id = g.Key.Id,
                Name = g.Key.Name,
                TotalSold = g.Sum(x => x.Quantity),
                TotalRevenue = g.Sum(x => x.ItemPrice)
            };
        var bestSeller = await query.OrderByDescending(x => x.TotalSold).ThenByDescending(x => x.TotalRevenue).Take(5)
            .ToListAsync();
        return bestSeller;
    }

    public async Task<ResponseModel> UpdatePreorderProductAsync(Guid productId, UpdatePreorderProductModel model)
    {
        if (model.StartDate > model.EndDate) return ResponseModel.BadRequest(ResponseConstants.InvalidFilterDate);

        var isExist = await _productRepository.IsExistAsync(productId);
        if (!isExist) return ResponseModel.BadRequest(ResponseConstants.NotFound("Sản phẩm"));

        var preorderProduct = await _preorderProductRepository.GetByIdAsync(productId);
        if (preorderProduct == null) return ResponseModel.BadRequest(ResponseConstants.NotFound("Sản phẩm đặt trước"));

        // update only if there is a value
        preorderProduct.MaxPreOrderQuantity = model.MaxPreOrderQuantity;
        preorderProduct.StartDate = model.StartDate;
        preorderProduct.EndDate = model.EndDate;
        preorderProduct.ExpectedPreOrderDays = model.ExpectedPreOrderDays;
        _preorderProductRepository.Update(preorderProduct);
        var result = await _unitOfWork.SaveChangesAsync();
        if (result > 0) return ResponseModel.Success(ResponseConstants.Update("sản phẩm đặt trước", true), null);

        return ResponseModel.Error(ResponseConstants.Update("sản phẩm đặt trước", false));
    }

    public async Task<ResponseModel> GetSearchResultsAsync(ProductSearchModel queryModel)
    {
        var searchTerm = StringExtension.Normalize(queryModel.SearchTerm);
        var query = _productRepository.GetProductsQuery(true, false)
            .Include(p => p.PreorderProduct).AsQueryable();

        #region Search

        //thu gọn thành 1 where thôi
        query = query.Where(p =>
            (p.IsActive)
            //search theo name, description, brand, unit, category
            && (
                string.IsNullOrEmpty(searchTerm)
                || p.Name.Contains(searchTerm)
                || p.Description!.Contains(searchTerm)
                || p.Brand!.Name.Contains(searchTerm)
                || p.Unit!.Name.Contains(searchTerm)
                || p.Category!.Name.Contains(searchTerm)
            )
            // exclude out of stock products
            && p.StatusId != (int)ProductStatusId.OutOfStock
            //filter by active brand, category, unit
            && (p.Brand!.IsActive && p.Category!.IsActive && p.Unit!.IsActive)
        );

        #endregion

        #region Sort

        if ("desc".Equals(queryModel.SortOrder?.ToLower()))
            query = query.OrderByDescending(GetSortProperty(queryModel.SortColumn));
        else
            query = query.OrderBy(GetSortProperty(queryModel.SortColumn));

        #endregion

        var searchResultModel = query.Select(p => new ProductSearchResultModel
        {
            Id = p.Id,
            Name = p.Name,
            Brand = p.Brand!.Name,
            OriginalPrice = p.OriginalPrice,
            SalePrice = p.SalePrice,
            IsPreOrder = p.PreorderProduct != null,
            AverageRating = p.ProductReviews.IsNullOrEmpty()
                ? 0
                : p.ProductReviews.Average(pr => (double)pr.Rating),
            RatingCount = p.ProductReviews.Count,
            Thumbnail = p.Thumbnail,
            Status = p.ProductStatus!.Name
        });

        #region Pagination

        var searchResults = await PagedList<ProductSearchResultModel>.CreateAsync(
            searchResultModel,
            queryModel.Page,
            queryModel.PageSize
        );

        #endregion

        return ResponseModel.Success(
            ResponseConstants.Get("kết quả tìm kiếm", searchResults.TotalCount > 0),
            searchResults
        );
    }

    private static ProductModel ToProductModel(Product product)
    {
        var orderDetails = product.OrderDetails.IsNullOrEmpty()
            ? []
            : product.OrderDetails.Where(od => od.Order.StatusId == (int)OrderStatusId.Delivered).ToList();
        var model = new ProductModel
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
            AverageRating = product.ProductReviews.IsNullOrEmpty()
                ? 0
                : product.ProductReviews.Average(pr => (double)pr.Rating),
            RatingCount = product.ProductReviews.Count,
            OrderCount = orderDetails.IsNullOrEmpty() ? 0 : orderDetails.Sum(od => od.Quantity),
            IsActive = product.IsActive,
            CreatedAt = product.CreatedAt
        };
        if (product.PreorderProduct == null) return model;
        model.MaxPreOrderQuantity = product.PreorderProduct.MaxPreOrderQuantity;
        model.StartDate = product.PreorderProduct.StartDate;
        model.EndDate = product.PreorderProduct.EndDate;
        model.ExpectedPreOrderDays = product.PreorderProduct.ExpectedPreOrderDays;

        return model;
    }

    /// <summary>
    /// Get sort property by column
    /// </summary>
    /// <param name="sortColumn"></param>
    /// <returns></returns>
    private static Expression<Func<Product, object>> GetSortProperty(
        string? sortColumn
    )
    {
        return sortColumn?.ToLower().Replace(" ", "") switch
        {
            "name" => p => p.Name,
            "saleprice" => p => p.SalePrice == 0 ? p.OriginalPrice : p.SalePrice,
            "quantity" => p => p.Quantity,
            "createdat" => p => p.CreatedAt,
            "rating" => p => p.ProductReviews.Average(pr => (double)pr.Rating),
            "ordercount" => p => p.OrderDetails.Sum(od => od.Quantity),
            _ => product => product.Id
        };
    }

    #region Validation

    /// <summary>
    ///     Validate brand, category, unit id if they are valid
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
            if (brand == null) return ResponseModel.BadRequest(ResponseConstants.NotFound("Thương hiệu"));
        }

        if (categoryId != 0)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (category == null) return ResponseModel.BadRequest(ResponseConstants.NotFound("Danh mục"));
        }

        if (unitId != 0)
        {
            var unit = await _unitRepository.GetByIdAsync(unitId);
            if (unit == null) return ResponseModel.BadRequest(ResponseConstants.NotFound("Đơn vị"));
        }

        return null;
    }

    /// <summary>
    ///     Validate common fields and logic
    /// </summary>
    /// <param name="salePrice"></param>
    /// <param name="originalPrice"></param>
    /// <param name="quantity"></param>
    /// <param name="statusId"></param>
    /// <param name="thumbnail"></param>
    /// <returns></returns>
    private static ResponseModel? ValidateCommon(int salePrice, int originalPrice, int quantity, int statusId,
        string? thumbnail)
    {
        if (!string.IsNullOrEmpty(thumbnail) && !Uri.IsWellFormedUriString(thumbnail, UriKind.Absolute))
            return ResponseModel.BadRequest(ResponseConstants.WrongFormat("URL"));

        if (salePrice > originalPrice) return ResponseModel.BadRequest(ResponseConstants.InvalidSalePrice);
        if (statusId == (int)ProductStatusId.Selling && quantity == 0)
            return ResponseModel.BadRequest(ResponseConstants.InvalidQuantity);
        if (quantity < 0) return ResponseModel.BadRequest(ResponseConstants.InvalidQuantity);
        // if (statusId == (int)ProductStatusId.Preorder && quantity != 0)
        //     return ResponseModel.BadRequest(ResponseConstants.NoQuantityPreorder);
        return null;
    }

    private static ResponseModel? ValidatePreorderProduct(PreorderProduct preorderProduct, Product product)
    {
        if(preorderProduct.StartDate < DateTime.UtcNow)
            return ResponseModel.BadRequest("Ngày bắt đầu không thể nhỏ hơn ngày hiện tại");
        if (preorderProduct.StartDate > preorderProduct.EndDate)
            return ResponseModel.BadRequest(ResponseConstants.InvalidFilterDate);
        if (preorderProduct.MaxPreOrderQuantity <= 0)
            return ResponseModel.BadRequest(ResponseConstants.InvalidMaxPreOrderQuantity);
        if(preorderProduct.ExpectedPreOrderDays <= 0)
            return ResponseModel.BadRequest(ResponseConstants.InvalidExpectedPreOrderDays);
        if (product.Quantity >= preorderProduct.MaxPreOrderQuantity)
            return ResponseModel.BadRequest("Số lượng đặt trước tối đa phải lớn hơn số lượng hiện có");
        return null;
    }

    #endregion

    private async Task<List<CategoryBrandStats>> GetCategoryStats(IQueryable<Category> categories,
        IQueryable<OrderDetail> orderDetails, int? parentId)
    {
        var allCategories = await categories.Include(c => c.Parent).ToListAsync();
        var categoriesList = allCategories.Where(c => c.ParentId == parentId)
            .Select(c => new
            {
                c.Id,
                Name = c.Name + (c.ParentId == null ? "" : $" ({c.Parent!.Name})"),
                childCategoryIds = _categoryRepository.GetChildCategoryIds(c.Id, allCategories)
            }).ToList();
        var allChildCategoryIds = categoriesList.SelectMany(c => c.childCategoryIds).ToList();
        var query = from c in categories
            // join ... into ... from ... in ... DefaultIfEmpty() to perform left join
            // include all categories even if there is no order detail
            join od in orderDetails on c.Id equals od.Product.CategoryId into categoryOrderDetails
            from cod in categoryOrderDetails.DefaultIfEmpty()
            where allChildCategoryIds.Contains(c.Id)
            group new { cod.Quantity, cod.ItemPrice } by c.Id
            into g
            select new
            {
                Id = g.Key,
                TotalSold = g.Sum(x => x.Quantity),
                TotalRevenue = g.Sum(x => x.ItemPrice)
            };
        var dataList = await query.ToListAsync();
        var categoryStatsList = categoriesList.Select(c => new CategoryBrandStats
        {
            Name = c.Name,
            TotalSold = dataList.Where(d => c.childCategoryIds.Contains(d.Id)).Sum(d => d.TotalSold),
            TotalRevenue = dataList.Where(d => c.childCategoryIds.Contains(d.Id)).Sum(d => d.TotalRevenue)
        }).ToList();
        return categoryStatsList;
    }

    private async Task<List<CategoryBrandStats>> GetBrandStats(IQueryable<Brand> brands,
        IQueryable<OrderDetail> orderDetails)
    {
        var query = from b in brands
            // join ... into ... from ... in ... DefaultIfEmpty() to perform left join
            // include all brands even if there is no order detail
            join od in orderDetails on b.Id equals od.Product.BrandId into brandOrderDetails
            from bod in brandOrderDetails.DefaultIfEmpty()
            group new { bod.Quantity, bod.ItemPrice } by b.Name
            into g
            select new CategoryBrandStats
            {
                Name = g.Key,
                TotalSold = g.Sum(x => x.Quantity),
                TotalRevenue = g.Sum(x => x.Quantity * x.ItemPrice)
            };
        List<CategoryBrandStats> brandStatsList = await query.ToListAsync();
        return brandStatsList;
    }
}