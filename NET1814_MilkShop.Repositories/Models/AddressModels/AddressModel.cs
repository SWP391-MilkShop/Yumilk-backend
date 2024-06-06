namespace NET1814_MilkShop.Repositories.Models.AddressModels;

public class AddressModel
{
    public int Id { get; set; }
    public string? ReceiverName { get; set; }

    public string? ReceiverPhone { get; set; }

    public string? Address { get; set; }

    public string? WardName { get; set; }

    public string? DistrictName { get; set; }

    public string? ProvinceName { get; set; }

    public bool IsDefault { get; set; }
}
