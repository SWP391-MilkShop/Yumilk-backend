using Microsoft.AspNetCore.Mvc;
using NET1814_MilkShop.API.CoreHelpers.Extensions;
using NET1814_MilkShop.Repositories.Models.ShippingModels;
using NET1814_MilkShop.Services.Services.Interfaces;
using ILogger = Serilog.ILogger;

namespace NET1814_MilkShop.API.Controllers;

[ApiController]
[Route("api/shipping")]
public class ShippingController : ControllerBase
{
    private readonly ILogger _logger;
    private readonly IShippingService _shippingService;

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


    /// <summary>
    ///     Get shipping fee of order
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpGet("fee")]
    public async Task<IActionResult> GetShippingFee([FromQuery] ShippingFeeRequestModel model)
    {
        _logger.Information("Get shipping fee");
        var response = await _shippingService.GetShippingFeeAsync(model);
        return ResponseExtension.Result(response);
    }

    /// <summary>
    ///     Create order shipping in ghn
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
    ///     Preview order shipping in ghn
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
    ///     Get order detail by our Guid orderId (not ghn shippingCode)
    /// </summary>
    /// <param name="orderId"></param>
    /// <returns></returns>
    [HttpGet("order/detail/{orderId}")]
    public async Task<IActionResult> GetOrderDetail(Guid orderId)
    {
        _logger.Information("Get shipping order detail");
        var response = await _shippingService.GetOrderDetailAsync(orderId);
        return ResponseExtension.Result(response);
    }

    /// <summary>
    ///     Cancel shipping order by our Guid orderId (not ghn shippingCode)
    /// </summary>
    /// <param name="orderId"></param>
    /// <returns></returns>
    [HttpPost("order/cancel/{orderId}")]
    public async Task<IActionResult> CancelOrder(Guid orderId)
    {
        _logger.Information("Cancel shipping order");
        var response = await _shippingService.CancelOrderShippingAsync(orderId);
        return ResponseExtension.Result(response);
    }
}