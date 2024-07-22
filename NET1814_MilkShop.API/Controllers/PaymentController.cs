using Microsoft.AspNetCore.Mvc;
using NET1814_MilkShop.API.CoreHelpers.Extensions;
using NET1814_MilkShop.Services.Services.Interfaces;
using ILogger = Serilog.ILogger;

namespace NET1814_MilkShop.API.Controllers;

[ApiController]
[Route("api/payment-requests")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger _logger;

    public PaymentController(IServiceProvider serviceProvider, ILogger logger)
    {
        _paymentService = serviceProvider.GetRequiredService<IPaymentService>();
        _logger = logger;
    }

    /// <summary>
    /// Create payment link by OrderCode in database
    /// Check exist order by OrderCode, check exist Customer, check exist any item in cart, expire link after 5 mins
    /// 200 (Tạo link thanh toán thành công) 500 (Tạo link thanh toán thất bại)
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <response code="400">Order detail in our database has no item</response>
    /// <response code="500">The api PayOS either respond error or our server has encounter a problem </response>
    [HttpPost("{id}")]
    public async Task<IActionResult> CreatePaymentLink(int id)
    {
        _logger.Information("Create payment link order");
        var response = await _paymentService.CreatePaymentLink(id);
        return ResponseExtension.Result(response);
    }

    /// <summary>
    /// Get payment link information by OrderId in database
    /// Check exist order by orderId, check exist transaction code
    /// check data response (code == null & code != "00") -> return Error (Có lỗi trong quá trình lấy dữ liệu thông tin thanh toán)
    /// check data if null or empty -> return Error (Không tồn tại thông tin thanh toán)
    /// </summary>
    /// <param name="orderId"></param>
    /// <returns></returns>
    [HttpGet("{orderId}")]
    public async Task<IActionResult> GetPaymentLinkInformation(Guid orderId)
    {
        _logger.Information("Get payment link information");
        var response = await _paymentService.GetPaymentLinkInformation(orderId);
        return ResponseExtension.Result(response);
    }

    /// <summary>
    /// Cancel payment link by OrderId in database
    /// Check exist order by orderId, check exist transaction code
    /// Check cancel success or error
    /// </summary>
    /// <param name="orderId"></param>
    /// <returns></returns>
    [HttpPost("cancel/{orderId}")]
    public async Task<IActionResult> CanclePaymentLink(Guid orderId)
    {
        _logger.Information("Cancel payment link");
        var response = await _paymentService.CancelPaymentLink(orderId);
        return ResponseExtension.Result(response);
    }
}