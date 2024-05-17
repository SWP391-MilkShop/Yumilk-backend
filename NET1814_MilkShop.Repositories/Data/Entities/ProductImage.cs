using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NET1814_MilkShop.Repositories.Data.Entities;

[Table("ProductImages")]
public partial class ProductImage
{
    [Key]
    public int Id { get; set; }

    public Guid? ProductId { get; set; }

    public string ImageUrl { get; set; } = null!;

    [DefaultValue(false)]
    public bool IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual Product? Product { get; set; }
}
