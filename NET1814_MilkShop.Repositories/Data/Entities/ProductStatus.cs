using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NET1814_MilkShop.Repositories.Data.Entities;

[Table("ProductStatuses")]
public partial class ProductStatus
{
    [Key]
    public int Id { get; set; }

    [Column(TypeName = "nvarchar(255)")]
    public string? Name { get; set; } //SELLING, OUTOFSTOCK, PREORDER

    [Column(TypeName = "nvarchar(2000)")]
    public string? Description { get; set; }
}
