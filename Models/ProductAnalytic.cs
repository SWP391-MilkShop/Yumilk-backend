using System;
using System.Collections.Generic;

namespace SWP391_DEMO.Models
{
    public partial class ProductAnalytic
    {
        public int ProductId { get; set; }
        public int? Views { get; set; }
        public int? Purchases { get; set; }

        public virtual Product Product { get; set; } = null!;
    }
}
