using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Models.VoucherModels;

namespace NET1814_MilkShop.Services.Services.Interfaces;

public interface IVoucherService
{
    Task<ResponseModel> GetVouchersAsync(VoucherQueryModel model);
    Task<ResponseModel> GetByIdAsync(Guid id);
    Task<ResponseModel> CreateVoucherAsync(CreateVoucherModel model);
    Task<ResponseModel> UpdateVoucherAsync(Guid id, UpdateVoucherModel model);
    Task<ResponseModel> DeleteVoucherAsync(Guid id);
    Task<ResponseModel> GetByCodeAsync(string code);
}