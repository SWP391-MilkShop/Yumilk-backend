using SWP391_DEMO.Entities;

namespace SWP391_DEMO.Services {

    public interface IProductService {
        List<Product> GetAllProducts();
    }
    public class ProductService : IProductService {
        private readonly IProductService _productService;

        public ProductService(IProductService productService) {
            _productService = productService;
        }

        public List<Product> GetAllProducts() {
            return _productService.GetAllProducts();
        }
    }
}
