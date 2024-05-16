using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWP391_DEMO.Entities;

[Table("CustomerAddresses")]
public partial class CustomerAddress
{
    [Key]
    public int Id { get; set; }

    public string? Address { get; set; }
    [Column("province_id")]
    public int ProvinceId { get; set; }
    [Column("province_name")]
    [StringLength(255)]
    public string? ProvinceName { get; set; }
    [Column("district_id")]
    public int DistrictId { get; set; }
    [Column("district_name")]
    [StringLength(255)]
    public string? DistrictName { get; set; }
    [Column("ward_code")]
    public int WardCode { get; set; }
    [Column("ward_name")]
    [StringLength(255)]
    public string? WardName { get; set; }
    public string? PhoneNumber { get; set; }
    [Column("receiver_name")]
    [StringLength(255)]
    public string? ReceiverName { get; set; }
    [Column("is_default")]
    public bool IsDefault { get; set; }
    public Guid? UserId { get; set; }
    [Column("created_at", TypeName ="datetime2")]
    public DateTime? CreatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual Customer? User { get; set; }
}
