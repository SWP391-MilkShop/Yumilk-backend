using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NET1814_MilkShop.Repositories.Models;
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
        public async Task<IActionResult> GetUsers()
        {
            _logger.Information("Get all users");
            var users = await _userService.GetUsersAsync();
            return Ok(users);
        }
        [HttpGet]
        [Route("api/customers")]
        [Authorize(AuthenticationSchemes = "Access", Roles = "1")]
        public async Task<IActionResult> GetCustomers()
        {
            _logger.Information("Get all customers");
            var customers = await _customerService.GetCustomersAsync();
            return Ok(customers);
        }
        [HttpGet]
        [Route("api/customers/{id}")]
        [Authorize(AuthenticationSchemes = "Access", Roles = "1")]
        public async Task<IActionResult> GetCustomerById(Guid id)
        {
            _logger.Information("Get customer by id");
            var customer = await _customerService.GetById(id);
            if (customer == null)
            {
                return NotFound("Customer not found");
            }
            return Ok(customer);
        }
        [HttpGet]
        [Route("api/user/me")]
        [Authorize(AuthenticationSchemes = "Access", Roles = "3")]
        public async Task<IActionResult> GetCurrentAuthUser()
        {
            _logger.Information("Get current user");
            var userId = User.Claims.Where(c => "UserId".Equals(c.Type)).FirstOrDefault()?.Value;
            if (userId == null)
            {
                return BadRequest("UserId not found");
            }
            var user = await _customerService.GetById(Guid.Parse(userId));
            if (user == null)
            {
                return NotFound("User not found");
            }
            return Ok(user);
        }
        [HttpPut]
        [Route("api/user/change-info")]
        [Authorize(AuthenticationSchemes = "Access", Roles = "3")]
        public async Task<IActionResult> ChangeInfo([FromBody] ChangeUserInfoModel model)
        {
            _logger.Information("Change user info");
            var userId = User.Claims.Where(c => "UserId".Equals(c.Type)).FirstOrDefault()?.Value;
            if (userId == null)
            {
                return BadRequest("UserId not found");
            }
            var user = await _customerService.GetById(Guid.Parse(userId));
            if (user == null)
            {
                return NotFound("User not found");
            }
            var response = await _customerService.ChangeInfoAsync(userId, model);
            if (response.Status == "Error")
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
