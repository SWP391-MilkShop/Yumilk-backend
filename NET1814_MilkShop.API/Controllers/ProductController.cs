using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Models.BrandModels;
using NET1814_MilkShop.Repositories.Models.CategoryModels;
using NET1814_MilkShop.Repositories.Models.ProductModels;
using NET1814_MilkShop.Repositories.Models.UnitModels;
using NET1814_MilkShop.Services.Services;
using ILogger = Serilog.ILogger;

namespace NET1814_MilkShop.API.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly IUnitService _unitService;
        private readonly ICategoryService _categoryService;
        private readonly ILogger _logger;
        private readonly IBrandService _brandService;

        public ProductController(ILogger logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _productService = serviceProvider.GetRequiredService<IProductService>();
            _brandService = serviceProvider.GetRequiredService<IBrandService>();
            _unitService = serviceProvider.GetRequiredService<IUnitService>();
            _categoryService = serviceProvider.GetRequiredService<ICategoryService>();
        }

        #region Product

        /// <summary>
        /// Filter products by category, brand, unit, status, min price, max price
        /// </summary>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] ProductQueryModel queryModel)
        {
            _logger.Information("Get all products");
            if (queryModel.MinPrice > queryModel.MaxPrice)
            {
                var responseError = new ResponseModel
                {
                    Message = "Min price must be less than max price",
                    Status = "Error"
                };
                return BadRequest(responseError);
            }

            var response = await _productService.GetProductsAsync(queryModel);
            if (response.Status == "Error")
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
        #endregion
        #region Brand

        [HttpGet("brands")]
        public async Task<IActionResult> GetBrands([FromQuery] BrandQueryModel queryModel)
        {
            var response = await _brandService.GetBrandsAsync(queryModel);
            if (response.Status == "Error")
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("brands")]
        public async Task<IActionResult> AddBrand([FromBody] CreateBrandModel model)
        {
            _logger.Information("Add Brand");
            var response = await _brandService.AddBrandAsync(model);
            if (response.Status == "Error")
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPut("brands/{id}")]
        public async Task<IActionResult> UpdateBrand(int id, [FromBody] CreateBrandModel model)
        {
            _logger.Information("Update Brand");
            var response = await _brandService.UpdateBrandAsync(id, model);
            if (response.Status == "Error")
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpDelete("brands/{id}")]
        public async Task<IActionResult> DeleteBrand(int id)
        {
            _logger.Information("Delete Brand");
            var response = await _brandService.DeleteBrandAsync(id);
            if (response.Status == "Error")
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        #endregion

        #region Unit

        /// <summary>
        /// Get all units search by name and description, sort by name, description (default is id ascending)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("units")]
        public async Task<IActionResult> GetUnits([FromQuery] UnitQueryModel request)
        {
            _logger.Information("Get all units");
            var response = await _unitService.GetUnitsAsync(request);
            if (response.Status == "Error")
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("units/{id}")]
        public async Task<IActionResult> GetUnitById(int id)
        {
            _logger.Information("Get unit by id");
            var response = await _unitService.GetUnitByIdAsync(id);
            if (response.Status == "Error")
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("units")]
        [Authorize(AuthenticationSchemes = "Access", Roles = "1,2")]
        public async Task<IActionResult> CreateUnit([FromBody] CreateUnitModel model)
        {
            _logger.Information("Create unit");
            var response = await _unitService.CreateUnitAsync(model);
            if (response.Status == "Error")
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPut("units/{id}")]
        [Authorize(AuthenticationSchemes = "Access", Roles = "1,2")]
        public async Task<IActionResult> UpdateUnit(int id, [FromBody] UpdateUnitModel model)
        {
            _logger.Information("Update unit");
            var response = await _unitService.UpdateUnitAsync(id, model);
            if (response.Status == "Error")
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpDelete("units/{id}")]
        [Authorize(AuthenticationSchemes = "Access", Roles = "1,2")]
        public async Task<IActionResult> DeleteUnit(int id)
        {
            _logger.Information("Delete unit");
            var response = await _unitService.DeleteUnitAsync(id);
            if (response.Status == "Error")
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        #endregion

        #region Category

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories([FromQuery] CategoryQueryModel queryModel)
        {
            _logger.Information("Get all categories");
            var response = await _categoryService.GetCategoriesAsync(queryModel);
            if (response.Status == "Error")
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("categories/{id}")]
        [Authorize(AuthenticationSchemes = "Access", Roles = "1, 2")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            _logger.Information("Get category by id");
            var response = await _categoryService.GetCategoryByIdAsync(id);
            if (response.Status == "Error")
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("categories")]
        [Authorize(AuthenticationSchemes = "Access", Roles = "1, 2")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryModel model)
        {
            _logger.Information("Create category");
            var response = await _categoryService.CreateCategoryAsync(model);
            if (response.Status == "Error")
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Leave the fields empty if you don't want to update
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut("categories/{id}")]
        [Authorize(AuthenticationSchemes = "Access", Roles = "1, 2")]
        public async Task<IActionResult> UpdateCategory(
            int id,
            [FromBody] UpdateCategoryModel model
        )
        {
            _logger.Information("Update category");
            var response = await _categoryService.UpdateCategoryAsync(id, model);
            if (response.Status == "Error")
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpDelete("categories/{id}")]
        [Authorize(AuthenticationSchemes = "Access", Roles = "1, 2")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            _logger.Information("Delete category");
            var response = await _categoryService.DeleteCategoryAsync(id);
            if (response.Status == "Error")
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        #endregion
    }
}
