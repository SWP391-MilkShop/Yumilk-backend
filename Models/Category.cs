using System;
using System.Collections.Generic;

namespace SWP391_DEMO.Models
{
    public partial class Category
    {
        public Category()
        {
            Products = new HashSet<Product>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public int? Index { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
