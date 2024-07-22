using Microsoft.AspNetCore.Mvc;
using NET1814_MilkShop.API.CoreHelpers.Extensions;
using NET1814_MilkShop.Repositories.Models.UserModels;
using NET1814_MilkShop.Services.Services.Interfaces;
using ILogger = Serilog.ILogger;

namespace NET1814_MilkShop.API.Controllers;

[ApiController]
[Route("api/authentication")]
public class AuthenticationController : Controller
{
    private readonly ILogger _logger;
    private readonly IAuthenticationService _authenticationService;

    public AuthenticationController(
        ILogger logger,
        IWebHostEnvironment webHostEnvironment,
        IServiceProvider serviceProvider
    )
    {
        _logger = logger;
        _authenticationService = serviceProvider.GetRequiredService<IAuthenticationService>();
    }

    /// <summary>
    /// Validate username, số đt, email tồn tại
    /// <para>Nếu thành công thì lưu token vào db và gửi mail xác thực</para>
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("sign-up")]
    public async Task<IActionResult> SignUp([FromBody] SignUpModel model)
    {
        _logger.Information("Sign up");
        var response = await _authenticationService.SignUpAsync(model);
        return ResponseExtension.Result(response);
    }

    /// <summary>
    /// Nhận token qua query và xác thực với token lưu trong db
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    [HttpPost("verify")]
    public async Task<IActionResult> VerifyAccount([FromQuery] string token)
    {
        _logger.Information("Verify Account");
        var response = await _authenticationService.VerifyAccountAsync(token);
        return ResponseExtension.Result(response);
    }

    /// <summary>
    /// Only customer role can login, others will say wrong username or password.
    /// <para> Nếu bị ban thì chặn ko cho login</para>
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
    /// <para> Nếu bị ban thì chặn ko cho login</para>
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

    /// <summary>
    /// Validate nếu email tồn tại, lưu token vào db và gửi mail reset password
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordModel request)
    {
        _logger.Information("Forgot Password");
        var response = await _authenticationService.ForgotPasswordAsync(request);
        return ResponseExtension.Result(response);
    }

    /// <summary>
    /// Gửi token qua body kèm với password mới, nếu token đúng thì update password mới
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel request)
    {
        _logger.Information("Reset Password");
        var response = await _authenticationService.ResetPasswordAsync(request);
        return ResponseExtension.Result(response);
    }

    /// <summary>
    /// Gửi token qua query, nếu token đúng thì tạo access token và refresh token mới
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromQuery] string token)
    {
        _logger.Information("Refresh Token");
        var response = await _authenticationService.RefreshTokenAsync(token);
        return ResponseExtension.Result(response);
    }

    /// <summary>
    /// Gửi email qua query, nếu email tồn tại tạo token lưu db và gửi mail xác thực
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    [HttpPost("activate-account")]
    public async Task<IActionResult> ActivateAccount([FromQuery] string email)
    {
        _logger.Information("Activate Account");
        var response = await _authenticationService.ActivateAccountAsync(email);
        return ResponseExtension.Result(response);
    }

    /// <summary>
    /// Gửi token qua query, check token với project id tạo trên firebase
    /// <para>decode token và deserialize ra đc các info về user</para>
    /// <para>Nếu email_verified = false yêu cầu xác thực tài khoản google</para>
    /// <para>Nếu chưa có acc thì tự tạo và kích hoạt acc, gửi generated username + password</para>
    /// <para>Nếu có acc thì gán google_id nếu chưa có + kích hoạt nếu chưa kích hoạt</para>
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    [HttpPost("google-login")]
    public async Task<IActionResult> GoogleLogin([FromQuery] string token)
    {
        _logger.Information("Google Login");
        var response = await _authenticationService.GoogleLoginAsync(token);
        return ResponseExtension.Result(response);
    }
}