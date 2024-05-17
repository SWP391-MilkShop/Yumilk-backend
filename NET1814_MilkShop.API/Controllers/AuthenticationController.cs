using Microsoft.AspNetCore.Mvc;
using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Services.Services;

namespace NET1814_MilkShop.API.Controllers
{
    [Route("/api/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost]
        public async Task<IActionResult> Login(RequestLoginModel model)
        {
            var res = await _authenticationService.LoginAsync(model);
            return Ok(res);
        }
    }
}
