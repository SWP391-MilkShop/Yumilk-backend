using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NET1814_MilkShop.Repositories.Models.OrderModels;
using NET1814_MilkShop.Services.Services;
using ILogger = Serilog.ILogger;

namespace NET1814_MilkShop.API.Controllers;

[ApiController]
[Route("api/orders")]
public class OrderController : ControllerBase
{
    private readonly ILogger _logger;
    private readonly IOrderService _orderService;

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
        if (response.Status == "Error") return BadRequest(response);
        return Ok(response);
    }
}