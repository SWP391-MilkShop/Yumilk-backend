using System.ComponentModel;
using NET1814_MilkShop.Repositories.Data.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NET1814_MilkShop.Repositories.Data.Entities;

[Table("product_images")]
public partial class ProductImage : IAuditableEntity
{
    [Key]
    public int Id { get; set; }

    public Guid? ProductId { get; set; }

    public string ImageUrl { get; set; } = null!;

    [DefaultValue(false)]
    public bool IsActive { get; set; }

    [Column("created_at", TypeName = "datetime2")]
    public DateTime CreatedAt { get; set; }

    [Column("modified_at", TypeName = "datetime2")]
    public DateTime? ModifiedAt { get; set; }

    [Column("deleted_at", TypeName = "datetime2")]
    public DateTime? DeletedAt { get; set; }

    public virtual Product? Product { get; set; }
}
