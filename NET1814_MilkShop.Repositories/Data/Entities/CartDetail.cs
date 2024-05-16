using System.ComponentModel.DataAnnotations.Schema;

namespace NET1814_MilkShop.Repositories.Data.Entities;

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
