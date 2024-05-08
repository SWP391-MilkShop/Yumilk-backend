using System;
using System.Collections.Generic;

namespace SWP391_DEMO.Models
{
    public partial class UserVoucher
    {
        public Guid UserId { get; set; }
        public int VoucherId { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual User User { get; set; } = null!;
        public virtual Voucher Voucher { get; set; } = null!;
    }
}
