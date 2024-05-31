using NET1814_MilkShop.Repositories.CoreHelpers.Constants;
using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Repositories;
using NET1814_MilkShop.Repositories.UnitOfWork;

namespace NET1814_MilkShop.Services.Services
{
    public interface IProductImageService
    {
        Task<ResponseModel> GetByProductIdAsync(Guid id);
        Task<ResponseModel> CreateProductImageAsync(Guid id, string imageUrl);
        Task<ResponseModel> UpdateProductImageAsync(int id, string imageUrl);
        Task<ResponseModel> DeleteProductImageAsync(int id);

    }
    public class ProductImageService : IProductImageService
    {
        private readonly IProductImageRepository _productImageRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;
        public ProductImageService(IProductImageRepository productImageRepository, IProductRepository productRepository, IUnitOfWork unitOfWork)
        {
            _productImageRepository = productImageRepository;
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<ResponseModel> CreateProductImageAsync(Guid id, string imageUrl)
        {
            if(!Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
            {
                return ResponseModel.BadRequest(ResponseConstants.WrongFormat("Url"));
            }
            var product = await _productRepository.GetById(id);
            if(product == null)
            {
                return ResponseModel.Success(ResponseConstants.NotFound("Sản phẩm"), null);
            }
            var productImage = new ProductImage
            {
                ProductId = id,
                ImageUrl = imageUrl,
            };
            _productImageRepository.Add(productImage);
            var result = await _unitOfWork.SaveChangesAsync();
            if (result > 0)
            {
                return ResponseModel.Success(ResponseConstants.Create("hình ảnh sản phẩm", true), null);
            }
            return ResponseModel.Error(ResponseConstants.Create("hình ảnh sản phẩm", false));
        }

        public async Task<ResponseModel> DeleteProductImageAsync(int id)
        {
            var productImage = await _productImageRepository.GetById(id);
            if (productImage == null)
            {
                return ResponseModel.Success(ResponseConstants.NotFound("Hình ảnh sản phẩm"), null);
            }
            _productImageRepository.Delete(productImage);
            var result = await _unitOfWork.SaveChangesAsync();
            if (result > 0)
            {
                return ResponseModel.Success(ResponseConstants.Delete("hình ảnh sản phẩm", true), null);
            }
            return ResponseModel.Error(ResponseConstants.Delete("hình ảnh sản phẩm", false));
        }

        public async Task<ResponseModel> GetByProductIdAsync(Guid id)
        {
            var productImages = await _productImageRepository.GetByProductIdAsync(id);
            if (productImages.Any())
            {
                return ResponseModel.Success(ResponseConstants.Get("hình ảnh sản phẩm", true), productImages);
            }
            return ResponseModel.Success(ResponseConstants.NotFound("Hình ảnh sản phẩm"), null);
        }

        public async Task<ResponseModel> UpdateProductImageAsync(int id, string imageUrl)
        {
            var productImage = await _productImageRepository.GetById(id);
            if (productImage == null)
            {
                return ResponseModel.Success(ResponseConstants.NotFound("Hình ảnh sản phẩm"), null);
            }
            if(imageUrl != null)
            {
                if(!Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
                {
                    return ResponseModel.BadRequest(ResponseConstants.WrongFormat("Url"));
                }
                productImage.ImageUrl = imageUrl;
                _productImageRepository.Update(productImage);
                var result = await _unitOfWork.SaveChangesAsync();
                if (result > 0)
                {
                    return ResponseModel.Success(ResponseConstants.Update("hình ảnh sản phẩm", true), null);
                }
                return ResponseModel.Error(ResponseConstants.Update("hình ảnh sản phẩm", false));
            }
            return ResponseModel.BadRequest(ResponseConstants.WrongFormat("Url"));
        }
    }
}
