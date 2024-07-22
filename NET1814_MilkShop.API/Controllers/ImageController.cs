using Microsoft.AspNetCore.Mvc;
using NET1814_MilkShop.API.CoreHelpers.Extensions;
using NET1814_MilkShop.Repositories.Models.ImageModels;
using NET1814_MilkShop.Services.Services.Interfaces;
using ILogger = Serilog.ILogger;

namespace NET1814_MilkShop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImageController : Controller
{
    private readonly IImageService _imageService;
    private readonly ILogger _logger;

    public ImageController(
        IServiceProvider serviceProvider,
        ILogger logger
    )
    {
        _imageService = serviceProvider.GetRequiredService<IImageService>();
        _logger = logger;
    }

    /// <summary>
    /// Get image by image hash (id)
    /// Add ClientId of Imgure in header
    /// 200 (Thành công) 400 (Yêu cầu không hợp lệ) 404 (Không tìm thấy) 500 (Lỗi server)
    /// </summary>
    /// <param name="imageHash"></param>
    /// <returns></returns>
    [HttpGet("{imageHash}")]
    public async Task<IActionResult> GetImage(string imageHash)
    {
        _logger.Information("Get image by hash: {imageHash}", imageHash);
        var response = await _imageService.GetImageAsync(imageHash);
        return ResponseExtension.Result(response);
    }

    /// <summary>
    /// Upload image to Imgur
    /// Add ClientId of Imgure in header
    /// Require an image (IFormFile)
    /// 200 (Thành công) 400 (Yêu cầu không hợp lệ) 404 (Không tìm thấy) 500 (Lỗi server)
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> UploadImage([FromForm] ImageUploadModel model)
    {
        _logger.Information("Upload image");
        var response = await _imageService.UploadImageAsync(model);
        return ResponseExtension.Result(response);
    }
}