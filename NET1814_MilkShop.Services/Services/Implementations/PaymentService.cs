using System.Configuration;
using Microsoft.Extensions.Configuration;
using Net.payOS;
using Net.payOS.Types;
using Net.payOS.Utils;
using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Repositories.Interfaces;
using NET1814_MilkShop.Services.Services.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NET1814_MilkShop.Services.Services.Implementations;

public class PaymentService : IPaymentService
{
    private readonly PayOS _payOs;
    private readonly IConfiguration _configuration;
    private readonly IOrderRepository _orderRepository;
    private readonly HttpClient _client;

    public PaymentService(IConfiguration configuration,
        IOrderRepository orderRepository,
        HttpClient client)
    {
        _client = client;
        _configuration = configuration;
        _orderRepository = orderRepository;
        _payOs ??= new PayOS(
            _configuration["PayOS:ClientId"]!,
            _configuration["PayOS:ApiKey"]!,
            _configuration["PayOS:CheckSumKey"]!
        );
    }

    public async Task<ResponseModel> CreatePaymentLink(int orderCode)
    {
        try
        {
            var order = await _orderRepository.GetByCodeAsync(orderCode);
            if (order is null || order.OrderCode is null)
            {
                return ResponseModel.BadRequest("Không tìm thấy đơn hàng");
            }

            if (order.Customer is null)
            {
                return ResponseModel.Error(
                    "Không tìm thấy thông tin khách hàng. Vui lòng kiểm tra lại tài khoản"
                );
            }

            var items = order
                .OrderDetails.Select(item => new ItemData(
                    item.Product.Name,
                    item.Quantity,
                    item.ItemPrice
                ))
                .ToList();
            if (items.Count <= 0)
            {
                return ResponseModel.BadRequest("Không tìm thấy sản phẩm trong đơn hàng");
            }

            var customerName = $"{order.Customer?.User.FirstName} {order.Customer?.User.LastName}";
            var customerEmail = order.Customer?.Email;
            var customerPhone = order.Customer?.PhoneNumber;
            var description = $"{orderCode} Shipfee: {order.ShippingFee}đ";
            var expiredAt = (int)DateTimeOffset.UtcNow.AddMinutes(15).ToUnixTimeSeconds();
            var paymentData = new PaymentData(
                (long)order.OrderCode,
                order.TotalAmount,
                description,
                items,
                _configuration["PayOS:CancelUrl"]! + order.Id,
                _configuration["PayOS:ReturnUrl"]! + order.Id,
                buyerName: customerName,
                buyerEmail: customerEmail,
                buyerPhone: customerPhone,
                expiredAt: expiredAt
            );
            var createPayment = await _payOs.createPaymentLink(paymentData);
            return createPayment.status == "ERROR"
                ? ResponseModel.Error("Tạo link thanh toán thất bại")
                : ResponseModel.Success("Tạo link thanh toán thành công", createPayment);
        }
        catch (Exception ex)
        {
            return ResponseModel.Error(ex.Message);
        }
    }

    /*public async Task<ResponseModel> GetPaymentLinkInformation(Guid orderId)
    {
        try
        {
            var existOrder = await _orderRepository.GetByIdNoIncludeAsync(orderId);
            if (existOrder is null)
            {
                return ResponseModel.BadRequest("Không tìm thấy đơn hàng");
            }

            if (existOrder.OrderCode is null)
            {
                return ResponseModel.BadRequest("Không tìm thấy mã đơn hàng thanh toán");
            }

            var orderCode = (long)existOrder.OrderCode;
            var paymentLinkInformation = await _payOs.getPaymentLinkInformation(orderCode);
            if (paymentLinkInformation.status == "Error")
            {
                return ResponseModel.Error(
                    "Đã có lỗi xảy ra trong quá trình lấy thông tin link thanh toán"
                );
            }

            return ResponseModel.Success(
                "Lấy thông tin link thanh toán thành công",
                paymentLinkInformation
            );
        }
        catch (Exception ex)
        {
            return ResponseModel.Error(ex.Message);
        }
    }*/
    
    public async Task<ResponseModel> GetPaymentLinkInformation(Guid orderId)
   {
       try
       {
           var existOrder = await _orderRepository.GetByIdNoIncludeAsync(orderId);
           if (existOrder is null)
           {
               return ResponseModel.BadRequest("Không tìm thấy đơn hàng");
           }

           if (existOrder.OrderCode is null)
           {
               return ResponseModel.BadRequest("Không tìm thấy mã đơn hàng thanh toán");
           }

           var orderCode = (long)existOrder.OrderCode;
           _client.DefaultRequestHeaders.Add("x-client-id", _configuration["PayOS:ClientId"]);
           _client.DefaultRequestHeaders.Add("x-api-key", _configuration["PayOS:ApiKey"]);
           var response =
               await _client.GetAsync("https://api-merchant.payos.vn/v2/payment-requests/"+ orderCode) ;
           // Read the response content
           var responseContent = await response.Content.ReadAsStringAsync();
           var responseBodyJson = JObject.Parse(responseContent);
           var code = responseBodyJson["code"]?.ToString();
           var data = responseBodyJson["data"]?.ToString();
           string paymentLinkResSignature = SignatureControl.CreateSignatureFromObj(responseBodyJson, _configuration["PayOS:CheckSumKey"]!);
           if (paymentLinkResSignature != responseBodyJson["signature"]!.ToString())
           {
               return ResponseModel.Error("Signature không hợp lệ: paymentLinkResSignature(our server):"
                   + paymentLinkResSignature+"\nPayOSSignature:"+responseBodyJson["signature"]);
           }
               
           if (code == null && code != "00")
           {
               return ResponseModel.Error("Có lỗi trong quá trình lấy dữ liệu thông tin thanh toán");
           }

           if (data == null)
           {
               return ResponseModel.Error("Không tồn tại thông tin thanh toán");
           }
           var payOsData = JsonConvert.DeserializeObject<PaymentLinkInformation>(data);
           return ResponseModel.Success(
               "Lấy thông tin link thanh toán thành công",
               payOsData
           );
       }
       catch (Exception ex)
       {
           return ResponseModel.Error(ex.Message);
       }
   }

    public async Task<ResponseModel> CancelPaymentLink(Guid orderId)
    {
        try
        {
            var existOrder = await _orderRepository.GetByIdNoIncludeAsync(orderId);
            if (existOrder is null)
            {
                return ResponseModel.BadRequest("Không tìm thấy đơn hàng");
            }

            if (existOrder.OrderCode is null)
            {
                return ResponseModel.BadRequest("Không tìm thấy mã đơn hàng thanh toán");
            }

            var orderCode = existOrder.OrderCode.Value;
            var cancelPaymentLink = await _payOs.cancelPaymentLink(orderCode);
            return cancelPaymentLink.status == "ERROR"
                ? ResponseModel.Error("Hủy link thanh toán thất bại")
                : ResponseModel.Success("Hủy link thanh toán thành công", cancelPaymentLink);
        }
        catch (Exception ex)
        {
            return ResponseModel.Error(ex.Message);
        }
    }
}