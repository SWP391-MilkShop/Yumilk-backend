using Microsoft.AspNetCore.Mvc;
using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Services.Services;
using Serilog;
using ILogger = Serilog.ILogger;
namespace NET1814_MilkShop.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : Controller
    {
        private readonly ILogger _logger;
        private readonly IAuthenticationService _authenticationService;
        public AuthenticationController(ILogger logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _authenticationService = serviceProvider.GetRequiredService<IAuthenticationService>();
        }
        [HttpPost("create-user")]
        public async Task<IActionResult> CreateUserAsync([FromBody] CreateUserModel model)
        {
            _logger.Information("Create user");
            var response = await _authenticationService.CreateUserAsync(model);

            if (response.Status == "Error")
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("sign-up")]
        public async Task<IActionResult> SignUpAsync([FromBody] SignUpModel model)
        {
            _logger.Information("Sign up");
            var response = await _authenticationService.SignUpAsync(model);

            if (response.Status == "Error")
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
