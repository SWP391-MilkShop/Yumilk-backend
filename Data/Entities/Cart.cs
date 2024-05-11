using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWP391_DEMO.Entities;
[Table("Carts")]
public partial class Cart
{
    [Key]
    public int Id { get; set; }

    public Guid? CustomerId { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<CartDetail> CartDetails { get; set; } = new List<CartDetail>();

    public virtual Customer? Customer { get; set; }
}
