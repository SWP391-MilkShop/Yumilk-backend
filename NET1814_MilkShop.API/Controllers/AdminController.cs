using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NET1814_MilkShop.Repositories.Models.UserModels;
using NET1814_MilkShop.Services.Services;
using ILogger = Serilog.ILogger;
namespace NET1814_MilkShop.API.Controllers;

[ApiController]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly ILogger _logger;
    private readonly IAuthenticationService _authenticationService;

    public AdminController(ILogger logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _authenticationService = serviceProvider.GetRequiredService<IAuthenticationService>();
    }
    
    /// <summary>
    ///  Only Admin role can login, others will say wrong username or password.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login(RequestLoginModel model)
    {
        _logger.Information("Login");
        var response = await _authenticationService.AdminLoginAsync(model);
        if (response.Status == "Error")
        {
            return BadRequest(response);
        }
        return Ok(response);
    }
}