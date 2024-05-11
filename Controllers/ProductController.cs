using Microsoft.AspNetCore.Mvc;
using SWP391_DEMO.Services;
using SWP391_DEMO.CoreHelper;
using Serilog;
namespace SWP391_DEMO.Controllers {
    public class ProductController : ControllerBase {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;

        public ProductController(IProductService productService, ILogger<ProductController> logger) {
            _productService = productService;
            _logger = logger;
        }

        [HttpGet]
        [Route("/api/[controller]/get-all-products")]
        public IActionResult GetAllProduct() {
            try {
                _logger.LogInformation("Get all products: ");
                return Ok(_productService.GetAllProducts());
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("/api/[controller]/get-product-by-id")]
        public IActionResult GetProductById(Guid productId) {
            try {
                _logger.LogInformation("Get product by id: ");
                var exist = _productService.GetProductById(productId);
                if (exist == null) {
                    return StatusCode(404, ResponseConstant.ProductNotFound);
                }
                return Ok(exist);
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }
    }
}
