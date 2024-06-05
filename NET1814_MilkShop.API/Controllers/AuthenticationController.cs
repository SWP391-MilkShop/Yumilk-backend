using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NET1814_MilkShop.API.CoreHelpers.Extensions;
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
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AuthenticationController(
            ILogger logger,
            IWebHostEnvironment webHostEnvironment,
            IServiceProvider serviceProvider
        )
        {
            _logger = logger;
            _authenticationService = serviceProvider.GetRequiredService<IAuthenticationService>();
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpPost("sign-up")]
        public async Task<IActionResult> SignUp([FromBody] SignUpModel model)
        {
            _logger.Information("Sign up");
            var environment = _webHostEnvironment.EnvironmentName;
            var response = await _authenticationService.SignUpAsync(model, environment);
            return ResponseExtension.Result(response);
        }

        [HttpPost("verify")]
        public async Task<IActionResult> VerifyAccount([FromQuery] string token)
        {
            _logger.Information("Verify Account");
            var response = await _authenticationService.VerifyAccountAsync(token);
            return ResponseExtension.Result(response);
        }

        /// <summary>
        /// Only customer role can login, others will say wrong username or password.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login(RequestLoginModel model)
        {
            _logger.Information("Login");
            var res = await _authenticationService.LoginAsync(model);
            return ResponseExtension.Result(res);
        }

        /// <summary>
        ///  Only Admin,Staff role can login, others will say wrong username or password.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("dashboard/login")]
        public async Task<IActionResult> AdminLogin(RequestLoginModel model)
        {
            _logger.Information("Login");
            var response = await _authenticationService.DashBoardLoginAsync(model);
            return ResponseExtension.Result(response);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordModel request)
        {
            _logger.Information("Forgot Password");
            var environment = _webHostEnvironment.EnvironmentName;
            var response = await _authenticationService.ForgotPasswordAsync(request, environment);
            return ResponseExtension.Result(response);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel request)
        {
            _logger.Information("Reset Password");
            var response = await _authenticationService.ResetPasswordAsync(request);
            return ResponseExtension.Result(response);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromQuery] string token)
        {
            _logger.Information("Refresh Token");
            var response = await _authenticationService.RefreshTokenAsync(token);
            return ResponseExtension.Result(response);
        }

        [HttpPost("activate-account")]
        public async Task<IActionResult> ActivateAccount([FromBody] string email)
        {
            _logger.Information("Activate Account");
            var environment = _webHostEnvironment.EnvironmentName;
            var response = await _authenticationService.ActivateAccountAsync(email, environment);
            return ResponseExtension.Result(response);
        }
    }
}
