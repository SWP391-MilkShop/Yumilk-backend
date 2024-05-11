using SWP391_DEMO.Entities;
using SWP391_DEMO.Models;
using SWP391_DEMO.Repository;

namespace SWP391_DEMO.Services {

    public interface IProductService {
        List<ProductModel> GetAllProducts();
        ProductModel? GetProductById(Guid productId);
    }
    public class ProductService : IProductService {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productService) {
            _productRepository = productService;
        }

        public List<ProductModel> GetAllProducts() {
            var entity = _productRepository.GetAllProducts();
            var list = new List<ProductModel>();
            entity.ForEach(p => {
                var a = new ProductModel {
                    Name = p.Name,
                    Description = p.Description,
                    Quantity = p.Quantity,
                    OriginalPrice = p.OriginalPrice,
                    SalePrice = p.SalePrice,
                };
                list.Add(a);
            });
            return list;
        }

        public ProductModel? GetProductById(Guid productId) {
            var entity = _productRepository.GetProductById(productId);
            if (entity == null) {
                return null;
            }
            var p = new ProductModel() {
                Name = entity.Name,
                Description = entity.Description,
                Quantity = entity.Quantity,
                OriginalPrice = entity.OriginalPrice,
                SalePrice = entity.SalePrice,
            };
            return p;
        }
    }
}
