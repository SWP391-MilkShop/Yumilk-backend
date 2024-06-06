using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace NET1814_MilkShop.Repositories.Models.ImageModels
{
    public class ImageUploadModel
    {
        [Required(ErrorMessage = "Image is required")]
        public required IFormFile Image { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
    }
}
