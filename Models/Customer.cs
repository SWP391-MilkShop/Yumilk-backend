using System;
using System.Collections.Generic;

namespace SWP391_DEMO.Models
{
    public partial class Customer
    {
        public Guid UserId { get; set; }
        public int? Points { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }

        public virtual User User { get; set; } = null!;
    }
}
