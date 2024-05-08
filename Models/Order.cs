using System;
using System.Collections.Generic;

namespace SWP391_DEMO.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderItems = new HashSet<OrderItem>();
        }

        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public decimal? TotalPrice { get; set; }
        public decimal? ShippingFee { get; set; }
        public decimal? TotalAmount { get; set; }
        public int? VoucherId { get; set; }
        public string Address { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }
        public int? Status { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual User? User { get; set; }
        public virtual Voucher? Voucher { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}
