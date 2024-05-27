using Microsoft.AspNetCore.Mvc;

namespace NET1814_MilkShop.API.Controllers;

public class HomeController : ControllerBase
{
    private readonly IWebHostEnvironment _webHostEnvironment;

    public HomeController(IWebHostEnvironment webHostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment;
    }

    public IActionResult Index()
    {
        var info = $"Service is running normally on {_webHostEnvironment.EnvironmentName}";
        return Ok(info);
    }
}