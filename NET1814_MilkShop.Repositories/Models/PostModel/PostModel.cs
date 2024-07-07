namespace NET1814_MilkShop.Repositories.Models.PostModel;

public class PostModel
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public Guid AuthorId { get; set; }

    public string AuthorName { get; set; } = "";

    public string? MetaTitle { get; set; }

    public string? MetaDescription { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}