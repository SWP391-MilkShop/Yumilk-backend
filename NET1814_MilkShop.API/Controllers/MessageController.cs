using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NET1814_MilkShop.API.CoreHelpers.ActionFilters;
using NET1814_MilkShop.API.CoreHelpers.Extensions;
using NET1814_MilkShop.Repositories.Models.MessageModels;
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
            var res = await _messageService.CreateMessageAsync(senderId, model);
            return ResponseExtension.Result(res);
        }

        [HttpGet("thread/{otherUserId}")]
        [Authorize(AuthenticationSchemes = "Access")]
        [ServiceFilter(typeof(UserExistsFilter))]
        public async Task<IActionResult> GetMessageThread(Guid otherUserId)
        {
            _logger.Information("Get Message Thread");
            var senderId = (HttpContext.Items["UserId"] as Guid?)!.Value;
            var res = await _messageService.GetMessageThreadAsync(senderId, otherUserId);
            return ResponseExtension.Result(res);
        }

        [HttpDelete("{messageId}")]
        [Authorize(AuthenticationSchemes = "Access")]
        [ServiceFilter(typeof(UserExistsFilter))]
        public async Task<IActionResult> DeleteMessage(Guid messageId)
        {
            _logger.Information("Delete Message");
            var senderId = (HttpContext.Items["UserId"] as Guid?)!.Value;
            var res = await _messageService.DeleteMessageAsync(senderId, messageId);
            return ResponseExtension.Result(res);
        }
    }
}