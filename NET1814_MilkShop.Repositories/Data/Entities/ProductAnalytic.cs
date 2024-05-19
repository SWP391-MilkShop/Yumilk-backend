using NET1814_MilkShop.Repositories.Data.Interfaces;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NET1814_MilkShop.Repositories.Data.Entities;

[Table("ProductAnalytics")]
public partial class ProductAnalytic : IAuditableEntity
{
    [Key]
    public int Id { get; set; }

    public Guid? ProductId { get; set; }

    public int ViewCount { get; set; }

    public int PurchaseCount { get; set; }

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
