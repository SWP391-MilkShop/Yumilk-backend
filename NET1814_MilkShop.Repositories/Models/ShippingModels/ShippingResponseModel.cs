using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace NET1814_MilkShop.Repositories.Models.ShipModels;

public class ShippingResponseModel<TValue> where TValue : class
{
    public int Code { get; set; }
    public string? Message { get; set; }
    public TValue? Data { get; set; }
}

public class ProvinceData
{
    public int ProvinceId { get; set; }
    public string? ProvinceName { get; set; }
    public string? Code { get; set; }
}

public class DistrictData
{
    public int DistrictId { get; set; }
    public int ProvinceId { get; set; }
    public string? DistrictName { get; set; }
    public string? Code { get; set; }
}

public class WardData
{
    public int WardCode { get; set; }
    public int DistrictId { get; set; }
    public string? WardName { get; set; }
}

public class CalculateFeeData
{
    public int Total { get; set; }
    public int ServiceFee { get; set; }
    public int InsuranceFee { get; set; }
}

public class OrderResponseData
{
    [JsonProperty("order_code")]
    public string? OrderCode { get; set; }
    [JsonProperty("trans_type")]
    public string? TransType { get; set; }
    [JsonProperty("total_fee")]
    public int? TotalFee { get; set; }
    [JsonProperty("expected_delivery_time")]
    public string? ExpectedDeliveryTime { get; set; }
}

public class OrderDetailInformation
{
    [JsonProperty("shop_id")]
    public int ShopId { get; set; }
    [JsonProperty("client_id")]
    public int ClientId { get; set; }
    [JsonProperty("return_named")]
    public string? ReturnName { get; set; }
    [JsonProperty("return_phone")]
    public string? ReturnPhone { get; set; }
    [JsonProperty("return_address")]
    public string? ReturnAddress { get; set; }
    [JsonProperty("return_ward_code")]
    public string? ReturnWardCode { get; set; }
    [JsonProperty("return_district_id")]
    public int ReturnDistrictId { get; set; }
    [JsonProperty("from_name")]
    public string? FromName { get; set; }
    [JsonProperty("from_phone")]
    public string? FromPhone { get; set; }
    [JsonProperty("from_hotline")]
    public string? FromHotline { get; set; }
    [JsonProperty("from_address")]
    public string? FromAddress { get; set; }
    [JsonProperty("from_ward_code")]
    public string? FromWardCode { get; set; }
    [JsonProperty("from_district_id")]
    public int FromDistrictId { get; set; }
    [JsonProperty("to_name")]
    public string? ToName { get; set; }
    [JsonProperty("to_phone")]
    public string? ToPhone { get; set; }
    [JsonProperty("to_address")]
    public string? ToAddress { get; set; }
    [JsonProperty("to_ward_code")]
    public string? ToWardCode { get; set; }
    [JsonProperty("to_district_id")]
    public int ToDistrictId { get; set; }
    [JsonProperty("payment_type_id")]
    public int PaymentTypeId { get; set; }
    [JsonProperty("cod_amount")]
    public int CodAmount { get; set; }
    [JsonProperty("cod_collect_date")]
    public object? CodCollectDate { get; set; }
    [JsonProperty("cod_transfer_date")]
    public object? CodTransferDate { get; set; }
    [JsonProperty("is_cod_transferred")]
    public bool IsCodTransferred { get; set; }
    [JsonProperty("is_cod_collected")]
    public bool IsCodCollected { get; set; }
    [JsonProperty("cod_failed_amount")]
    public int CodFailedAmount { get; set; }
    [JsonProperty("cod_failed_collect_date")]
    public object? CodFailedCollectDate { get; set; }
    [JsonProperty("required_note")]
    public string? RequiredNote { get; set; }
    [JsonProperty("content")]
    public string? Content { get; set; }
    [JsonProperty("note")]
    public string? Note { get; set; }
    [JsonProperty("pickup_time")]
    public DateTime PickupTime { get; set; }
    [JsonProperty("log")]
    public List<Log> Logs { get; set; } = [];
    [JsonPropertyName("items")]
    public List<Item> Items { get; set; } = [];
}

public class Log
{
    [JsonProperty("status")]
    public string? Status { get; set; }
    [JsonProperty("updated_date")]
    public string? UpdatedDate { get; set; }
}