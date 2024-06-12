namespace NET1814_MilkShop.Repositories.Models.ShipModels;

public class ShippingFeeRequestModel
{
    public int FromDistrictId { get; set; }
    public string FromWardCode { get; set; } = null!;
}