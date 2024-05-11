using Microsoft.EntityFrameworkCore;
using SWP391_DEMO.Data;
using SWP391_DEMO.Entities;

namespace SWP391_DEMO.Repository {
    public interface IProductRepository {
        List<Product> GetAllProducts();
        Product? GetProductById(Guid productId);
    }
    public class ProductRepository : IProductRepository {
        public List<Product> GetAllProducts() {
            using (var context = new AppDbContext()) {
                return context.Set<Product>().AsNoTracking().ToList();
            }
        }

        public Product? GetProductById(Guid productId) {
            using (var context = new AppDbContext()) {
                return context.Set<Product>().AsNoTracking().FirstOrDefault(x => x.Id == productId);
            }
        }
    }
}
