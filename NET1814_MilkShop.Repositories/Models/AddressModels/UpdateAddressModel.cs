using System.ComponentModel.DataAnnotations;

namespace NET1814_MilkShop.Repositories.Models.AddressModels;

public class UpdateAddressModel
{

    [Required(ErrorMessage = "Receiver name is required")]
    public string? ReceiverName { get; set; }

    [Required(ErrorMessage = "Receiver phone is required")]
    public string? ReceiverPhone { get; set; }

    [Required(ErrorMessage = "ProvinceId is required")]
    public int ProvinceId { get; set; }

    [Required(ErrorMessage = "ProvinceName is required")]
    public string? ProvinceName { get; set; }

    [Required(ErrorMessage = "DistrictId is required")]
    public int DistrictId { get; set; }

    [Required(ErrorMessage = "DistrictName is required")]
    public string? DistrictName { get; set; }

    [Required(ErrorMessage = "WardId is required")]
    public int WardId { get; set; }

    [Required(ErrorMessage = "WardName is required")]
    public string? WardName { get; set; }

    [Required(ErrorMessage = "Address is required")]
    public string? Address { get; set; }

    public bool IsDefault { get; set; } = false;
}