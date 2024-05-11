using Microsoft.AspNetCore.Mvc;
using SWP391_DEMO.Services;

namespace SWP391_DEMO.Controllers {
    public class ProductController : ControllerBase {
        private readonly IProductService _productService;

        public ProductController(IProductService productService) {
            _productService = productService;
        }

        [HttpGet]
        [Route("/api/[controller]/get-all-products")]
        public IActionResult GetAllProduct() {
            try {
                return Ok(_productService.GetAllProducts());
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }
    }
}
