using System.ComponentModel.DataAnnotations.Schema;

namespace SWP391_DEMO.Entities;

[Table("UserVouchers")]
public partial class UserVoucher
{
    public int VoucherId { get; set; }

    public Guid CustomerId { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual Voucher Voucher { get; set; } = null!;
}
