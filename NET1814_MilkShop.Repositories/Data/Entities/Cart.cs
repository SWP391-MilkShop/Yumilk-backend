using NET1814_MilkShop.Repositories.Data.Interfaces;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NET1814_MilkShop.Repositories.Data.Entities;

[Table("Carts")]
public partial class Cart : IAuditableEntity
{
    [Key]
    public int Id { get; set; }

    public Guid? CustomerId { get; set; }

    [DefaultValue(false)]
    public bool IsActive { get; set; }

    [Column("created_at", TypeName = "datetime2")]
    public DateTime CreatedAt { get; set; }

    [Column("modified_at", TypeName = "datetime2")]
    public DateTime? ModifiedAt { get; set; }

    [Column("deleted_at", TypeName = "datetime2")]
    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<CartDetail> CartDetails { get; set; } = [];

    public virtual Customer? Customer { get; set; }
}
