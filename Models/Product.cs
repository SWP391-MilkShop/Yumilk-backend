using System;
using System.Collections.Generic;

namespace SWP391_DEMO.Models
{
    public partial class Product
    {
        public Product()
        {
            CartItems = new HashSet<CartItem>();
            OrderItems = new HashSet<OrderItem>();
            ProductImages = new HashSet<ProductImage>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public int? Quantity { get; set; }
        public decimal? Price { get; set; }
        public int? Discount { get; set; }
        public int? CategoryId { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual Category? Category { get; set; }
        public virtual ProductAnalytic? ProductAnalytic { get; set; }
        public virtual ICollection<CartItem> CartItems { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
        public virtual ICollection<ProductImage> ProductImages { get; set; }
    }
}
