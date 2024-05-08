using System;
using System.Collections.Generic;

namespace SWP391_DEMO.Models
{
    public partial class Voucher
    {
        public Voucher()
        {
            Orders = new HashSet<Order>();
            UserVouchers = new HashSet<UserVoucher>();
        }

        public int Id { get; set; }
        public string? Code { get; set; }
        public int? Quantity { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? MinimumOrderAmount { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<UserVoucher> UserVouchers { get; set; }
    }
}
