namespace NET1814_MilkShop.Repositories.Models.ShippingModels;

public class DeliveryTimeRequestModel
{
    public int FromDistrictId { get; set; }
    public string FromWardCode { get; set; } = null!;
    public int ToDistrictId { get; set; }
    public string ToWardCode { get; set; } = null!;
}