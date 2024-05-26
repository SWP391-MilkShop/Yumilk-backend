using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NET1814_MilkShop.Repositories.Models.UserModels;
using NET1814_MilkShop.Services.Services;
using ILogger = Serilog.ILogger;

namespace NET1814_MilkShop.API.Controllers
{
    [ApiController]
    [Route("api/authentication")]
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
        [Authorize(AuthenticationSchemes = "Access", Roles = "1")]
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

        [HttpPost("verify")]
        public async Task<IActionResult> VerifyAccount([FromQuery] string token)
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

        /// <summary>
        ///  Only Admin role can login, others will say wrong username or password.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("admin/login")]
        public async Task<IActionResult> AdminLogin(RequestLoginModel model)
        {
            _logger.Information("Login");
            var response = await _authenticationService.AdminLoginAsync(model);
            if (response.Status == "Error")
            {
                return BadRequest(response);
            }
            return Ok(response);
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

        [HttpPost("refresh-token")]
        [Authorize(AuthenticationSchemes = "Refresh")]
        public async Task<IActionResult> RefreshToken()
        {
            _logger.Information("Refresh Token");
            if (!Request.Headers.TryGetValue("RefreshToken", out var token))
            {
                return BadRequest("Not found token");
            }

            var res = await _authenticationService.RefreshTokenAsync(token);
            if (res.Status == "Error")
            {
                return BadRequest(res);
            }

            return Ok(res);
        }
    }
}
