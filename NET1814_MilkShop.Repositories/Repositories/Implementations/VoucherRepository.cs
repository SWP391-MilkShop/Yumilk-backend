using Microsoft.EntityFrameworkCore;
using NET1814_MilkShop.Repositories.Data;
using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Repositories.Interfaces;

namespace NET1814_MilkShop.Repositories.Repositories.Implementations;

public class VoucherRepository : Repository<Voucher>, IVoucherRepository
{
    public VoucherRepository(AppDbContext context) : base(context)
    {
    }


    public async Task<Voucher?> GetByCodeAsync(string code)
    {
        return await _query.FirstOrDefaultAsync(x => x.Code == code);
    }

    public IQueryable<Voucher> GetVouchersQuery()
    {
        return _query;
    }

    public Task<bool> IsCodeExistAsync(string code)
    {
        return _query.AnyAsync(x => x.Code == code);
    }
}