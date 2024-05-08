using System;
using System.Collections.Generic;

namespace SWP391_DEMO.Models
{
    public partial class Cart
    {
        public Cart()
        {
            CartItems = new HashSet<CartItem>();
        }

        public int Id { get; set; }
        public Guid? UserId { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual User? User { get; set; }
        public virtual ICollection<CartItem> CartItems { get; set; }
    }
}
