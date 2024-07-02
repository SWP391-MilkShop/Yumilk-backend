using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NET1814_MilkShop.API.CoreHelpers.ActionFilters;
using NET1814_MilkShop.API.CoreHelpers.Extensions;
using NET1814_MilkShop.Repositories.Models.MessageModels;
using NET1814_MilkShop.Repositories.Repositories;
using NET1814_MilkShop.Services.Services;
using ILogger = Serilog.ILogger;

namespace NET1814_MilkShop.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly ILogger _logger;

        public MessageController(IMessageService messageService, ILogger logger)
        {
            _messageService = messageService;
            _logger = logger;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "Access")]
        [ServiceFilter(typeof(UserExistsFilter))]
        public async Task<IActionResult> CreateMessage([FromBody] CreateMessageModel model)
        {
            _logger.Information("Create Message");
            var senderId = (HttpContext.Items["UserId"] as Guid?)!.Value;
            var res = await _messageService.CreateMessage(senderId, model);
            return ResponseExtension.Result(res);
        }
    }
}