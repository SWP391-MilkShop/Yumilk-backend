

using Newtonsoft.Json;

namespace NET1814_MilkShop.Repositories.Models.ShippingModels;

public class CreateOrderShippingModel
{
    /// <summary>
    /// Choose who pay shipping fee. 1: Shop/Seller. 2: Buyer/Consignee.
    /// </summary>
    [JsonProperty("payment_type_id")]
    public int PaymentTypeId { get; set; } 
    [JsonProperty("note")]
    public string? Note { get; set; }
    /// <summary>
    /// Note shipping order.Allowed values: 
    /// CHOTHUHANG mean Buyer can request to see and trial goods
    /// CHOXEMHANGKHONGTHU mean Buyer can see goods but not allow to trial goods
    /// KHONGCHOXEMHANG mean Buyer not allow to see goods
    /// </summary>
    [JsonProperty("required_note")]
    public string RequiredNote { get; set; } = "CHOXEMHANGKHONGTHU";
    [JsonProperty("to_name")]
    public string? ToName { get; set; }
    [JsonProperty("to_phone")]
    public string? ToPhone { get; set; }
    [JsonProperty("to_address")]
    public string? ToAddress { get; set; }
    [JsonProperty("to_ward_code")]
    public string? ToWardCode { get; set; }
    [JsonProperty("to_district_id")]
    public int? ToDistrictId { get; set; }
    [JsonProperty("cod_amount")]
    public int CodAmount { get; set; }
    [JsonProperty("weight")]
    public int Weight { get; set; } //Set cứng vì GHN bắt buộc cần Weight :D
    [JsonProperty("service_type_id")]
    public int ServiceTypeId { get; set; } = 2; //2: E-commerce Delivery
    [JsonProperty("items")]
    public List<Item>? Items { get; set; }
}

public class Item
{
    [JsonProperty("name")]
    public string? ProductName { get; set; }
    [JsonProperty("quantity")]
    public int Quantity { get; set; }
}