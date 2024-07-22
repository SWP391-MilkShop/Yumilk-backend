using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NET1814_MilkShop.API.CoreHelpers.Extensions;
using NET1814_MilkShop.Repositories.Models.OrderModels;
using NET1814_MilkShop.Repositories.Models.ProductModels;
using NET1814_MilkShop.Repositories.Models.UserModels;
using NET1814_MilkShop.Services.Services.Interfaces;
using ILogger = Serilog.ILogger;

namespace NET1814_MilkShop.API.Controllers;

[ApiController]
[Route("api/dashboard")]
public class DashboardController : Controller
{
    private readonly IOrderService _orderService;
    private readonly IProductService _productService;
    private readonly ICustomerService _customerService;
    private readonly ILogger _logger;

    public DashboardController(IOrderService orderService, IProductService productService, ILogger logger,
        ICustomerService customerService)
    {
        _orderService = orderService;
        _productService = productService;
        _logger = logger;
        _customerService = customerService;
    }

    /// <summary>
    /// Filter theo thời gian, isPreorder, OrderStatus, PaymentMethod, Email, TotalAmount
    /// <para>Sort mặc định theo status asc, created date desc</para>
    /// <para>Nếu có shipping code, fetch data từ API GHN để lấy ngày nhận hàng dự kiến</para>
    /// </summary>
    /// <param name="queryModel"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("/api/dashboard/orders")]
    [Authorize(AuthenticationSchemes = "Access", Roles = "1,2")]
    public async Task<IActionResult> GetOrders([FromQuery] OrderQueryModel queryModel)
    {
        _logger.Information("Get all orders");
        var response = await _orderService.GetOrderAsync(queryModel);
        return ResponseExtension.Result(response);
    }

    /// <summary>
    /// Get order stats
    /// Total number of orders
    /// Total number of orders per status
    /// Total revenue (only count orders that have been delivered)
    /// Total shipping fee (only count orders that have been delivered)
    /// </summary>
    /// <param name="queryModel"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("orders/stats")]
    [Authorize(AuthenticationSchemes = "Access", Roles = "1,2")]
    public async Task<IActionResult> GetOrderStats([FromQuery] OrderStatsQueryModel queryModel)
    {
        _logger.Information("Get order stats");
        var response = await _orderService.GetOrderStatsAsync(queryModel);
        return ResponseExtension.Result(response);
    }

    /// <summary>
    /// PENDING -> PROCESSING -> SHIPPED
    /// <para>PREORDER - SHIPPED (cập nhật trừ số lượng đặt hàng)</para>
    /// <para>Chặn các case còn lại</para>
    /// </summary>
    /// <param name="id"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPatch]
    [Route("orders/{id}/status")]
    [Authorize(AuthenticationSchemes = "Access", Roles = "1,2")]
    public async Task<IActionResult> UpdateOrderStatus(Guid id, [FromBody] OrderStatusModel model)
    {
        _logger.Information("Update order status");
        var response = await _orderService.UpdateOrderStatusAsync(id, model);
        return ResponseExtension.Result(response);
    }

    /// <summary>
    /// Chuyển từ SHIPPED sang DELIVERED
    /// <para>Cộng point cho user = total price * 1% </para>
    /// <para>Thêm order log, cập nhật payment date cho order COD</para>
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPatch]
    [Route("orders/{id}/status/delivered")]
    [Authorize(AuthenticationSchemes = "Access", Roles = "1,2")]
    public async Task<IActionResult> UpdateOrderStatusDelivered(Guid id)
    {
        _logger.Information("Update order status to delivered");
        var response = await _orderService.UpdateOrderStatusDeliveredAsync(id);
        return ResponseExtension.Result(response);
    }

    /// <summary>
    /// Get product stats (total sold, revenue per brand, category)
    /// <para>List 5 sp best sellers (total sold, revenue) order by total sold desc then by total revenue desc</para>
    /// </summary>
    /// <param name="queryModel"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("products/stats")]
    [Authorize(AuthenticationSchemes = "Access", Roles = "1,2")]
    public async Task<IActionResult> GetProductStats([FromQuery] ProductStatsQueryModel queryModel)
    {
        _logger.Information("Get product stats");
        var response = await _productService.GetProductStatsAsync(queryModel);
        return ResponseExtension.Result(response);
    }

    /// <summary>
    /// Get users stats
    /// Total new customers,
    /// Total customers who have bought any product
    /// </summary>
    /// <param name="queryModel"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("customers/stats")]
    [Authorize(AuthenticationSchemes = "Access", Roles = "1,2")]
    public async Task<IActionResult> GetCustomersStats([FromQuery] CustomersStatsQueryModel queryModel)
    {
        _logger.Information("Get users stats");
        var res = await _customerService.GetCustomersStatsAsync(queryModel);
        return ResponseExtension.Result(res);
    }
    // [HttpGet]
    // [Route("customers/{id}/stats")]
    // [Authorize(AuthenticationSchemes = "Access", Roles = "1,2")]
    // public async Task<IActionResult> GetCustomerStats(Guid id)
    // {
    //     _logger.Information("Get user stats");
    //     var res = await _customerService.GetCustomersStatsByIdAsync(id);
    //     return ResponseExtension.Result(res);
    // }

    /// <summary>
    /// Admin and Staff have full permission to cancel order (PREORDER, PROCESSING, SHIPPING).
    /// <para>If an order is already in shipping, preorder status (order has been created in GHN),</para>
    /// <para>Admin or Staff must manually cancel shipping order in GHN.</para>
    /// <para>Nếu payment method là PAYOS, hủy link thanh toán</para>
    /// <para>Trả point nếu có sử dụng point</para>
    /// <para>Cập nhật quantity product khi hủy đơn hàng</para>
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPatch]
    [Route("orders/cancel/{id}")]
    [Authorize(AuthenticationSchemes = "Access", Roles = "1,2")]
    public async Task<IActionResult> CancelOrder(Guid id)
    {
        _logger.Information("Cancel order");
        var response = await _orderService.CancelOrderAdminStaffAsync(id);
        return ResponseExtension.Result(response);
    }

    /// <summary>
    /// Lấy chi tiết đơn hàng + order log, lấy log GHN (nếu có)
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("orders/{id}")]
    [Authorize(AuthenticationSchemes = "Access", Roles = "1,2")]
    public async Task<IActionResult> GetOrderHistoryDetail(Guid id)
    {
        _logger.Information("Get order detail history");
        var res = await _orderService.GetOrderHistoryDetailDashBoardAsync(id);
        return ResponseExtension.Result(res);
    }

    /// <summary>
    /// Lấy tỉ lệ sử dụng payment method COD với PAYOS
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("payment/stats/payment-methods")]
    [Authorize(AuthenticationSchemes = "Access", Roles = "1")]
    public async Task<IActionResult> GetPaymentMethodStats()
    {
        _logger.Information("Get payment method stats");
        var res = await _orderService.GetPaymentMethodStats();
        return ResponseExtension.Result(res);
    }

    /// <summary>
    /// tổng đơn hàng đặt trong thứ ngày tháng
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("orders/stats/orders-by-date")]
    [Authorize(AuthenticationSchemes = "Access", Roles = "1,2")]
    public async Task<IActionResult> GetOrdersStatsByDate([FromQuery] OrderStatsQueryModel model)
    {
        _logger.Information("Get orders stats by date");
        var res = await _orderService.GetOrdersStatsByDateAsync(model);
        return ResponseExtension.Result(res);
    }

    /// <summary>
    /// khách hàng quay trở lại mua hàng theo quý trong năm
    /// </summary>
    /// <param name="year"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("customers/stats/{year}/returning-customers")]
    [Authorize(AuthenticationSchemes = "Access", Roles = "1")]
    public async Task<IActionResult> GetReturnCustomersStats(int year)
    {
        _logger.Information("Get customers return stats by year");
        var res = await _customerService.GetReturnCustomerStatsAsync(year);
        return ResponseExtension.Result(res);
    }

    /// <summary>
    ///  Get revenue by each month, enter a year to get revenue by month
    /// </summary>
    /// <param name="year"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("orders/stats/{year}/revenue-by-month")]
    [Authorize(AuthenticationSchemes = "Access", Roles = "1")]
    public async Task<IActionResult> GetRevenueByMonth(int year)
    {
        _logger.Information("Get revenue by month");
        var res = await _orderService.GetRevenueByMonthAsync(year);
        return ResponseExtension.Result(res);
    }

    /// <summary>
    /// Lấy 5 khách hàng mua nhiều nhất theo giá trị đơn hàng
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("customers/stats/total-purchase")]
    [Authorize(AuthenticationSchemes = "Access", Roles = "1,2")]
    public async Task<IActionResult> GetTotalPurchase()
    {
        _logger.Information("Get total purchase");
        var res = await _customerService.GetTotalPurchaseAsync();
        return ResponseExtension.Result(res);
    }

    /// <summary>
    /// Lấy số lượng đơn hàng và tổng giá trị đơn hàng của khách hàng theo năm
    /// </summary>
    /// <param name="id"></param>
    /// <param name="year"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("customers/{id}/stats/{year}/total-purchase/")]
    [Authorize(AuthenticationSchemes = "Access", Roles = "1,2")]
    public async Task<IActionResult> GetTotalPurchaseByCustomer(Guid id, int year)
    {
        _logger.Information("Get total purchase by customer");
        var res = await _customerService.GetTotalPurchaseByCustomerAsync(id, year);
        return ResponseExtension.Result(res);
    }
}