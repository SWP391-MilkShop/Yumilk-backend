using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NET1814_MilkShop.Repositories.Data.Interfaces;

namespace NET1814_MilkShop.Repositories.Data.Entities;

[Table("posts")]
public class Post : IAuditableEntity
{
    [Key] public Guid Id { get; set; }

    [Column("title", TypeName = "nvarchar(255)")]
    public string Title { get; set; } = null!;

    [Column("content", TypeName = "nvarchar(max)")]
    public string Content { get; set; } = null!;

    [Column("author_id")]
    [ForeignKey("Author")]
    public Guid AuthorId { get; set; }

    [Column("meta_title", TypeName = "nvarchar(255)")]
    public string MetaTitle { get; set; } = "";

    [Column("meta_description", TypeName = "nvarchar(max)")]
    public string MetaDescription { get; set; } = "";
    
    [Column("thumbnail", TypeName = "nvarchar(255)")]
    public string? Thumbnail { get; set; }

    [Column("is_active")] public bool IsActive { get; set; }

    public virtual User? Author { get; set; }

    [Column("created_at", TypeName = "datetime2")]
    public DateTime CreatedAt { get; set; }

    [Column("modified_at", TypeName = "datetime2")]
    public DateTime? ModifiedAt { get; set; }

    [Column("deleted_at", TypeName = "datetime2")]
    public DateTime? DeletedAt { get; set; }
}