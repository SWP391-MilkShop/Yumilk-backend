using Microsoft.AspNetCore.Mvc;
using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Models.BrandModels;
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
        private readonly ILogger _logger;

        public ProductController(ILogger logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _productService = serviceProvider.GetRequiredService<IProductService>();
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

        /// <summary>
        /// filter by description (default: filter by name)
        /// </summary>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("/api/products/brands")]
        public async Task<IActionResult> GetBrands([FromQuery] QueryModel queryModel)
        {
            var response = await _productService.GetBrandsAsync(queryModel);
            if (response.Status == "Error")
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost]
        [Route("/api/products/brands")]
        public async Task<IActionResult> AddBrand([FromBody] BrandModel model)
        {
            _logger.Information("Add Brand");
            var response = await _productService.AddBrandAsync(model);
            if (response.Status == "Error")
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPut]
        [Route("/api/products/brands")]
        public async Task<IActionResult> UpdateBrand([FromBody] BrandModel model)
        {
            _logger.Information("Update Brand");
            var response = await _productService.UpdateBrandAsync(model);
            if (response.Status == "Error")
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpDelete]
        [Route("/api/products/brands")]
        public async Task<IActionResult> DeleteBrand([FromQuery] int id)
        {
            _logger.Information("Delete Brand");
            var response = await _productService.DeleteBrandAsync(id);
            if (response.Status == "Error")
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}