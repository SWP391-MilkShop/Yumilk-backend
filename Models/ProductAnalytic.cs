using System;
using System.Collections.Generic;

namespace SWP391_DEMO.Models;

public partial class ProductAnalytic
{
    public int Id { get; set; }

    public Guid? ProductId { get; set; }

    public int? ViewCount { get; set; }

    public int? PurchaseCount { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual Product? Product { get; set; }
}
