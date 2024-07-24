using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NET1814_MilkShop.API.CoreHelpers.Extensions;
using NET1814_MilkShop.Repositories.Models.VoucherModels;
using NET1814_MilkShop.Services.Services.Interfaces;
using ILogger = Serilog.ILogger;

namespace NET1814_MilkShop.API.Controllers;

[ApiController]
public class VoucherController : Controller
{
    private readonly ILogger _logger;
    private readonly IVoucherService _voucherService;

    public VoucherController(ILogger logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _voucherService = serviceProvider.GetRequiredService<IVoucherService>();
    }
    /// <summary>
    /// Get vouchers with filter, sort, paging and searchTerm will search by code and description
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpGet("api/vouchers")]
    // [Authorize(AuthenticationSchemes = "Access")]
    public async Task<IActionResult> GetVouchersAsync([FromQuery] VoucherQueryModel model)
    {
        _logger.Information("Get vouchers");
        var response = await _voucherService.GetVouchersAsync(model);
        return ResponseExtension.Result(response);
    }
    /// <summary>
    /// Get voucher by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("api/vouchers/{id}")]
    [Authorize(AuthenticationSchemes = "Access")]
    public async Task<IActionResult> GetVoucherByIdAsync(Guid id)
    {
        _logger.Information($"Get voucher {id}");
        var response = await _voucherService.GetByIdAsync(id);
        return ResponseExtension.Result(response);
    }
    
    /// <summary>
    /// Get voucher by code
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    [HttpGet("api/vouchers/code/{code}")]
    [Authorize(AuthenticationSchemes = "Access")]
    public async Task<IActionResult> GetVoucherByCodeAsync(string code)
    {
        _logger.Information($"Get voucher by code {code}");
        var response = await _voucherService.GetByCodeAsync(code);
        return ResponseExtension.Result(response);
    }

    /// <summary>
    /// Create voucher, voucher code will auto generated to random string that contain uppercase and number,
    /// when created voucher isActive will be set to false,
    /// all fields are required except description, start date can not greater than end date,
    /// percent must be an int type and percent must be from 5% to 50%
    /// min price condition and max price must be greater than or equal to 0
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPost("api/vouchers")]
    [Authorize(AuthenticationSchemes = "Access", Roles = "1,2")]
    public async Task<IActionResult> CreateVoucherAsync([FromBody] CreateVoucherModel model)
    {
        _logger.Information("Create voucher");
        var response = await _voucherService.CreateVoucherAsync(model);
        return ResponseExtension.Result(response);
    }
    /// <summary>
    /// Update voucher by id,
    /// If a field is not provided, that field will not be updated, percent must be from 5% to 50%,
    /// min price condition and max price must be greater than or equal to 0, start date can not greater than end date
    /// </summary>
    /// <param name="id"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpPatch("api/vouchers/{id}")]
    [Authorize(AuthenticationSchemes = "Access", Roles = "1,2")]
    public async Task<IActionResult> UpdateVoucherAsync(Guid id, [FromBody] UpdateVoucherModel model)
    {
        _logger.Information($"Update voucher {id}");
        var response = await _voucherService.UpdateVoucherAsync(id, model);
        return ResponseExtension.Result(response);
    }

    /// <summary>
    /// Soft delete voucher by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("api/vouchers/{id}")]
    [Authorize(AuthenticationSchemes = "Access", Roles = "1,2")]
    public async Task<IActionResult> DeleteVoucherAsync(Guid id)
    {
        _logger.Information($"Delete voucher {id}");
        var response = await _voucherService.DeleteVoucherAsync(id);
        return ResponseExtension.Result(response);
    }
}