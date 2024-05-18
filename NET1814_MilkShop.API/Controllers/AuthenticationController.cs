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
        public async Task<IActionResult> CreateUser([FromBody] CreateUserModel model)
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
        public async Task<IActionResult> SignUp([FromBody] SignUpModel model)
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
        public async Task<IActionResult> VerifyAccount(string token)
        {
            _logger.Information("Verify Account");
            var response = await _authenticationService.VerifyAccountAsync(token);
            if (response.Status == "Error")
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(RequestLoginModel model)
        {
            _logger.Information("Login");
            var res = await _authenticationService.LoginAsync(model);
            if (res.Status == "Error")
            {
                return BadRequest(res);
            }
            return Ok(res);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordModel request)
        {
            _logger.Information("Forgot Password");
            var response = await _authenticationService.ForgotPasswordAsync(request);
            if (response.Status == "Error")
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel request)
        {
            _logger.Information("Reset Password");
            var response = await _authenticationService.ResetPasswordAsync(request);
            if (response.Status == "Error")
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
    }
}
