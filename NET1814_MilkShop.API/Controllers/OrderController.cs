using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NET1814_MilkShop.API.CoreHelpers.ActionFilters;
using NET1814_MilkShop.API.CoreHelpers.Extensions;
using NET1814_MilkShop.Repositories.Models.OrderModels;
using NET1814_MilkShop.Services.Services;
using ILogger = Serilog.ILogger;

namespace NET1814_MilkShop.API.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger _logger;

        public OrderController(IOrderService orderService, ILogger logger)
        {
            _orderService = orderService;
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
        [Route("/api/dashboard/orders/stats")]
        [Authorize(AuthenticationSchemes = "Access", Roles = "1,2")]
        public async Task<IActionResult> GetOrderStats([FromQuery] OrderStatsQueryModel queryModel)
        {
            _logger.Information("Get order stats");
            var response = await _orderService.GetOrderStatsAsync(queryModel);
            return ResponseExtension.Result(response);
        }
        [HttpPatch]
        [Route("/api/dashboard/orders/{id}/status")]
        [Authorize(AuthenticationSchemes = "Access", Roles = "1,2")]
        public async Task<IActionResult> UpdateOrderStatus(Guid id, [FromBody] OrderStatusModel model)
        {
            _logger.Information("Update order status");
            var response = await _orderService.UpdateOrderStatusAsync(id, model);
            return ResponseExtension.Result(response);
        }
    }
}