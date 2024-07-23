using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NET1814_MilkShop.API.CoreHelpers.Extensions;
using NET1814_MilkShop.Repositories.Models.BrandModels;
using NET1814_MilkShop.Repositories.Models.CategoryModels;
using NET1814_MilkShop.Repositories.Models.PreorderModels;
using NET1814_MilkShop.Repositories.Models.ProductAttributeModels;
using NET1814_MilkShop.Repositories.Models.ProductAttributeValueModels;
using NET1814_MilkShop.Repositories.Models.ProductModels;
using NET1814_MilkShop.Repositories.Models.ProductReviewModels;
using NET1814_MilkShop.Repositories.Models.UnitModels;
using NET1814_MilkShop.Services.Services.Interfaces;
using ILogger = Serilog.ILogger;

namespace NET1814_MilkShop.API.Controllers;

[ApiController]
[Route("api/products")]
public class ProductController : Controller
{
    private readonly IBrandService _brandService;
    private readonly ICategoryService _categoryService;
    private readonly ILogger _logger;
    private readonly IProductAttributeService _productAttributeService;
    private readonly IProductAttributeValueService _productAttributeValueService;
    private readonly IProductImageService _productImageService;
    private readonly IProductReviewService _productReviewService;
    private readonly IProductService _productService;
    private readonly IUnitService _unitService;

    public ProductController(ILogger logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _productService = serviceProvider.GetRequiredService<IProductService>();
        _brandService = serviceProvider.GetRequiredService<IBrandService>();
        _unitService = serviceProvider.GetRequiredService<IUnitService>();
        _categoryService = serviceProvider.GetRequiredService<ICategoryService>();
        _productAttributeService = serviceProvider.GetRequiredService<IProductAttributeService>();
        _productAttributeValueService = serviceProvider.GetRequiredService<IProductAttributeValueService>();
        _productImageService = serviceProvider.GetRequiredService<IProductImageService>();
        _productReviewService = serviceProvider.GetRequiredService<IProductReviewService>();
    }

    #region Preorder Product

    /// <summary>
    ///     Need to set the product status to PREORDER to update preorder info
    /// Check validate start date and end date, check exist product, check exist preorder product (Table preorder)
    /// Update only if there is a value
    ///     <para> max expected preorder days is 30 days </para>
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPatch("{productId}/preorder")]
    [Authorize(AuthenticationSchemes = "Access", Roles = "1,2")]
    public async Task<IActionResult> UpdatePreorderProduct(Guid productId,
        [FromBody] UpdatePreorderProductModel model)
    {
        _logger.Information("Update Preorder Product");
        var response = await _productService.UpdatePreorderProductAsync(productId, model);
        return ResponseExtension.Result(response);
    }

    #endregion

    #region Product

    /// <summary>
    /// Search product by name, description, brand, unit, category
    /// Sort by name, saleprice, quantity, createdat (default id)
    /// </summary>
    /// <param name="queryModel"></param>
    /// <returns></returns>
    [HttpGet("search")]
    public async Task<IActionResult> GetSearchResults([FromQuery] ProductSearchModel queryModel)
    {
        _logger.Information("Get search results");
        var response = await _productService.GetSearchResultsAsync(queryModel);
        return ResponseExtension.Result(response);
    }

    /// <summary>
    ///     Filter products by category, brand, unit, status, min price, max price
    /// Sort by name, quantity, sale price, created at, rating, order count (default is id asc) (ordercount delivery status)
    ///     <para> Default status is selling</para>
    /// </summary>
    /// <param name="queryModel"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetProducts([FromQuery] ProductQueryModel queryModel)
    {
        _logger.Information("Get all products");
        var response = await _productService.GetProductsAsync(queryModel);
        return ResponseExtension.Result(response);
    }

    /// <summary>
    /// Return additional information if the product is preordered
    /// Check exist product by id, ordercount on delivered status
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById(Guid id)
    {
        _logger.Information("Get product by id");
        var response = await _productService.GetProductByIdAsync(id);
        /*if (response.Status == "Error")
        {
            return BadRequest(response);
        }

        return Ok(response);*/

        return ResponseExtension.Result(response);
    }

    /// <summary>
    /// Add product
    /// Required name (shorter than 255 character) quantity (larger than 0) , originnal and sale price (>0), brand, unit, category, status id in 1-3
    /// Check exist brand, unit, category, check unique name, check validate thumbnail url
    /// set quantity to 0 if status is preorder or out of stock, add preorder product if status is preordered
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize(AuthenticationSchemes = "Access", Roles = "1,2")]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductModel model)
    {
        _logger.Information("Create product");
        var response = await _productService.CreateProductAsync(model);
        return ResponseExtension.Result(response);
    }

    /// <summary>
    /// quantity (>0) , originnal and sale price (>0), check exist brand, unit, category, check unique name, check validate thumbnail url
    /// Cant change product status if in active order (not DELIVERED OR CANCELLED)
    /// If status is preorder, add preorder product if not exists, set quantity to 0, set default max preorder quantity to 1000
    /// If status is preorder and exist in preorder product, validate start date and end date, maxpreorderquantity > 0,
    /// expected delivery date >0, maxpreorderquantity > product quantity (now)
    /// <para>Leave the Ids as 0 for no update</para>
    /// <para>Leave the price as null for no update</para>
    /// </summary>
    /// <param name="id"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPatch("{id}")]
    [Authorize(AuthenticationSchemes = "Access", Roles = "1,2")]
    public async Task<IActionResult> UpdateProduct(Guid id, [FromBody] UpdateProductModel model)
    {
        _logger.Information("Update product");
        var response = await _productService.UpdateProductAsync(id, model);
        return ResponseExtension.Result(response);
    }

    /// <summary>
    /// Delete product by id
    /// Check exist product by id, check exist any order include the product
    /// Check exist preorder (yes) -> delete in preorder table
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    [Authorize(AuthenticationSchemes = "Access", Roles = "1,2")]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        _logger.Information("Delete product");
        var response = await _productService.DeleteProductAsync(id);
        return ResponseExtension.Result(response);
    }

    #endregion

    #region Brand

    /// <summary>
    /// Get brand by id
    /// Check exist brand by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("brands/{id}")]
    public async Task<IActionResult> GetBrandById(int id)
    {
        _logger.Information("Get brand by id");
        var response = await _brandService.GetBrandByIdAsync(id);
        return ResponseExtension.Result(response);
    }

    /// <summary>
    /// Get all brands
    /// Filter by brand status, Search by name or description, Sort by id or name (default is id asc)
    /// </summary>
    /// <param name="queryModel"></param>
    /// <returns></returns>
    [HttpGet("brands")]
    public async Task<IActionResult> GetBrands([FromQuery] BrandQueryModel queryModel)
    {
        var response = await _brandService.GetBrandsAsync(queryModel);
        return ResponseExtension.Result(response);
    }

    /// <summary>
    /// Create new brand
    /// Check exist brand by id, check unique name
    /// Name is require
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("brands")]
    [Authorize(AuthenticationSchemes = "Access", Roles = "1,2")]
    public async Task<IActionResult> AddBrand([FromBody] CreateBrandModel model)
    {
        _logger.Information("Add Brand");
        var response = await _brandService.CreateBrandAsync(model);

        return ResponseExtension.Result(response);
    }

    /// <summary>
    /// Update brand
    /// Check exist brand by id, check unique name
    /// If (any field is null) => not update
    /// </summary>
    /// <param name="id"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPatch("brands/{id}")]
    [Authorize(AuthenticationSchemes = "Access", Roles = "1,2")]
    public async Task<IActionResult> UpdateBrand(int id, [FromBody] UpdateBrandModel model)
    {
        _logger.Information("Update Brand");
        var response = await _brandService.UpdateBrandAsync(id, model);
        return ResponseExtension.Result(response);
    }

    /// <summary>
    /// Delete brand
    /// Check exist brand by id, check exist any product use the brand
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("brands/{id}")]
    [Authorize(AuthenticationSchemes = "Access", Roles = "1,2")]
    public async Task<IActionResult> DeleteBrand(int id)
    {
        _logger.Information("Delete Brand");
        var response = await _brandService.DeleteBrandAsync(id);
        return ResponseExtension.Result(response);
    }

    #endregion

    #region Unit

    /// <summary>
    ///     Get all units search by name and description, unit status, sort by name, description (default is gram ascending)
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet("units")]
    public async Task<IActionResult> GetUnits([FromQuery] UnitQueryModel request)
    {
        _logger.Information("Get all units");
        var response = await _unitService.GetUnitsAsync(request);
        return ResponseExtension.Result(response);
    }

    /// <summary>
    /// Get unit by id
    /// Check exist unit by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("units/{id}")]
    public async Task<IActionResult> GetUnitById(int id)
    {
        _logger.Information("Get unit by id");
        var response = await _unitService.GetUnitByIdAsync(id);
        return ResponseExtension.Result(response);
    }

    /// <summary>
    /// Add new unit
    /// Require name, description, gram
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("units")]
    [Authorize(AuthenticationSchemes = "Access", Roles = "1,2")]
    public async Task<IActionResult> CreateUnit([FromBody] CreateUnitModel model)
    {
        _logger.Information("Create unit");
        var response = await _unitService.CreateUnitAsync(model);
        return ResponseExtension.Result(response);
    }

    /// <summary>
    /// Update unit
    /// Check exist unit by id
    /// Check if any update field is null or empty or gram = 0 => not update
    /// </summary>
    /// <param name="id"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPatch("units/{id}")]
    [Authorize(AuthenticationSchemes = "Access", Roles = "1,2")]
    public async Task<IActionResult> UpdateUnit(int id, [FromBody] UpdateUnitModel model)
    {
        _logger.Information("Update unit");
        var response = await _unitService.UpdateUnitAsync(id, model);
        return ResponseExtension.Result(response);
    }

    /// <summary>
    /// Delete brand
    /// Check exist unit by id, check exist any product use the unit
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("units/{id}")]
    [Authorize(AuthenticationSchemes = "Access", Roles = "1,2")]
    public async Task<IActionResult> DeleteUnit(int id)
    {
        _logger.Information("Delete unit");
        var response = await _unitService.DeleteUnitAsync(id);
        return ResponseExtension.Result(response);
    }

    #endregion

    #region Category

    /// <summary>
    /// Get all categories
    /// Search by category status, parentId,
    /// Sort by name (default is id asc) 
    /// </summary>
    /// <param name="queryModel"></param>
    /// <returns></returns>
    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories([FromQuery] CategoryQueryModel queryModel)
    {
        _logger.Information("Get all categories");
        var response = await _categoryService.GetCategoriesAsync(queryModel);
        return ResponseExtension.Result(response);
    }

    /// <summary>
    /// Get category by id
    /// Check exist category by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("categories/{id}")]
    [Authorize(AuthenticationSchemes = "Access", Roles = "1, 2")]
    public async Task<IActionResult> GetCategoryById(int id)
    {
        _logger.Information("Get category by id");
        var response = await _categoryService.GetCategoryByIdAsync(id);
        return ResponseExtension.Result(response);
    }

    /// <summary>
    /// Add new category
    /// Require name, description (default parentid = 0 (root))
    /// Check exist name and parent id
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("categories")]
    [Authorize(AuthenticationSchemes = "Access", Roles = "1, 2")]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryModel model)
    {
        _logger.Information("Create category");
        var response = await _categoryService.CreateCategoryAsync(model);
        return ResponseExtension.Result(response);
    }

    /// <summary>
    /// Update category
    /// Check exist category by id, check exist parent id, check level of parent id
    /// Leave the fields empty if you don't want to update
    /// </summary>
    /// <param name="id"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPatch("categories/{id}")]
    [Authorize(AuthenticationSchemes = "Access", Roles = "1, 2")]
    public async Task<IActionResult> UpdateCategory(
        int id,
        [FromBody] UpdateCategoryModel model
    )
    {
        _logger.Information("Update category");
        var response = await _categoryService.UpdateCategoryAsync(id, model);
        return ResponseExtension.Result(response);
    }

    /// <summary>
    /// Delete category
    /// Check exist category by id, check exist any product use the category, check any subcategory belongs to the category
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("categories/{id}")]
    [Authorize(AuthenticationSchemes = "Access", Roles = "1, 2")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        _logger.Information("Delete category");
        var response = await _categoryService.DeleteCategoryAsync(id);
        return ResponseExtension.Result(response);
    }

    #endregion

    #region ProductAttribute

    /// <summary>
    /// Get all product attribute
    /// Search by attribute status, name, description, sort by name (default is id asc)
    /// </summary>
    /// <param name="queryModel"></param>
    /// <returns></returns>
    [HttpGet("attributes")]
    public async Task<IActionResult> GetProductAttributes(
        [FromQuery] ProductAttributeQueryModel queryModel
    )
    {
        _logger.Information("Get Product Attributes");
        var res = await _productAttributeService.GetProductAttributesAsync(queryModel);
        return ResponseExtension.Result(res);
    }

    /// <summary>
    /// Get product attribute by id
    /// Check exist product attribute by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("attributes/{id}")]
    public async Task<IActionResult> GetProductAttributesById(int id)
    {
        _logger.Information("Get Product Attributes By Id");
        var res = await _productAttributeService.GetProductAttributesByIdAsync(id);
        return ResponseExtension.Result(res);
    }

    /// <summary>
    /// Add new product attribute
    /// Require name, check uniqure name
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("attributes")]
    public async Task<IActionResult> AddProductAttribute([FromBody] CreateProductAttributeModel model)
    {
        _logger.Information("Add Product Attribute");
        var res = await _productAttributeService.AddProductAttributeAsync(model);
        return ResponseExtension.Result(res);
    }

    /// <summary>
    /// Update product attribute
    /// Check exist product attribute by id
    /// Require name, check unique name
    /// </summary>
    /// <param name="id"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPatch("attributes/{id}")]
    public async Task<IActionResult> UpdateProductAttribute(
        int id,
        [FromBody] UpdateProductAttributeModel model
    )
    {
        _logger.Information("Update Product Attribute");
        var res = await _productAttributeService.UpdateProductAttributeAsync(id, model);
        return ResponseExtension.Result(res);
    }

    /// <summary>
    /// Delete product attribute
    /// Check exist product attribute by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("attributes/{id}")]
    public async Task<IActionResult> DeleteProductAttribute(int id)
    {
        _logger.Information("Delete Product Attribute");
        var res = await _productAttributeService.DeleteProductAttributeAsync(id);
        return ResponseExtension.Result(res);
    }

    #endregion

    #region ProductAttributeValue

    /// <summary>
    /// Get product attribute value
    /// Search by product id and value
    /// sort by productid, attributeid (default: sort by value asc)
    /// </summary>
    /// <param name="id"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpGet("{id}/attributes/values")]
    public async Task<IActionResult> GetProductAttributeValue(
        Guid id,
        [FromQuery] ProductAttributeValueQueryModel model
    )
    {
        _logger.Information("Get Product Attribute Value");
        var res = await _productAttributeValueService.GetProductAttributeValue(id, model);
        return ResponseExtension.Result(res);
    }

    /// <summary>
    /// Add new Product Attribute Value
    /// Check exist product by id, attribute by id, check unique value
    /// </summary>
    /// <param name="id"></param>
    /// <param name="attributeId"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("{id}/attributes/{attributeId}/values")]
    public async Task<IActionResult> AddProAttValues(
        Guid id,
        int attributeId,
        [FromBody] CreateUpdatePavModel model
    )
    {
        _logger.Information("Add Product Attribute Value");
        var res = await _productAttributeValueService.AddProductAttributeValue(
            id,
            attributeId,
            model
        );

        return ResponseExtension.Result(res);
    }


    /// <summary>
    /// Update product attribute value
    /// Check exist product by id, attribute by id, check unique value
    /// </summary>
    /// <param name="id"></param>
    /// <param name="attributeId"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPatch("{id}/attributes/{attributeId}/values")]
    public async Task<IActionResult> UpdateProAttValues(
        Guid id,
        int attributeId,
        [FromBody] CreateUpdatePavModel model
    )
    {
        _logger.Information("Update Product Attribute Value");
        var res = await _productAttributeValueService.UpdateProductAttributeValue(
            id,
            attributeId,
            model
        );

        return ResponseExtension.Result(res);
    }

    /// <summary>
    /// Delete product attribute value
    /// Check exist product by id, attribute by id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="attributeId"></param>
    /// <returns></returns>
    [HttpDelete("{id}/attributes/{attributeId}/values")]
    public async Task<IActionResult> DeleteProAttValues(Guid id, int attributeId)
    {
        _logger.Information("Delete Product Attribute Value");
        var res = await _productAttributeValueService.DeleteProductAttributeValue(
            id,
            attributeId
        );

        return ResponseExtension.Result(res);
    }

    #endregion

    #region ProductImage

    /// <summary>
    /// Get product images
    /// Check exist product's image
    /// </summary>
    /// <param name="id"></param>
    /// <param name="isActive"></param>
    /// <returns></returns>
    [HttpGet("{id}/images")]
    public async Task<IActionResult> GetProductImages(Guid id, [FromQuery] bool? isActive)
    {
        _logger.Information("Get Product Images");
        var response = await _productImageService.GetByProductIdAsync(id, isActive);
        return ResponseExtension.Result(response);
    }

    /// <summary>
    /// Upload product images (max 10 images per product)
    /// Check exist product by id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="images"></param>
    /// <returns></returns>
    [HttpPost("{id}/images")]
    [Authorize(AuthenticationSchemes = "Access", Roles = "1,2")]
    public async Task<IActionResult> CreateProductImage(
        Guid id,
        [FromForm] List<IFormFile> images
    )
    {
        _logger.Information("Upload Product Image");
        var response = await _productImageService.CreateProductImageAsync(id, images);
        return ResponseExtension.Result(response);
    }

    /// <summary>
    /// Update product image
    /// Check exist product image by id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="isActive"></param>
    /// <returns></returns>
    [HttpPatch("images/{id}")]
    [Authorize(AuthenticationSchemes = "Access", Roles = "1,2")]
    public async Task<IActionResult> UpdateProductImage(int id, [FromBody] bool isActive)
    {
        _logger.Information("Update Product Image");
        var response = await _productImageService.UpdateProductImageAsync(id, isActive);
        return ResponseExtension.Result(response);
    }

    /// <summary>
    /// Delete by image id (Hard delete)
    /// Check exist product image by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("images/{id}")]
    [Authorize(AuthenticationSchemes = "Access", Roles = "1,2")]
    public async Task<IActionResult> DeleteProductImage(int id)
    {
        _logger.Information("Delete Product Image");
        var response = await _productImageService.DeleteProductImageAsync(id);
        return ResponseExtension.Result(response);
    }

    #endregion

    #region ProductReview

    /// <summary>
    /// Get all reviews
    /// Filter by rating, isActive, product id, orderid, status,
    /// sort by rating (default is createdat)
    /// </summary>
    /// <param name="queryModel"></param>
    /// <returns></returns>
    [HttpGet("reviews")]
    public async Task<IActionResult> GetProductReviews([FromQuery] ReviewQueryModel queryModel)
    {
        _logger.Information("Get Product Reviews");
        var response = await _productReviewService.GetProductReviewsAsync(queryModel);
        return ResponseExtension.Result(response);
    }

    /// <summary>
    /// Get product' reviews by product id
    /// Check exist product by id, Rating must be between 0 and 5
    /// Filter by product id, rating, status, sort by rating (default is createdat)
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="queryModel"></param>
    /// <returns></returns>
    [HttpGet("{productId}/reviews")]
    public async Task<IActionResult> GetProductReviews(Guid productId, [FromQuery] ProductReviewQueryModel queryModel)
    {
        _logger.Information("Get Product Reviews by Product Id {productId}", productId);
        var response = await _productReviewService.GetProductReviewsByProductIdAsync(productId, queryModel);
        return ResponseExtension.Result(response);
    }

    /// <summary>
    ///     Create review using order id to make sure the customer has bought the product
    /// Check exist product by id, check exist order by id, check exist product in delivered order
    /// Check exist any review before
    /// Required rating, review, order id
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("{productId}/reviews")]
    [Authorize(AuthenticationSchemes = "Access", Roles = "3")]
    public async Task<IActionResult> CreateProductReview(Guid productId, [FromBody] CreateReviewModel model)
    {
        _logger.Information("Create Product Review");
        var response = await _productReviewService.CreateProductReviewAsync(productId, model);
        return ResponseExtension.Result(response);
    }

    /// <summary>
    /// Update product review
    /// check exist review by id
    /// If any field is null or empty or rating = 0 => not update
    /// </summary>
    /// <param name="id"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPatch("reviews/{id}")]
    [Authorize(AuthenticationSchemes = "Access", Roles = "3")]
    public async Task<IActionResult> UpdateProductReview(int id, [FromBody] UpdateReviewModel model)
    {
        _logger.Information("Update Product Review");
        var response = await _productReviewService.UpdateProductReviewAsync(id, model);
        return ResponseExtension.Result(response);
    }

    /// <summary>
    /// Delete review
    /// Check exist review by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("reviews/{id}")]
    [Authorize(AuthenticationSchemes = "Access", Roles = "1,2,3")]
    public async Task<IActionResult> DeleteProductReview(int id)
    {
        _logger.Information("Delete Product Review");
        var response = await _productReviewService.DeleteProductReviewAsync(id);
        return ResponseExtension.Result(response);
    }

    #endregion
}