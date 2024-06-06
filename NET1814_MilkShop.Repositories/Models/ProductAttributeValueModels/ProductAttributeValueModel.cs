namespace NET1814_MilkShop.Repositories.Models.ProductAttributeValueModels;

public class ProductAttributeValueModel
{
    public Guid ProductId { get; set; }

    public int AttributeId { get; set; }

    public string? Value { get; set; }
}
