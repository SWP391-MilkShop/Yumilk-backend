using Microsoft.AspNetCore.Mvc;
using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Models.ProductModels;
using NET1814_MilkShop.Services.Services;
using ILogger = Serilog.ILogger;

namespace NET1814_MilkShop.API.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly ILogger _logger;

        public ProductController(ILogger logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _productService = serviceProvider.GetRequiredService<IProductService>();
            _categoryService = serviceProvider.GetRequiredService<ICategoryService>();
        }

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
        [HttpPost("categories")]
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
        [HttpPut("categories/{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryModel model)
        {
            _logger.Information("Update category");
            model.Id = id;
            var response = await _categoryService.UpdateCategoryAsync(id, model);
            if (response.Status == "Error")
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        [HttpDelete("categories/{id}")]
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

    }
}
