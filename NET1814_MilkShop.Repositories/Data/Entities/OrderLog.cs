using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NET1814_MilkShop.Repositories.Data.Interfaces;

namespace NET1814_MilkShop.Repositories.Data.Entities;

[Table("order_logs")]
public class OrderLog : IAuditableEntity
{
    [Key] public int Id { get; set; }

    [Column("order_id")]
    [ForeignKey("Order")]
    public Guid OrderId { get; set; }

    [Column("new_status_id")] public int NewStatusId { get; set; }

    [Column("status_name", TypeName = "nvarchar(50)")]
    public string StatusName { get; set; } = null!;

    [Column("created_at", TypeName = "datetime2")]
    public DateTime CreatedAt { get; set; }

    [Column("modified_at", TypeName = "datetime2")]
    public DateTime? ModifiedAt { get; set; }

    [Column("deleted_at", TypeName = "datetime2")]
    public DateTime? DeletedAt { get; set; }

    public virtual Order Order { get; set; } = null!;
}