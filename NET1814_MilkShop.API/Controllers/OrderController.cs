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

        #region OrderHistory

        [HttpGet]
        [Route("/api/customer/orders")]
        [Authorize(AuthenticationSchemes = "Access", Roles = "3")]
        [ServiceFilter(typeof(UserExistsFilter))]
        public async Task<IActionResult> GetOrderHistory([FromQuery] OrderHistoryQueryModel model)
        {
            _logger.Information("Get order history");
            var userId = (HttpContext.Items["UserId"] as Guid?)!.Value;
            var res = await _orderService.GetOrderHistoryAsync(userId, model);
            return ResponseExtension.Result(res);
        }

        [HttpGet("/api/customer/orders/{id}")]
        [Authorize(AuthenticationSchemes = "Access", Roles = "3")]
        [ServiceFilter(typeof(UserExistsFilter))]
        public async Task<IActionResult> GetOrderHistoryDetail(Guid id)
        {
            _logger.Information("Get order history by id");
            var userId = (HttpContext.Items["UserId"] as Guid?)!.Value;
            var res = await _orderService.GetOrderHistoryDetailAsync(userId, id);
            return ResponseExtension.Result(res);
        }

        #endregion
    }
}