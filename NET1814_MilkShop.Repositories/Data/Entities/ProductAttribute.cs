using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NET1814_MilkShop.Repositories.Data.Interfaces;

namespace NET1814_MilkShop.Repositories.Data.Entities;

[Table("ProductAttributes")]
public class ProductAttribute : IAuditableEntity
{
    [Key] public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    [DefaultValue(false)] public bool IsActive { get; set; }

    public virtual ICollection<ProductAttributeValue> ProductAttributeValues { get; set; } = [];

    [Column("created_at", TypeName = "datetime2")]
    public DateTime CreatedAt { get; set; }

    [Column("modified_at", TypeName = "datetime2")]
    public DateTime? ModifiedAt { get; set; }

    [Column("deleted_at", TypeName = "datetime2")]
    public DateTime? DeletedAt { get; set; }
}