using Microsoft.EntityFrameworkCore;
using NET1814_MilkShop.Repositories.Data;
using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Models.ProductModels;

namespace NET1814_MilkShop.Repositories.Repositories
{
    public interface IProductImageRepository
    {
        /// <summary>
        /// Return list of product images by product id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<List<ProductImageModel>> GetByProductIdAsync(Guid id);
        Task<ProductImage?> GetById(int id);
        void Add(ProductImage productImage);
        void Update(ProductImage productImage);
        void Delete(ProductImage productImage);

    }
    public class ProductImageRepository : Repository<ProductImage>, IProductImageRepository
    {
        public ProductImageRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<List<ProductImageModel>> GetByProductIdAsync(Guid id)
        {
            return await _query.Where(x => x.ProductId == id).Select(x => new ProductImageModel
            {
                Id = x.Id,
                ProductId = id,
                ImageUrl = x.ImageUrl,
            }).ToListAsync();
        }
    }
}
