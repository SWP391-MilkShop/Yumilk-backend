using System;
using System.Collections.Generic;

namespace SWP391_DEMO.Models
{
    public partial class OrderItem
    {
        public Guid OrderId { get; set; }
        public int ProductId { get; set; }
        public int? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public string? ProductName { get; set; }
        public decimal? ItemPrice { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual Order Order { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
    }
}
