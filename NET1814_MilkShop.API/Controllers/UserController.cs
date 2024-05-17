using Microsoft.AspNetCore.Mvc;
using NET1814_MilkShop.Services.Services;
using Serilog;
using ILogger = Serilog.ILogger;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NET1814_MilkShop.API.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IUserService _userService;

        public UserController(ILogger logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _userService = serviceProvider.GetRequiredService<IUserService>();
        }

        [HttpGet]
        public async Task<IActionResult> GetUsersAsync()
        {
            _logger.Information("Get all users");
            var users = await _userService.GetUsersAsync();
            return Ok(users);
        }

        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}
    }
}
