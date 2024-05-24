using Microsoft.AspNetCore.Authorization;
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
        
        [HttpGet("units")]
        [Authorize(AuthenticationSchemes = "Access", Roles = "1,2")]
        public async Task<IActionResult> GetUnits([FromQuery] UnitQueryModel request)
        {
            _logger.Information("Get all units");
            var response = await _productService.GetUnitsAsync(request);
            if (response.Status == "Error")
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        
        [HttpGet("units/{id}")]
        [Authorize(AuthenticationSchemes = "Access", Roles = "1,2")]
        public async Task<IActionResult> GetUnitById(int id)
        {
            _logger.Information("Get unit by id");
            var response = await _productService.GetUnitByIdAsync(id);
            if (response.Status == "Error")
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        
        [HttpPost("units")]
        [Authorize(AuthenticationSchemes = "Access", Roles = "1,2")]
        public async Task<IActionResult> CreateUnitAsync([FromBody] CreateUnitModel model)
        {
            _logger.Information("Create unit");
            var response = await _productService.CreateUnitAsync(model);
            if (response.Status == "Error")
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        
        [HttpPut("units")]
        [Authorize(AuthenticationSchemes = "Access", Roles = "1,2")]
        public async Task<IActionResult> UpdateUnitAsync([FromBody] UnitModel model)
        {
            _logger.Information("Update unit");
            var response = await _productService.UpdateUnitAsync(model);
            if (response.Status == "Error")
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        
        [HttpDelete("units/{id}")]
        [Authorize(AuthenticationSchemes = "Access", Roles = "1,2")]
        public async Task<IActionResult> DeleteUnitAsync(int id)
        {
            _logger.Information("Delete unit");
            var response = await _productService.DeleteUnitAsync(id);
            if (response.Status == "Error")
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
