using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SWP391_DEMO.Entities;

[Table("CartDetails")]
public partial class CartDetail
{
    public int CartId { get; set; }
    public Guid ProductId { get; set; }

    public int? Quantity { get; set; }

    public bool? IsActive { get; set; }

    public virtual Cart Cart { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
