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
        private readonly IEmailService _emailService;

        public AuthenticationController(ILogger logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _authenticationService = serviceProvider.GetRequiredService<IAuthenticationService>();
            _emailService = serviceProvider.GetRequiredService<IEmailService>(); 
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
            var token = await _authenticationService.GetVerificationTokenAsync(model.Username);
            if (response.Status == "Error")
            {
                return BadRequest(response);
            }
            _emailService.SendVerificationEmail(model.Email,token);
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(RequestLoginModel model)
        {
            var res = await _authenticationService.LoginAsync(model);
            return Ok(res);
        }
    }
}
