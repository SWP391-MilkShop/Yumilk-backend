using Microsoft.AspNetCore.Http;
using NET1814_MilkShop.Repositories.CoreHelpers.Constants;
using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Models.ImageModels;
using NET1814_MilkShop.Repositories.Repositories.Interfaces;
using NET1814_MilkShop.Repositories.UnitOfWork.Interfaces;
using NET1814_MilkShop.Services.Services.Interfaces;

namespace NET1814_MilkShop.Services.Services.Implementations;

public class ProductImageService : IProductImageService
{
    private readonly IProductImageRepository _productImageRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IImageService _imageService;

    public ProductImageService(
        IProductImageRepository productImageRepository,
        IProductRepository productRepository,
        IUnitOfWork unitOfWork,
        IImageService imageService
    )
    {
        _productImageRepository = productImageRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _imageService = imageService;
    }

    public async Task<ResponseModel> CreateProductImageAsync(Guid id, List<IFormFile> images)
    {
        var isExist = await _productRepository.IsExistAsync(id);
        if (!isExist)
        {
            return ResponseModel.BadRequest(ResponseConstants.NotFound("Sản phẩm"));
        }

        var productImages = await _productImageRepository.GetByProductIdAsync(id, null);
        if (productImages.Count + images.Count > 10)
        {
            return ResponseModel.Success(
                ResponseConstants.OverLimit("hình ảnh sản phẩm"),
                null
            );
        }

        //Upload images to imgur asynchronously
        var uploadTasks = images.Select(async image =>
        {
            var response = await _imageService.UploadImageAsync(
                new ImageUploadModel { Image = image }
            );
            if (response.StatusCode == 200)
            {
                var imgurData = response.Data as ImgurData;
                if (imgurData != null)
                {
                    var productImage = new ProductImage
                    {
                        ProductId = id,
                        ImageUrl = imgurData.Link
                    };
                    _productImageRepository.Add(productImage);
                    return ResponseConstants.Upload(image.FileName, true);
                }
            }

            return ResponseConstants.Upload(image.FileName, false);
        });
        //Wait for all upload tasks to complete
        var uploadResults = await Task.WhenAll(uploadTasks);
        var result = await _unitOfWork.SaveChangesAsync();
        if (result > 0)
        {
            return ResponseModel.Success(
                ResponseConstants.Create("hình ảnh sản phẩm", true),
                uploadResults
            );
        }

        return ResponseModel.Error(ResponseConstants.Create("hình ảnh sản phẩm", false));
    }

    public async Task<ResponseModel> DeleteProductImageAsync(int id)
    {
        var productImage = await _productImageRepository.GetByIdAsync(id);
        if (productImage == null)
        {
            return ResponseModel.Success(ResponseConstants.NotFound("Hình ảnh sản phẩm"), null);
        }

        //_productImageRepository.Delete(productImage);
        _productImageRepository.Remove(productImage);
        var result = await _unitOfWork.SaveChangesAsync();
        if (result > 0)
        {
            return ResponseModel.Success(
                ResponseConstants.Delete("hình ảnh sản phẩm", true),
                null
            );
        }

        return ResponseModel.Error(ResponseConstants.Delete("hình ảnh sản phẩm", false));
    }

    public async Task<ResponseModel> UpdateProductImageAsync(int id, bool isActive)
    {
        var productImage = await _productImageRepository.GetByIdAsync(id);
        if (productImage == null)
        {
            return ResponseModel.Success(ResponseConstants.NotFound("Hình ảnh sản phẩm"), null);
        }

        productImage.IsActive = isActive;
        _productImageRepository.Update(productImage);
        var result = await _unitOfWork.SaveChangesAsync();
        if (result > 0)
        {
            return ResponseModel.Success(
                ResponseConstants.Update("hình ảnh sản phẩm", true), null);
        }

        return ResponseModel.Error(ResponseConstants.Update("hình ảnh sản phẩm", false));
    }

    public async Task<ResponseModel> GetByProductIdAsync(Guid id, bool? isActive)
    {
        var productImages = await _productImageRepository.GetByProductIdAsync(id, isActive);
        if (productImages.Any())
        {
            return ResponseModel.Success(
                ResponseConstants.Get("hình ảnh sản phẩm", true),
                productImages
            );
        }

        return ResponseModel.Success(ResponseConstants.NotFound("Hình ảnh sản phẩm"), null);
    }
}