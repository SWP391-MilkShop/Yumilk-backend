using System;
using System.Collections.Generic;
namespace SWP391_DEMO.Models
{
    public partial class RefreshToken
    {
        public int Id { get; set; }

        public string? Token { get; set; }

        public DateTime? Expires { get; set; }

        public Guid? UserId { get; set; }

        public bool? IsActive { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        public virtual User? User { get; set; }
    }
}

