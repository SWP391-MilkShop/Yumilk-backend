using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NET1814_MilkShop.API.CoreHelpers.Extensions;
using NET1814_MilkShop.Repositories.Models.OrderModels;
using NET1814_MilkShop.Repositories.Models.ProductModels;
using NET1814_MilkShop.Services.Services;
using ILogger = Serilog.ILogger;
namespace NET1814_MilkShop.API.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    public class DashboardController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly ILogger _logger;
        public DashboardController(IOrderService orderService, IProductService productService, ILogger logger)
        {
            _orderService = orderService;
            _productService = productService;
            _logger = logger;
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
        /// Get product stats
        /// Total number of products sold
        /// Total number of products sold per category
        /// Total number of products sold per brand
        /// (only count products that have been delivered)
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
    }
}
