namespace NET1814_MilkShop.Repositories.Models.ShippingModels;

public class ShippingFeeRequestModel
{
    public int FromDistrictId { get; set; }
    public string FromWardCode { get; set; } = null!;

    public int TotalWeight { get; set; }
}