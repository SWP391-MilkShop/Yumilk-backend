using System;
using System.Collections.Generic;

namespace SWP391_DEMO.Models;

public partial class ProductImage
{
    public int Id { get; set; }

    public Guid? ProductId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual Product? Product { get; set; }
}
