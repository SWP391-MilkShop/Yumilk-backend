using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWP391_DEMO.Entities;

[Table("ProductAnalytics")]
public partial class ProductAnalytic
{
    [Key]
    public int Id { get; set; }

    public Guid? ProductId { get; set; }

    public int? ViewCount { get; set; }

    public int? PurchaseCount { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual Product? Product { get; set; }
}
