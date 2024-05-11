using Microsoft.AspNetCore.Mvc;
using SWP391_DEMO.Service;
namespace SWP391_DEMO.Controller
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;
        public UserController(IServiceProvider serviceProvider, ILogger<UserController> logger)
        {
            _userService = serviceProvider.GetRequiredService<IUserService>();
            _logger = logger;
        }
        [HttpGet]
        [Route("api/[controller]/")]
        public IActionResult GetAllUser()
        {
            _logger.LogInformation("Get all user");
            var result = _userService.GetAllUser();
            return Ok(result);
        }
        [HttpGet]
        [Route("api/[controller]/{id}")]
        public IActionResult GetUserById(Guid id)
        {
            _logger.LogInformation("Get user by Id");
            var result = _userService.GetUserById(id);
            return Ok(result);
        }
    }
}
