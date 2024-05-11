using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWP391_DEMO.Entities;
[Table("CustomerAddresses")]
public partial class CustomerAddress
{
    [Key]
    public int Id { get; set; }

    public string? Address { get; set; }

    public string? PhoneNumber { get; set; }

    public Guid? UserId { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual Customer? User { get; set; }
}
