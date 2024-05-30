using Microsoft.AspNetCore.Mvc;
using NET1814_MilkShop.API.CoreHelpers.Extensions;
using NET1814_MilkShop.Repositories.Models.ImageModels;
using NET1814_MilkShop.Services.Services;
using Newtonsoft.Json;
using ILogger = Serilog.ILogger;
namespace NET1814_MilkShop.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageController : Controller
    {
        private readonly IImageService _imageService;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        public ImageController(IServiceProvider serviceProvider, ILogger logger, IConfiguration configuration)
        {
            _imageService = serviceProvider.GetRequiredService<IImageService>();
            _logger = logger;
            _configuration = configuration;
        }
        [HttpGet("{imageHash}")]
        public async Task<IActionResult> GetImage(string imageHash)
        {
            var response = await _imageService.GetImageAsync(imageHash);
            return ResponseExtension.Result(response);
        }
        [HttpPost]
        public async Task<IActionResult> UploadImage([FromForm] ImageUploadModel model)
        {
            var response = await _imageService.UploadImageAsync(model);
            return ResponseExtension.Result(response);
        }
    }
}
