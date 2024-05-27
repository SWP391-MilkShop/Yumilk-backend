using NET1814_MilkShop.Repositories.Data.Interfaces;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NET1814_MilkShop.Repositories.Data.Entities;

[Table("Products")]
public partial class Product : IAuditableEntity
{
    [Key]
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int Quantity { get; set; }

    public decimal OriginalPrice { get; set; }

    public decimal SalePrice { get; set; }
    [Column("thumbnail", TypeName = "nvarchar(255)")]
    public string? thumbnail { get; set; }

    public int CategoryId { get; set; }

    [Column("brand_id")]
    public int BrandId { get; set; }

    public int UnitId { get; set; }

    [Column("status_id")]
    [ForeignKey("ProductStatus")]
    public int StatusId { get; set; }

    [DefaultValue(false)]
    public bool IsActive { get; set; }

    [Column("created_at", TypeName = "datetime2")]
    public DateTime CreatedAt { get; set; }

    [Column("modified_at", TypeName = "datetime2")]
    public DateTime? ModifiedAt { get; set; }

    [Column("deleted_at", TypeName = "datetime2")]
    public DateTime? DeletedAt { get; set; }

    public virtual Brand? Brand { get; set; }

    public virtual ICollection<CartDetail> CartDetails { get; set; } = [];

    public virtual Category? Category { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = [];

    public virtual ICollection<ProductAnalytic> ProductAnalytics { get; set; } = [];

    public virtual ICollection<ProductAttributeValue> ProductAttributeValues { get; set; } = [];

    public virtual ICollection<ProductImage> ProductImages { get; set; } = [];

    public virtual ProductStatus? ProductStatus { get; set; }
    public virtual Unit? Unit { get; set; }
}
