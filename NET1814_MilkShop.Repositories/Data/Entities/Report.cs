using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NET1814_MilkShop.Repositories.Data.Interfaces;

namespace NET1814_MilkShop.Repositories.Data.Entities;

[Table("reports")]
public class Report : IAuditableEntity
{
    [Key] public Guid Id { get; set; }

    [Column("report_type_id")]
    [ForeignKey("ReportType")]
    public int ReportTypeId { get; set; }

    [Column("customer_id")]
    [ForeignKey("Customer")]
    public Guid CustomerId { get; set; }

    [Column("title", TypeName = "nvarchar(255)")]
    public string Title { get; set; } = null!;

    [Column("description", TypeName = "nvarchar(2000)")]
    public string? Description { get; set; }

    [Column("resolved_at", TypeName = "datetime2")]
    public DateTime? ResolvedAt { get; set; }

    [Column("resolved_by")] public Guid ResolvedBy { get; set; }

    public virtual ReportType ReportType { get; set; } = null!;

    public virtual Customer Customer { get; set; } = null!;

    [Column("created_at", TypeName = "datetime2")]
    public DateTime CreatedAt { get; set; }

    [Column("modified_at", TypeName = "datetime2")]
    public DateTime? ModifiedAt { get; set; }

    [Column("deleted_at", TypeName = "datetime2")]
    public DateTime? DeletedAt { get; set; }
}