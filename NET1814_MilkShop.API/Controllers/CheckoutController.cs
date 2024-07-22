using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NET1814_MilkShop.API.CoreHelpers.ActionFilters;
using NET1814_MilkShop.API.CoreHelpers.Extensions;
using NET1814_MilkShop.Repositories.Models.CheckoutModels;
using NET1814_MilkShop.Services.Services.Interfaces;
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
    /// Chặn tài khoản chưa kích hoạt hoặc bị khóa
    /// <para>Chặn nếu cart ko có item, item vượt quá số lượng, item ko còn trạng thái selling</para>
    /// <para>Yêu cầu phải có địa chỉ</para>
    /// <para>Apply Voucher (nếu có, validate đk sử dụng, trừ quantity voucher)</para>
    /// <para>Apply Point (nếu có, point > 100, point max = 50% total price (đã trừ voucher)</para>
    /// <para>Xóa cart details, trừ quantity sản phẩm trong kho, thêm order log</para>
    /// <para>Nếu là PAYOS thì tạo payment link</para>
    /// <para>Gửi mail xác nhận đơn hàng</para>
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
    /// Handle tương tự checkout
    /// <para>Chặn nếu quantity đặt đã vượt quá số lượng đặt trước tối đa cho phép</para>
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