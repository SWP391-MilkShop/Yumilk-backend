using System.ComponentModel.DataAnnotations;

namespace NET1814_MilkShop.Repositories.Models.PostModel;

public class CreatePostModel
{
    [Required(ErrorMessage = "Title is required")]
    [MinLength(5, ErrorMessage = "Title must be at least 5 characters")]
    public string Title { get; set; } = null!;

    [Required(ErrorMessage = "Content is required")]
    [MinLength(10, ErrorMessage = "Content must be at least 10 characters")]
    public string Content { get; set; } = null!;

    public string? MetaTitle { get; set; }

    public string? MetaDescription { get; set; }
}