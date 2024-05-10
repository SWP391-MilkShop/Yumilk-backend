using System;
using System.Collections.Generic;

namespace SWP391_DEMO.Models;

public partial class Order
{
    public Guid Id { get; set; }

    public Guid? CustomerId { get; set; }

    public decimal? TotalPrice { get; set; }

    public decimal? ShippingFee { get; set; }

    public decimal? TotalAmount { get; set; }

    public int? VoucherId { get; set; }

    public string Address { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string? Note { get; set; }

    public int? StatusId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual OrderStatus? Status { get; set; }

    public virtual Voucher? Voucher { get; set; }
}
