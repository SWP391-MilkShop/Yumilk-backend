using Microsoft.AspNetCore.Mvc;
using SWP391_DEMO.Service;
namespace SWP391_DEMO.Controller
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpGet]
        [Route("api/[controller]/get-all-user")]
        public IActionResult GetAllUser()
        {
            var result = _userService.GetAllUser();
            return Ok(result);
        }
    }
}
