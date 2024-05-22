using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NET1814_MilkShop.API.CoreHelpers.ActionFilters;
using NET1814_MilkShop.Repositories.Models.UserModels;
using NET1814_MilkShop.Services.Services;
using ILogger = Serilog.ILogger;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NET1814_MilkShop.API.Controllers
{

    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IUserService _userService;
        private readonly ICustomerService _customerService;

        public UserController(ILogger logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _userService = serviceProvider.GetRequiredService<IUserService>();
            _customerService = serviceProvider.GetRequiredService<ICustomerService>();
        }

        [HttpGet]
        [Route("api/users")]
        [Authorize(AuthenticationSchemes = "Access", Roles = "1")]
        [ServiceFilter(typeof(UserExistsFilter))]
        public async Task<IActionResult> GetUsers()
        {
            _logger.Information("Get all users");
            var response = await _userService.GetUsersAsync();
            if (response.Status == "Error")
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        [HttpGet]
        [Route("api/customers")]
        [Authorize(AuthenticationSchemes = "Access", Roles = "1")]
        [ServiceFilter(typeof(UserExistsFilter))]
        public async Task<IActionResult> GetCustomers()
        {
            _logger.Information("Get all customers");
            var response = await _customerService.GetCustomersAsync();
            if (response.Status == "Error")
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        [HttpGet]
        [Route("api/customers/{id}")]
        [Authorize(AuthenticationSchemes = "Access", Roles = "1")]
        [ServiceFilter(typeof(UserExistsFilter))]
        public async Task<IActionResult> GetCustomerById(Guid id)
        {
            _logger.Information("Get customer by id");
            var response = await _customerService.GetByIdAsync(id);
            if (response.Status == "Error")
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        [HttpGet]
        [Route("api/user/me")]
        [Authorize(AuthenticationSchemes = "Access", Roles = "3")]
        [ServiceFilter(typeof(UserExistsFilter))]
        public async Task<IActionResult> GetCurrentAuthUser()
        {
            _logger.Information("Get current user");
            var userId = (HttpContext.Items["UserId"] as Guid?)!.Value;
            var response = await _customerService.GetByIdAsync(userId);
            if (response.Status == "Error")
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        [HttpPut]
        [Route("api/user/change-info")]
        [Authorize(AuthenticationSchemes = "Access", Roles = "3")]
        [ServiceFilter(typeof(UserExistsFilter))]
        public async Task<IActionResult> ChangeInfo([FromBody] ChangeUserInfoModel model)
        {
            _logger.Information("Change user info");
            var userId = (HttpContext.Items["UserId"] as Guid?)!.Value;
            var response = await _customerService.ChangeInfoAsync(userId, model);
            if (response.Status == "Error")
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        [HttpPut]
        [Route("api/user/change-password")]
        [Authorize(AuthenticationSchemes = "Access", Roles = "3")]
        [ServiceFilter(typeof(UserExistsFilter))]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            _logger.Information("Change user password");
            var userId = (HttpContext.Items["UserId"] as Guid?)!.Value;
            var response = await _userService.ChangePasswordAsync(userId, model);
            if (response.Status == "Error")
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
