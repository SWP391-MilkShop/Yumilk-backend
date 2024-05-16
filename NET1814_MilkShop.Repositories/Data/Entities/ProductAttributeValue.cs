using System.ComponentModel.DataAnnotations.Schema;

namespace NET1814_MilkShop.Repositories.Data.Entities;

[Table("ProductAttributeValues")]
public partial class ProductAttributeValue
{
    public Guid ProductId { get; set; }

    public int AttributeId { get; set; }

    public string? Value { get; set; }

    public virtual ProductAttribute Attribute { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
