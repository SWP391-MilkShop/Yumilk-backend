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
using NET1814_MilkShop.Services.Services;
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
    [HttpGet("search")]
    public async Task<IActionResult> GetSearchResults([FromQuery] ProductSearchModel queryModel)
    {
        _logger.Information("Get search results");
        var response = await _productService.GetSearchResultsAsync(queryModel);
        return ResponseExtension.Result(response);
    }
    /// <summary>
    ///     Filter products by category, brand, unit, status, min price, max price
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

    [HttpPost]
    [Authorize(AuthenticationSchemes = "Access", Roles = "1,2")]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductModel model)
    {
        _logger.Information("Create product");
        var response = await _productService.CreateProductAsync(model);
        return ResponseExtension.Result(response);
    }
    /// <summary>
    /// Cant change product status if in active order (not DELIVERED OR CANCELLED)
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

    [HttpGet("brands/{id}")]
    public async Task<IActionResult> GetBrandById(int id)
    {
        _logger.Information("Get brand by id");
        var response = await _brandService.GetBrandByIdAsync(id);
        return ResponseExtension.Result(response);
    }

    [HttpGet("brands")]
    public async Task<IActionResult> GetBrands([FromQuery] BrandQueryModel queryModel)
    {
        var response = await _brandService.GetBrandsAsync(queryModel);
        return ResponseExtension.Result(response);
    }

    [HttpPost("brands")]
    [Authorize(AuthenticationSchemes = "Access", Roles = "1,2")]
    public async Task<IActionResult> AddBrand([FromBody] CreateBrandModel model)
    {
        _logger.Information("Add Brand");
        var response = await _brandService.CreateBrandAsync(model);

        return ResponseExtension.Result(response);
    }

    [HttpPatch("brands/{id}")]
    [Authorize(AuthenticationSchemes = "Access", Roles = "1,2")]
    public async Task<IActionResult> UpdateBrand(int id, [FromBody] UpdateBrandModel model)
    {
        _logger.Information("Update Brand");
        var response = await _brandService.UpdateBrandAsync(id, model);
        return ResponseExtension.Result(response);
    }

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
    ///     Get all units search by name and description, sort by name, description (default is id ascending)
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

    [HttpGet("units/{id}")]
    public async Task<IActionResult> GetUnitById(int id)
    {
        _logger.Information("Get unit by id");
        var response = await _unitService.GetUnitByIdAsync(id);
        return ResponseExtension.Result(response);
    }

    [HttpPost("units")]
    [Authorize(AuthenticationSchemes = "Access", Roles = "1,2")]
    public async Task<IActionResult> CreateUnit([FromBody] CreateUnitModel model)
    {
        _logger.Information("Create unit");
        var response = await _unitService.CreateUnitAsync(model);
        return ResponseExtension.Result(response);
    }

    [HttpPatch("units/{id}")]
    [Authorize(AuthenticationSchemes = "Access", Roles = "1,2")]
    public async Task<IActionResult> UpdateUnit(int id, [FromBody] UpdateUnitModel model)
    {
        _logger.Information("Update unit");
        var response = await _unitService.UpdateUnitAsync(id, model);
        return ResponseExtension.Result(response);
    }

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

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories([FromQuery] CategoryQueryModel queryModel)
    {
        _logger.Information("Get all categories");
        var response = await _categoryService.GetCategoriesAsync(queryModel);
        return ResponseExtension.Result(response);
    }

    [HttpGet("categories/{id}")]
    [Authorize(AuthenticationSchemes = "Access", Roles = "1, 2")]
    public async Task<IActionResult> GetCategoryById(int id)
    {
        _logger.Information("Get category by id");
        var response = await _categoryService.GetCategoryByIdAsync(id);
        return ResponseExtension.Result(response);
    }

    [HttpPost("categories")]
    [Authorize(AuthenticationSchemes = "Access", Roles = "1, 2")]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryModel model)
    {
        _logger.Information("Create category");
        var response = await _categoryService.CreateCategoryAsync(model);
        return ResponseExtension.Result(response);
    }

    /// <summary>
    ///     Leave the fields empty if you don't want to update
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

    [HttpGet("attributes")]
    public async Task<IActionResult> GetProductAttributes(
        [FromQuery] ProductAttributeQueryModel queryModel
    )
    {
        _logger.Information("Get Product Attributes");
        var res = await _productAttributeService.GetProductAttributesAsync(queryModel); 
        return ResponseExtension.Result(res);
    }

    [HttpPost("attributes")]
    public async Task<IActionResult> AddProductAttribute(
        [FromBody] CreateProductAttributeModel model
    )
    {
        _logger.Information("Add Product Attribute");
        var res = await _productAttributeService.AddProductAttributeAsync(model);
        return ResponseExtension.Result(res);
    }

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

    [HttpDelete("attributes/{id}")]
    public async Task<IActionResult> DeleteProductAttribute(int id)
    {
        _logger.Information("Delete Product Attribute");
        var res = await _productAttributeService.DeleteProductAttributeAsync(id);
        return ResponseExtension.Result(res);
    }

    #endregion

    #region ProductAttributeValue

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

    [HttpGet("{id}/images")]
    public async Task<IActionResult> GetProductImages(Guid id)
    {
        _logger.Information("Get Product Images");
        var response = await _productImageService.GetByProductIdAsync(id);
        return ResponseExtension.Result(response);
    }

    /// <summary>
    ///     Upload product images (max 10 images per product)
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
    ///     Delete by image id (Hard delete)
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

    [HttpGet("{productId}/reviews")]
    public async Task<IActionResult> GetProductReviews(Guid productId, [FromQuery] ReviewQueryModel queryModel)
    {
        _logger.Information("Get Product Reviews");
        var response = await _productReviewService.GetProductReviewsAsync(productId, queryModel);
        return ResponseExtension.Result(response);
    }

    /// <summary>
    ///     Create review using order id to make sure the customer has bought the product
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

    [HttpPatch("reviews/{id}")]
    [Authorize(AuthenticationSchemes = "Access", Roles = "3")]
    public async Task<IActionResult> UpdateProductReview(int id, [FromBody] UpdateReviewModel model)
    {
        _logger.Information("Update Product Review");
        var response = await _productReviewService.UpdateProductReviewAsync(id, model);
        return ResponseExtension.Result(response);
    }

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