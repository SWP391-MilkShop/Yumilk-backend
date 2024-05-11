using SWP391_DEMO.Data;
using SWP391_DEMO.Entities;

namespace SWP391_DEMO.Repository {

    public interface IProductRepository {
        List<Product> GetAllProducts();
    }
    public class ProductRepository : IProductRepository {
        public List<Product> GetAllProducts() {
            using (var context = new AppDbContext()) {
                return context.Set<Product>().ToList();
            }
        }
    }
}
