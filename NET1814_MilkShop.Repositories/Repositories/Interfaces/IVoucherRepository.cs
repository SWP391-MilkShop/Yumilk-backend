using NET1814_MilkShop.Repositories.Data.Entities;

namespace NET1814_MilkShop.Repositories.Repositories.Interfaces;

public interface IVoucherRepository
{
    Task<Voucher?> GetByIdAsync(Guid id);
    Task<Voucher?> GetByCodeAsync(string code);
    IQueryable<Voucher> GetVouchersQuery();
    Task<bool> IsCodeExistAsync(string code);
    void Add(Voucher voucher);
    void Update(Voucher voucher);
    void Delete(Voucher voucher);
}