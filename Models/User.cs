using System;
using System.Collections.Generic;

namespace SWP391_DEMO.Models
{
    public partial class User
    {
        public User()
        {
            Carts = new HashSet<Cart>();
            Orders = new HashSet<Order>();
            UserVouchers = new HashSet<UserVoucher>();
        }

        public Guid Id { get; set; }
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }
        public int RoleId { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? DeletedAt { get; set; }

        public virtual Role Role { get; set; } = null!;
        public virtual Customer? Customer { get; set; }
        public virtual ICollection<Cart> Carts { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<UserVoucher> UserVouchers { get; set; }
    }
}
