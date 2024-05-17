using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NET1814_MilkShop.Repositories.Data.Entities;

[Table("Orders")]
public partial class Order
{
    [Key]
    public Guid Id { get; set; }

    public Guid? CustomerId { get; set; }

    public decimal TotalPrice { get; set; }

    public decimal ShippingFee { get; set; }

    public decimal TotalAmount { get; set; }

    public int VoucherId { get; set; }

    public string Address { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string? Note { get; set; }
    [Column("payment_method", TypeName = "varchar(255)")]
    public string? PaymentMethod { get; set; }
    [Column("payment_date", TypeName = "datetime2")]
    public DateTime? PaymentDate { get; set; }
    public int StatusId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = [];

    public virtual OrderStatus? Status { get; set; }

    public virtual Voucher? Voucher { get; set; }
}
