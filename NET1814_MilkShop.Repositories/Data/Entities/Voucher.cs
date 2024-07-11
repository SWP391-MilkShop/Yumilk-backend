using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NET1814_MilkShop.Repositories.Data.Interfaces;

namespace NET1814_MilkShop.Repositories.Data.Entities;

[Table("vouchers")]
public class Voucher : IAuditableEntity
{
    [Key] public Guid Id { get; set; }

    // Mã voucher tối đa 10 ký tự
    [Column(TypeName = "nvarchar(10)")] public string Code { get; set; } = null!;

    [Column(TypeName = "nvarchar(2000)")] public string Description { get; set; } = null!;

    [Column("start_date", TypeName = "datetime2")]
    public DateTime StartDate { get; set; }

    [Column("end_date", TypeName = "datetime2")]
    public DateTime EndDate { get; set; }

    // Số lượng voucher còn lại
    public int Quantity { get; set; }
    
    // Phần trăm giảm giá
    public int Percent { get; set; }
    
    [Column("is_active")]
    public bool IsActive { get; set; }
    
    // Giá trị giảm giá tối đa, nếu bằng 0 thì không giới hạn
    [Column("max_discount")]
    public int MaxDiscount { get; set; }

    [Column("created_at", TypeName = "datetime2")]
    public DateTime CreatedAt { get; set; }

    [Column("modified_at", TypeName = "datetime2")]
    public DateTime? ModifiedAt { get; set; }

    [Column("deleted_at", TypeName = "datetime2")]
    public DateTime? DeletedAt { get; set; }
}