using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NET1814_MilkShop.API.CoreHelpers.Extensions;
using NET1814_MilkShop.Repositories.Models;
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

    [HttpGet]
    [Route("/api/dashboard/orders")]
    [Authorize(AuthenticationSchemes = "Access", Roles = "1,2")]
    public async Task<IActionResult> GetOrders([FromQuery] OrderQueryModel queryModel)
    {
        _logger.Information("Get all orders");
        var response = await _orderService.GetOrderAsync(queryModel);
        /*if (response.Status == "Error")
        {
            return BadRequest(response);
        }
        return Ok(response);*/
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
    /// Get product stats (total sold, revenue per brand, category)
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
    /// Total customers,
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

    /// <summary>
    /// Admin and Staff have full permission to cancel order (PREORDER, PROCESSING, SHIPPING).
    /// If an order is already in shipping, preorder status (order has been created in GHN),
    /// Admin or Staff must manually cancel shipping order in GHN.
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

    [HttpGet]
    [Route("orders/{id}")]
    [Authorize(AuthenticationSchemes = "Access", Roles = "1,2")]
    public async Task<IActionResult> GetOrderHistoryDetail(Guid id)
    {
        _logger.Information("Get order detail history");
        var res = await _orderService.GetOrderHistoryDetailDashBoardAsync(id);
        return ResponseExtension.Result(res);
    }

    [HttpGet]
    [Route("payment/stats")]
    [Authorize(AuthenticationSchemes = "Access", Roles = "1,2")]
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
    [Route("customers/returning/stats/{year}")]
    [Authorize(AuthenticationSchemes = "Access", Roles = "1,2")]
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
    [Authorize(AuthenticationSchemes = "Access",Roles="1,2")]
    public async Task<IActionResult> GetRevenueByMonth(int year)
    {
        _logger.Information("Get revenue by month");
        var res = await _orderService.GetRevenueByMonthAsync(year);
        return ResponseExtension.Result(res);
    }
    [HttpGet]
    [Route("customers/stats/total-purchase")]
    [Authorize(AuthenticationSchemes = "Access", Roles = "1,2")]
    public async Task<IActionResult> GetTotalPurchase()
    {
        _logger.Information("Get total purchase");
        var res = await _customerService.GetTotalPurchaseAsync();
        return ResponseExtension.Result(res);
    }
}