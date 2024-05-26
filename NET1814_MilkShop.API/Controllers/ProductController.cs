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
        private readonly IBrandService _brandService;

        public ProductController(ILogger logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _productService = serviceProvider.GetRequiredService<IProductService>();
            _brandService = serviceProvider.GetRequiredService<IBrandService>();
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

        [HttpGet]
        [Route("/api/products/brands")]
        public async Task<IActionResult> GetBrands([FromQuery] BrandQueryModel queryModel)
        {
            var response = await _brandService.GetBrandsAsync(queryModel);
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
            var response = await _brandService.AddBrandAsync(model);
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
            var response = await _brandService.UpdateBrandAsync(model);
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
            var response = await _brandService.DeleteBrandAsync(id);
            if (response.Status == "Error")
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}