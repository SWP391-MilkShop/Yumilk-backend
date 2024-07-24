using Microsoft.AspNetCore.Authorization;
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
    
    /// <summary>
    /// Get province name in Vietnam by GHN service
    /// </summary>
    /// <returns>provinceId and provinceName</returns>
    [HttpGet("provinces")]
    public async Task<IActionResult> GetProvince()
    {
        _logger.Information("Get all province");
        var response = await _shippingService.GetProvinceAsync();
        return ResponseExtension.Result(response);
    }
    /// <summary>
    /// Get all district name by provinceId
    /// </summary>
    /// <param name="provinceId">Pass provinceId from GHN service</param>
    /// <returns>districtId and districtName</returns>
    [HttpGet("districts/{provinceId}")]
    public async Task<IActionResult> GetDistrict(int provinceId)
    {
        _logger.Information("Get all district by provinceId: {provinceId}", provinceId);
        var response = await _shippingService.GetDistrictAsync(provinceId);
        return ResponseExtension.Result(response);
    }
    /// <summary>
    /// Get all ward name by districtId
    /// </summary>
    /// <param name="districtId">Pass districtId in GHN service</param>
    /// <returns>return wardId and wardName</returns>
    [HttpGet("wards/{districtId}")]
    public async Task<IActionResult> GetWard(int districtId)
    {
        _logger.Information("Get all district by districtId: {districtId}", districtId);
        var response = await _shippingService.GetWardAsync(districtId);
        return ResponseExtension.Result(response);
    }
    
    /// <summary>
    /// Get shipping fee of order by passing FromDistrictId, FromWardCode, TotalWeight (int)
    /// </summary>
    /// <param name="model">FromDistrictId, FromWardCode, TotalWeight (int)</param>
    /// <returns></returns>
    [HttpGet("fee")]
    public async Task<IActionResult> GetShippingFee([FromQuery] ShippingFeeRequestModel model)
    {
        _logger.Information("Get shipping fee");
        var response = await _shippingService.GetShippingFeeAsync(model);
        return ResponseExtension.Result(response);
    }

    /// <summary>
    /// Create order shipping in ghn
    /// </summary>
    /// <param name="orderId">orderId in Yumilk shop</param>
    /// <returns></returns>
    [HttpPost("order/create/{orderId}")]
    [Authorize(AuthenticationSchemes = "Access", Roles = "1,2")]
    public async Task<IActionResult> CreateOrder(Guid orderId)
    {
        _logger.Information("Create shipping order");
        var response = await _shippingService.CreateOrderShippingAsync(orderId);
        return ResponseExtension.Result(response);
    }

    /// <summary>
    ///  Preview order shipping in ghn this will only preview not create order in GHN service
    /// </summary>
    /// <param name="orderId">orderId in Yumilk shop</param>
    /// <returns></returns>
    [HttpPost("order/preview/{orderId}")]
    public async Task<IActionResult> PreviewOrder(Guid orderId)
    {
        _logger.Information("Preview shipping order");
        var response = await _shippingService.PreviewOrderShippingAsync(orderId);
        return ResponseExtension.Result(response);
    }

    /// <summary>
    /// Get tracking log from GHN service
    /// </summary>
    /// <param name="orderId">orderId in Yumilk Shop</param>
    /// <returns></returns>
    [HttpGet("order/tracking/{orderId}")]
    [Authorize(AuthenticationSchemes = "Access")]
    public async Task<IActionResult> GetOrderDetail(Guid orderId)
    {
        _logger.Information("Get shipping order logs");
        var response = await _shippingService.GetOrderDetailAsync(orderId);
        return ResponseExtension.Result(response);
    }

    /// <summary>
    ///  Cancel shipping order by GHN service
    /// </summary>
    /// <param name="orderId">orderId in Yumilk Shop</param>
    /// <returns></returns>
    [HttpPost("order/cancel/{orderId}")]
    [Authorize(AuthenticationSchemes = "Access", Roles = "1,2")]
    public async Task<IActionResult> CancelOrder(Guid orderId)
    {
        _logger.Information("Cancel shipping order");
        var response = await _shippingService.CancelOrderShippingAsync(orderId);
        return ResponseExtension.Result(response);
    }
}