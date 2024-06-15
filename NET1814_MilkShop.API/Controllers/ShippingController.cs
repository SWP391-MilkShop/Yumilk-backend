using Microsoft.AspNetCore.Mvc;
using NET1814_MilkShop.API.CoreHelpers.Extensions;
using NET1814_MilkShop.Repositories.Models.ShipModels;
using NET1814_MilkShop.Repositories.Models.ShippingModels;
using NET1814_MilkShop.Services.Services;
using ILogger = Serilog.ILogger;

namespace NET1814_MilkShop.API.Controllers;

[ApiController]
[Route("api/shipping")]
public class ShippingController : ControllerBase
{
    private readonly IShippingService _shippingService;
    private readonly ILogger _logger;

    public ShippingController(IServiceProvider serviceProvider, ILogger logger)
    {
        _shippingService = serviceProvider.GetRequiredService<IShippingService>();
        _logger = logger;
    }

    [HttpGet("provinces")]
    public async Task<IActionResult> GetProvince()
    {
        _logger.Information("Get all province");
        var response = await _shippingService.GetProvinceAsync();
        return ResponseExtension.Result(response);
    }

    [HttpGet("districts/{provinceId}")]
    public async Task<IActionResult> GetDistrict(int provinceId)
    {
        _logger.Information("Get all district by provinceId: {provinceId}", provinceId);
        var response = await _shippingService.GetDistrictAsync(provinceId);
        return ResponseExtension.Result(response);
    }

    [HttpGet("wards/{districtId}")]
    public async Task<IActionResult> GetWard(int districtId)
    {
        _logger.Information("Get all district by districtId: {districtId}", districtId);
        var response = await _shippingService.GetWardAsync(districtId);
        return ResponseExtension.Result(response);
    }

    [HttpGet("fee")]
    public async Task<IActionResult> GetShippingFee([FromQuery] ShippingFeeRequestModel request)
    {
        _logger.Information("Get shipping fee");
        var response = await _shippingService.GetShippingFeeAsync(request);
        return ResponseExtension.Result(response);
    }

    /// <summary>
    /// Create order shipping in ghn
    /// </summary>
    /// <param name="orderId"></param>
    /// <returns></returns>
    [HttpPost("order/create/{orderId}")]
    public async Task<IActionResult> CreateOrder(Guid orderId)
    {
        _logger.Information("Create shipping order");
        var response = await _shippingService.CreateOrderShippingAsync(orderId);
        return ResponseExtension.Result(response);
    }

    /// <summary>
    /// Preview order shipping in ghn
    /// </summary>
    /// <param name="orderId"></param>
    /// <returns></returns>
    [HttpPost("order/preview/{orderId}")]
    public async Task<IActionResult> PreviewOrder(Guid orderId)
    {
        _logger.Information("Preview shipping order");
        var response = await _shippingService.PreviewOrderShippingAsync(orderId);
        return ResponseExtension.Result(response);
    }

    /// <summary>
    /// Get order detail orderCode này là của ghn
    /// </summary>
    /// <param name="orderCode"></param>
    /// <returns></returns>
    [HttpGet("order/detail/{orderCode}")]
    public async Task<IActionResult> PreviewOrder(string orderCode)
    {
        _logger.Information("Get shipping order detail");
        var response = await _shippingService.GetOrderDetailAsync(orderCode);
        return ResponseExtension.Result(response);
    }

    [HttpGet("order/deliverytime")]
    public async Task<IActionResult> GetDeliveryTime([FromQuery] DeliveryTimeRequestModel model)
    {
        _logger.Information("Get delivery time");
        var response = await _shippingService.GetExpectedDeliveryTime(model);
        return ResponseExtension.Result(response);
    }
}