using SWP391_DEMO.Entities;
using SWP391_DEMO.Repository;

namespace SWP391_DEMO.Services {

    public interface IProductService {
        List<Product> GetAllProducts();
    }
    public class ProductService : IProductService {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productService) {
            _productRepository = productService;
        }

        public List<Product> GetAllProducts() {
            return _productRepository.GetAllProducts();
        }
    }
}
