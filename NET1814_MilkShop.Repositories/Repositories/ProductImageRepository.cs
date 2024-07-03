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
        /// <param name="isActive"></param>
        /// <returns></returns>
        Task<List<ProductImageModel>> GetByProductIdAsync(Guid id, bool? isActive);

        Task<ProductImage?> GetByIdAsync(int id);
        void Add(ProductImage productImage);
        void Update(ProductImage productImage);

        //void Delete(ProductImage productImage);
        void Remove(ProductImage productImage);
    }

    public class ProductImageRepository : Repository<ProductImage>, IProductImageRepository
    {
        public ProductImageRepository(AppDbContext context)
            : base(context)
        {
        }

        public async Task<List<ProductImageModel>> GetByProductIdAsync(Guid id, bool? isActive)
        {
            var query = _query.Where(x => x.ProductId == id && (!isActive.HasValue || x.IsActive == isActive));
            return await query.Select(x => new ProductImageModel
                {
                    Id = x.Id,
                    ProductId = id,
                    ImageUrl = x.ImageUrl,
                    IsActive = x.IsActive
                })
                .ToListAsync();
        }
    }
}