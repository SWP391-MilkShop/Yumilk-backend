using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NET1814_MilkShop.API.CoreHelpers.ActionFilters;
using NET1814_MilkShop.API.CoreHelpers.Extensions;
using NET1814_MilkShop.Repositories.CoreHelpers.Constants;
using NET1814_MilkShop.Repositories.Models.AddressModels;
using NET1814_MilkShop.Repositories.Models.UserModels;
using NET1814_MilkShop.Services.Services;
using ILogger = Serilog.ILogger;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NET1814_MilkShop.API.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IUserService _userService;
        private readonly ICustomerService _customerService;
        private readonly IAddressService _addressService;

        public UserController(ILogger logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _userService = serviceProvider.GetRequiredService<IUserService>();
            _customerService = serviceProvider.GetRequiredService<ICustomerService>();
            _addressService = serviceProvider.GetRequiredService<IAddressService>();
        }
        #region User
        [HttpGet]
        [Route("api/users")]
        [Authorize(AuthenticationSchemes = "Access", Roles = "1")]
        [ServiceFilter(typeof(UserExistsFilter))]
        public async Task<IActionResult> GetUsers([FromQuery] UserQueryModel request)
        {
            _logger.Information("Get all users");
            var response = await _userService.GetUsersAsync(request);
            /*if (response.Status == "Error")
            {
                return BadRequest(response);
            }
            return Ok(response);*/
            
            return ResponseExtension.Result(response);
        }
        #endregion

        #region  Customer
        [HttpGet]
        [Route("api/customers")]
        [Authorize(AuthenticationSchemes = "Access", Roles = "1")]
        [ServiceFilter(typeof(UserExistsFilter))]
        public async Task<IActionResult> GetCustomers([FromQuery] CustomerQueryModel request)
        {
            _logger.Information("Get all customers");
            var response = await _customerService.GetCustomersAsync(request);
            /*if (response.Status == "Error")
            {
                return BadRequest(response);
            }
            return Ok(response);*/
            return ResponseExtension.Result(response);
        }

        [HttpGet]
        [Route("api/customers/{id}")]
        [Authorize(AuthenticationSchemes = "Access", Roles = "1")]
        [ServiceFilter(typeof(UserExistsFilter))]
        public async Task<IActionResult> GetCustomerById(Guid id)
        {
            _logger.Information("Get customer by id");
            var response = await _customerService.GetByIdAsync(id);
            /*if (response.Status == "Error")
            {
                return BadRequest(response);
            }
            return Ok(response);*/
            return ResponseExtension.Result(response);
        }
        #endregion

        #region Account
        [HttpGet]
        /*[Route("api/user/me")]*/
        [Route("api/user/account/profile")]
        [Authorize(AuthenticationSchemes = "Access")]
        [ServiceFilter(typeof(UserExistsFilter))]
        public async Task<IActionResult> GetCurrentAuthUser()
        {
            _logger.Information("Get current user");
            var userId = (HttpContext.Items["UserId"] as Guid?)!.Value;
            var response = await _customerService.GetByIdAsync(userId);
            /*if (response.Status == "Error")
            {
                return BadRequest(response);
            }
            return Ok(response);*/
            return ResponseExtension.Result(response);
        }
        /// <summary>
        /// Only Customer can change profile info?
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPatch]
        /*[Route("api/user/account/change-info")]*/
        [Route("api/user/account/profile")]
        [Authorize(AuthenticationSchemes = "Access", Roles = "3")]
        [ServiceFilter(typeof(UserExistsFilter))]
        public async Task<IActionResult> ChangeInfo([FromBody] ChangeUserInfoModel model)
        {
            _logger.Information("Change user info");
            var userId = (HttpContext.Items["UserId"] as Guid?)!.Value;
            var response = await _customerService.ChangeInfoAsync(userId, model);
            /*if (response.Status == "Error")
            {
                return BadRequest(response);
            }
            return Ok(response);*/
            return ResponseExtension.Result(response);
        }

        [HttpPatch]
        /*[Route("api/user/change-password")]*/
        [Route("api/user/account/change-password")]
        [Authorize(AuthenticationSchemes = "Access")]
        [ServiceFilter(typeof(UserExistsFilter))]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            _logger.Information("Change user password");
            var userId = (HttpContext.Items["UserId"] as Guid?)!.Value;
            var response = await _userService.ChangePasswordAsync(userId, model);
            /*if (response.Status == "Error")
            {
                return BadRequest(response);
            }
            return Ok(response);*/
            return ResponseExtension.Result(response);
        }
        /// <summary>
        /// Feature only available for Customer role
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/user/account/addresses")]
        [Authorize(AuthenticationSchemes = "Access",Roles = "3")]
        [ServiceFilter(typeof(UserExistsFilter))]
        public async Task<IActionResult> GetCustomerAddresses()
        {
            _logger.Information("Get customer addresses");
            var userId = (HttpContext.Items["UserId"] as Guid?)!.Value;
            var response = await _addressService.GetAddressesByCustomerId(userId);
            /*if (response.Status == "Error")
            {
                return BadRequest(response);
            }
            return Ok(response);*/
            return ResponseExtension.Result(response);
        }
        
        /// <summary>
        /// Feature only available for Customer role,
        /// max 3 addresses and cannot set first address to non-default
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        /*[Route("api/user/change-password")]*/
        [Route("api/user/account/addresses")]
        [Authorize(AuthenticationSchemes = "Access",Roles = "3")]
        [ServiceFilter(typeof(UserExistsFilter))]
        public async Task<IActionResult> CreateCustomerAddress([FromBody] CreateAddressModel request)
        {
            _logger.Information("Create customer address");
            var customerId = (HttpContext.Items["UserId"] as Guid?)!.Value;
            var response = await _addressService.CreateAddressAsync(customerId, request);
            /*if (response.Status == "Error")
            {
                return BadRequest(response);
            }
            return Ok(response);*/
            return ResponseExtension.Result(response);
        }

        /// <summary>
        /// Feature only available for Customer role,
        /// if customer has only 1 address then cannot set it to non-default
        /// </summary>
        /// <returns></returns>
        [HttpPatch]
        [Route("api/user/account/addresses/{id}")]
        [Authorize(AuthenticationSchemes = "Access", Roles = "3")]
        [ServiceFilter(typeof(UserExistsFilter))]
        public async Task<IActionResult> UpdateCustomerAddress(
            int id,
            [FromBody] UpdateAddressModel request)
        {
            _logger.Information("Update Customer Address");
            var customerId = (HttpContext.Items["UserId"] as Guid?)!.Value;
            var response = await _addressService.UpdateAddressAsync(customerId,id,request);
            /*if (response.Status == "Error")
            {
                return BadRequest(response);
            }
            return Ok(response);*/
            return ResponseExtension.Result(response);
        }
        /// <summary>
        /// feature only available for Customer role,
        /// cannot delete with address that is default
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/user/account/addresses/{id}")]
        [Authorize(AuthenticationSchemes = "Access", Roles = "3")]
        [ServiceFilter(typeof(UserExistsFilter))]
        public async Task<IActionResult> DeleteCustomerAddress(int id)
        {
            _logger.Information("Delete Customer Address");
            var customerId = (HttpContext.Items["UserId"] as Guid?)!.Value;
            var response = await _addressService.DeleteAddressAsync(customerId, id);
            /*if (response.Status == "Error")
            {
                return BadRequest(response);
            }
            return Ok(response);*/
            return ResponseExtension.Result(response);
        }
        #endregion
    }
}
