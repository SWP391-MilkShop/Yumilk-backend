using System;
using System.Collections.Generic;

namespace SWP391_DEMO.Entities;

public partial class Cart
{
    public int Id { get; set; }

    public Guid? CustomerId { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<CartDetail> CartDetails { get; set; } = new List<CartDetail>();

    public virtual Customer? Customer { get; set; }
}
