using Microsoft.AspNetCore.Mvc;
using SWP391_DEMO.Service;

namespace SWP391_DEMO.Controller {
    public class ProductController : ControllerBase {

        private readonly IProductService _productService;

        public ProductController(IProductService productService) {
            _productService = productService;
        }

        [HttpGet]
        [Route("api/[controller]/get-all-products")]
        public IActionResult GetAllProducts() {
            try {
                return Ok(_productService.GetAllProducts());
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }
    }
}
