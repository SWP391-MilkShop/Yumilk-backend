using SWP391_DEMO.Entities;
using SWP391_DEMO.Repository;

namespace SWP391_DEMO.Service {

    public interface IProductService {
        List<Product> GetAllProducts();
    }
    public class ProductService : IProductService {

        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository) {
            _productRepository = productRepository;
        }

        public List<Product> GetAllProducts() {
            return _productRepository.GetAllProducts();
        }
    }
}
