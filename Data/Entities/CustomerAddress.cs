using System;
using System.Collections.Generic;

namespace SWP391_DEMO.Entities;

public partial class CustomerAddress
{
    public int Id { get; set; }

    public string? Address { get; set; }

    public string? PhoneNumber { get; set; }

    public Guid? UserId { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual Customer? User { get; set; }
}
