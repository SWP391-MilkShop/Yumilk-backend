using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NET1814_MilkShop.API.CoreHelpers.ActionFilters;
using NET1814_MilkShop.API.CoreHelpers.Extensions;
using NET1814_MilkShop.Repositories.Models.CartModels;
using NET1814_MilkShop.Services.Services.Interfaces;
using ILogger = Serilog.ILogger;

namespace NET1814_MilkShop.API.Controllers;

[ApiController]
public class CartController : Controller
{
    private readonly ILogger _logger;
    private readonly ICartService _cartService;

    public CartController(ILogger logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _cartService = serviceProvider.GetRequiredService<ICartService>();
    }

    /// <summary>
    /// Chặn nếu tài khoản chưa kích hoạt hoặc bị khóa
    /// <para>Có thể search theo name, sort theo price</para>
    /// <para>Apply voucher (nếu có) -> apply point (nếu có)</para>
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpGet]
    [Authorize(AuthenticationSchemes = "Access", Roles = "3")]
    [ServiceFilter(typeof(UserExistsFilter))]
    [Route("api/user/{userId}/cart")]
    public async Task<IActionResult> GetCartAsync(Guid userId, [FromQuery] CartQueryModel model)
    {
        var current = (HttpContext.Items["UserId"] as Guid?)!.Value;
        _logger.Information($"Get cart of customer {userId}");
        if (current != userId)
        {
            return Unauthorized();
        }

        var response = await _cartService.GetCartAsync(userId, model);
        return ResponseExtension.Result(response);
    }

    /// <summary>
    /// Chặn các sp chưa active, out of stock, preorder
    /// <para>Chặn số lượng mua vượt quá số lượng còn lại</para>
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize(AuthenticationSchemes = "Access", Roles = "3")]
    [ServiceFilter(typeof(UserExistsFilter))]
    [Route("api/user/{userId}/cart")]
    public async Task<IActionResult> AddToCartAsync(
        Guid userId,
        [FromBody] AddToCartModel model
    )
    {
        var current = (HttpContext.Items["UserId"] as Guid?)!.Value;
        if (current != userId)
        {
            return Unauthorized();
        }

        _logger.Information($"Add product {model.ProductId} to cart of customer {userId}");
        var response = await _cartService.AddToCartAsync(userId, model);
        return ResponseExtension.Result(response);
    }

    /// <summary>
    /// Cập nhật giỏ hàng, loại bỏ các sp không hợp lệ, preorder, out of stock
    /// <para>Nếu số lượng tồn ít hơn, cập nhật số lượng mua</para>
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpPut]
    [Authorize(AuthenticationSchemes = "Access", Roles = "3")]
    [ServiceFilter(typeof(UserExistsFilter))]
    [Route("api/user/{userId}/cart")]
    public async Task<IActionResult> UpdateCartAsync(Guid userId)
    {
        var current = (HttpContext.Items["UserId"] as Guid?)!.Value;
        if (current != userId)
        {
            return Unauthorized();
        }

        _logger.Information($"Update cart of customer {userId} with new product data");
        var response = await _cartService.UpdateCartAsync(userId);
        return ResponseExtension.Result(response);
    }

    /// <summary>
    /// Hard delete all items in cart
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpDelete]
    [Authorize(AuthenticationSchemes = "Access", Roles = "3")]
    [Route("api/user/{userId}/cart")]
    public async Task<IActionResult> ClearCartAsync(Guid userId)
    {
        var current = (HttpContext.Items["UserId"] as Guid?)!.Value;
        if (current != userId)
        {
            return Unauthorized();
        }

        _logger.Information($"Clear cart of customer {userId}");
        var response = await _cartService.ClearCartAsync(userId);
        return ResponseExtension.Result(response);
    }
    /// <summary>
    /// Chặn nếu vượt quá số lượng tồn
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="productId"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPatch]
    [Authorize(AuthenticationSchemes = "Access", Roles = "3")]
    [ServiceFilter(typeof(UserExistsFilter))]
    [Route("api/user/{userId}/cart/{productId}")]
    public async Task<IActionResult> UpdateCartItemAsync(
        Guid userId,
        Guid productId,
        [FromBody] UpdateCartItemModel model
    )
    {
        var current = (HttpContext.Items["UserId"] as Guid?)!.Value;
        if (current != userId)
        {
            return Unauthorized();
        }

        _logger.Information($"Update product {productId} in cart of customer {userId}");
        var response = await _cartService.UpdateCartItemAsync(userId, productId, model);
        return ResponseExtension.Result(response);
    }

    /// <summary>
    /// Hard delete item from cart
    /// <para>Chặn nếu không tồn tại item trong cart</para>
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="productId"></param>
    /// <returns></returns>
    [HttpDelete]
    [Authorize(AuthenticationSchemes = "Access", Roles = "3")]
    [ServiceFilter(typeof(UserExistsFilter))]
    [Route("api/user/{userId}/cart/{productId}")]
    public async Task<IActionResult> DeleteCartItemAsync(Guid userId, Guid productId)
    {
        var current = (HttpContext.Items["UserId"] as Guid?)!.Value;
        if (current != userId)
        {
            return Unauthorized();
        }

        _logger.Information($"Delete product {productId} from cart of customer {userId}");
        var response = await _cartService.DeleteCartItemAsync(userId, productId);
        return ResponseExtension.Result(response);
    }
}