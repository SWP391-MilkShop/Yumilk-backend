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

    [HttpGet("api/vouchers")]
    [Authorize(AuthenticationSchemes = "Access")]
    public async Task<IActionResult> GetVouchersAsync([FromQuery] VoucherQueryModel model)
    {
        _logger.Information("Get vouchers");
        var response = await _voucherService.GetVouchersAsync(model);
        return ResponseExtension.Result(response);
    }
    [HttpGet("api/vouchers/{id}")]
    [Authorize(AuthenticationSchemes = "Access")]
    public async Task<IActionResult> GetVoucherAsync(Guid id)
    {
        _logger.Information($"Get voucher {id}");
        var response = await _voucherService.GetByIdAsync(id);
        return ResponseExtension.Result(response);
    }
    [HttpGet("api/vouchers/code/{code}")]
    [Authorize(AuthenticationSchemes = "Access")]
    public async Task<IActionResult> GetVoucherByCodeAsync(string code)
    {
        _logger.Information($"Get voucher by code {code}");
        var response = await _voucherService.GetByCodeAsync(code);
        return ResponseExtension.Result(response);
    }
    [HttpPost("api/vouchers")]
    [Authorize(AuthenticationSchemes = "Access", Roles = "1,2")]
    public async Task<IActionResult> CreateVoucherAsync([FromBody] CreateVoucherModel model)
    {
        _logger.Information("Create voucher");
        var response = await _voucherService.CreateVoucherAsync(model);
        return ResponseExtension.Result(response);
    }
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