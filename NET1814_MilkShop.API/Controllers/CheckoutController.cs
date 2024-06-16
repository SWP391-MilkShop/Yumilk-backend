using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NET1814_MilkShop.API.CoreHelpers.ActionFilters;
using NET1814_MilkShop.API.CoreHelpers.Extensions;
using NET1814_MilkShop.Repositories.Models.CheckoutModels;
using NET1814_MilkShop.Services.Services;
using ILogger = Serilog.ILogger;

namespace NET1814_MilkShop.API.Controllers;

[ApiController]
[Route("api/checkout")]
public class CheckoutController : ControllerBase
{
    private readonly ICheckoutService _checkoutService;
    private readonly ILogger _logger;

    public CheckoutController(ICheckoutService checkoutService, ILogger logger)
    {
        _checkoutService = checkoutService;
        _logger = logger;
    }

    /// <summary>
    /// checkout order
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize(AuthenticationSchemes = "Access", Roles = "3")]
    [ServiceFilter(typeof(UserExistsFilter))]
    public async Task<IActionResult> Checkout([FromBody] CheckoutModel model)
    {
        _logger.Information("Checkout");
        var userId = (HttpContext.Items["UserId"] as Guid?)!.Value;
        var res = await _checkoutService.Checkout(userId, model);
        return ResponseExtension.Result(res);
    }

    /// <summary>
    /// checkout preorder
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("preorder")]
    [Authorize(AuthenticationSchemes = "Access", Roles = "3")]
    [ServiceFilter(typeof(UserExistsFilter))]
    public async Task<IActionResult> PreOrderCheckout([FromBody] PreorderCheckoutModel model)
    {
        _logger.Information("PreOrderCheckout");
        var userId = (HttpContext.Items["UserId"] as Guid?)!.Value;
        var res = await _checkoutService.PreOrderCheckout(userId, model);
        return ResponseExtension.Result(res);
    }
}