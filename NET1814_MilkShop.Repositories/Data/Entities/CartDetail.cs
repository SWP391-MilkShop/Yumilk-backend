using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using NET1814_MilkShop.Repositories.Data.Interfaces;

namespace NET1814_MilkShop.Repositories.Data.Entities;

[Table("CartDetails")]
public class CartDetail : IAuditableEntity
{
    public int CartId { get; set; }
    public Guid ProductId { get; set; }

    public int Quantity { get; set; }

    [DefaultValue(false)] public bool IsActive { get; set; }

    public virtual Cart Cart { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    [Column("created_at", TypeName = "datetime2")]
    public DateTime CreatedAt { get; set; }

    [Column("modified_at", TypeName = "datetime2")]
    public DateTime? ModifiedAt { get; set; }

    [Column("deleted_at", TypeName = "datetime2")]
    public DateTime? DeletedAt { get; set; }
}