using Microsoft.AspNetCore.Mvc;
using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Services.Services;
using Serilog;
using System.Net.WebSockets;
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

        [HttpGet("verify")]
        public async Task<IActionResult> VerifyAsync(string token)
        {
            _logger.Information("verify");
            var response = await _authenticationService.VerifyTokenAsync(token);
            if(response.Status == "Error")
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(RequestLoginModel model)
        {
            var res = await _authenticationService.LoginAsync(model);
            return Ok(res);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel request)
        {
            _logger.Information("ForgotPassword");
            var response = await _authenticationService.ForgotPasswordAsync(request);
            if (response.Status == "Error")
            {
                return BadRequest(response);
            }
            return Ok(response);    
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel request)
        {
            _logger.Information("ResetPassword");
            var response = await _authenticationService.RestPasswordAsync(request);
            if(response.Status == "Error")
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
