using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NET1814_MilkShop.Repositories.Data.Interfaces;

namespace NET1814_MilkShop.Repositories.Data.Entities;

[Table("report_types")]
public class ReportType : IAuditableEntity
{
    [Key] public int Id { get; set; }
    [Column(TypeName = "nvarchar(255)")] public string Name { get; set; } = null!;
    
    [Column(TypeName = "nvarchar(2000)")] public string? Description { get; set; }

    public ICollection<Report> Reports { get; set; } = [];

    [Column("created_at", TypeName = "datetime2")]
    public DateTime CreatedAt { get; set; }

    [Column("modified_at", TypeName = "datetime2")]
    public DateTime? ModifiedAt { get; set; }

    [Column("deleted_at", TypeName = "datetime2")]
    public DateTime? DeletedAt { get; set; }
}