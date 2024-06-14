using System.Net;
using System.Text;
using Microsoft.Extensions.Configuration;
using NET1814_MilkShop.Repositories.CoreHelpers.Enum;
using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Models.ShipModels;
using NET1814_MilkShop.Repositories.Repositories;
using NET1814_MilkShop.Repositories.UnitOfWork;
using NET1814_MilkShop.Services.CoreHelpers.Extensions;
using Newtonsoft.Json;

namespace NET1814_MilkShop.Services.Services;

public interface IShippingService
{
    Task<ResponseModel> GetProvinceAsync();
    Task<ResponseModel> GetDistrictAsync(int provinceId); 
    Task<ResponseModel> GetWardAsync(int districtId);
    Task<ResponseModel> GetShippingFeeAsync(ShippingFeeRequestModel request);
    Task<ResponseModel> CreateOrderShippingAsync(Guid orderId);
    Task<ResponseModel> PreviewOrderShippingAsync(Guid orderId);
    Task<ResponseModel> GetOrderDetailAsync(string orderCode);
}
public class ShippingService : IShippingService
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _client;
    private readonly string Token;
    private readonly string ShopId;
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ShippingService(IConfiguration configuration,
        HttpClient client,
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork)
    {
        _configuration = configuration;
        Token = _configuration["GHN:Token"];
        ShopId = _configuration["GHN:ShopId"];
        _client = client;
        _client.DefaultRequestHeaders.Add("Token", Token);
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseModel> GetProvinceAsync()
    {
        // Send the GET request to the API
        var response = await _client.GetAsync("https://dev-online-gateway.ghn.vn/shiip/public-api/master-data/province");
        
        // Read the response content
        var responseContent = await response.Content.ReadAsStringAsync();
        
        var responseModel = JsonConvert.DeserializeObject<ShippingResponseModel<List<ProvinceData>>>(responseContent);
        
        switch (response.StatusCode)
        {
            case HttpStatusCode.OK: 
                responseModel!.Data = responseModel.Data!.OrderBy(o=>o.ProvinceId).ToList();
                return ResponseModel.Success(
                    "Lấy tỉnh/thành phố thành công",
                    responseModel.Data
                );
            case HttpStatusCode.BadRequest:
                return ResponseModel.BadRequest(responseModel.Message);
            default:
                return ResponseModel.Error("Đã xảy ra lỗi khi lấy tỉnh/thành phố");
        }
    }

    public async Task<ResponseModel> GetDistrictAsync(int provinceId)
    {
        
        var url = $"https://dev-online-gateway.ghn.vn/shiip/public-api/master-data/district?province_id={provinceId}";
        
        var response = await _client.GetAsync(url);
        
        // Read the response content
        var responseContent = await response.Content.ReadAsStringAsync();
        
        var responseModel = JsonConvert.DeserializeObject<ShippingResponseModel<List<DistrictData>>>(responseContent);
        

        switch (response.StatusCode)
        {
            case HttpStatusCode.OK:
                return ResponseModel.Success(
                    "Lấy quận/huyện thành công",
                    responseModel.Data
                );
            case HttpStatusCode.BadRequest:
                return ResponseModel.BadRequest(responseModel.Message);
            default:
                return ResponseModel.Error("Đã xảy ra lỗi khi lấy quận/huyện");
        }
    }

    public async Task<ResponseModel> GetWardAsync(int districtId)
    {
                
        var url = $"https://dev-online-gateway.ghn.vn/shiip/public-api/master-data/ward?district_id={districtId}";
        
        var response = await _client.GetAsync(url);
        
        // Read the response content
        var responseContent = await response.Content.ReadAsStringAsync();
        
        var responseModel = JsonConvert.DeserializeObject<ShippingResponseModel<List<WardData>>>(responseContent);
        
        switch (response.StatusCode)
        {
            case HttpStatusCode.OK:
                return ResponseModel.Success(
                    "Lấy phường thành công",
                    responseModel.Data
                );
            case HttpStatusCode.BadRequest:
                return ResponseModel.BadRequest(responseModel.Message);
            default:
                return ResponseModel.Error("Đã xảy ra lỗi khi lấy phường/xã");
        }
    }

    public async Task<ResponseModel> GetShippingFeeAsync(ShippingFeeRequestModel request)
    {
        _client.DefaultRequestHeaders.Add("ShopId", ShopId);
        
        var url = $"https://dev-online-gateway.ghn.vn/shiip/public-api/v2/shipping-order/fee?" +
                  $"to_ward_code={request.FromWardCode}&to_district_id={request.FromDistrictId}&weight=200" +
                  $"&service_id=0&service_type_id=2";
        
        var response = await _client.GetAsync(url);
        
        var responseContent = await response.Content.ReadAsStringAsync();
        
        var responseModel = JsonConvert.DeserializeObject<ShippingResponseModel<CalculateFeeData>>(responseContent);
        
        switch (response.StatusCode)
        {
            case HttpStatusCode.OK:
                return ResponseModel.Success(
                    "Lấy phí vận chuyển thành công",
                    responseModel.Data
                );
            case HttpStatusCode.BadRequest:
                return ResponseModel.BadRequest(responseModel.Message);
            default:
                return ResponseModel.Error("Đã xảy ra lỗi khi lấy phí vận chuyển");
        }
    }

    public async Task<ResponseModel> CreateOrderShippingAsync(Guid orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, true);
        if (order is null)
        {
            return ResponseModel.BadRequest("Đơn hàng không tồn tại");
        }
        var request = new CreateOrderShippingModel
        {
            PaymentTypeId = order.PaymentMethod == "COD" ? 2 : 1,
            Note = order.Note,
            ToName = order.ReceiverName,
            ToPhone = order.PhoneNumber,
            ToAddress = order.Address,
            ToDistrictId = order.DistrictId,
            ToWardCode = order.WardCode,
            CodAmount = order.PaymentMethod == "COD" ? order.TotalAmount.ToInt() : 0,
            Items = order.OrderDetails.Select(x => new Item
            {
                ProductName = x.ProductName,
                Quantity = x.Quantity
            }).ToList()
        };
        var url = "https://dev-online-gateway.ghn.vn/shiip/public-api/v2/shipping-order/create";
        
        var json = JsonConvert.SerializeObject(request);
        // Create the content for the POST request
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        // Send the POST request
        var response = await _client.PostAsync(url, content);
        var responseContent = await response.Content.ReadAsStringAsync();
        var responseModel = JsonConvert.DeserializeObject<ShippingResponseModel<OrderResponseData>>(responseContent);
        switch (response.StatusCode)
        {
            case HttpStatusCode.OK:
                return ResponseModel.Success(
                    "Tạo đơn hàng vận chuyển thành công",
                    responseModel.Data
                );
            case HttpStatusCode.BadRequest:
                return ResponseModel.BadRequest(responseModel.Message);
            default:
                return ResponseModel.Error("Đã xảy ra lỗi khi tạo đơn hàng vận chuyển");
        }
    }

    public async Task<ResponseModel> PreviewOrderShippingAsync(Guid orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, true);
        if (order is null)
        {
            return ResponseModel.BadRequest("Đơn hàng không tồn tại");
        }
        var request = new CreateOrderShippingModel
        {
            PaymentTypeId = order.PaymentMethod == "COD" ? 2 : 1,
            Note = order.Note,
            ToName = order.ReceiverName,
            ToPhone = order.PhoneNumber,
            ToAddress = order.Address,
            ToDistrictId = order.DistrictId,
            /*ToWardCode = order.WardCode.ToString(),*/
            CodAmount = order.PaymentMethod == "COD" ? order.TotalAmount.ToInt() : 0,
            Items = order.OrderDetails.Select(x => new Item
            {
                ProductName = x.ProductName,
                Quantity = x.Quantity
            }).ToList()
        };
        
        var url = "https://dev-online-gateway.ghn.vn/shiip/public-api/v2/shipping-order/preview";
        
        var json = JsonConvert.SerializeObject(request);
        // Create the content for the POST request
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        // Send the POST request
        var response = await _client.PostAsync(url, content);
        var responseContent = await response.Content.ReadAsStringAsync();
        var responseModel = JsonConvert.DeserializeObject<ShippingResponseModel<OrderResponseData>>(responseContent);
        switch (response.StatusCode)
        {
            case HttpStatusCode.OK:
                return ResponseModel.Success(
                    "Tạo đơn hàng vận chuyển thành công",
                    responseModel.Data
                );
            case HttpStatusCode.BadRequest:
                return ResponseModel.BadRequest(responseModel.Message);
            default:
                return ResponseModel.Error("Đã xảy ra lỗi khi tạo đơn hàng vận chuyển");
        }
    }
    
    public async Task<ResponseModel> GetOrderDetailAsync(string orderCode)
    {
        var response = await _client.GetAsync($"https://dev-online-gateway.ghn.vn/shiip/public-api/v2/shipping-order/detail?order_code={orderCode}");
        
        var responseContent = await response.Content.ReadAsStringAsync();
        
        var responseModel = JsonConvert.DeserializeObject<ShippingResponseModel<OrderDetailInformation>>(responseContent);

        switch (response.StatusCode)
        {
            case HttpStatusCode.OK:
                return ResponseModel.Success(
                    "Lấy chi tiết đơn hàng thành công",
                    responseModel.Data
                );
            case HttpStatusCode.BadRequest:
                return ResponseModel.BadRequest(responseModel.Message);
            default:
                return ResponseModel.Error("Đã xảy ra lỗi khi lấy chi tiết đơn hàng");
        }
    }

}