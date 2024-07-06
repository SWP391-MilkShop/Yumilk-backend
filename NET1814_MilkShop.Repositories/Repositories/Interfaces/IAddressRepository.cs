using NET1814_MilkShop.Repositories.Data.Entities;

namespace NET1814_MilkShop.Repositories.Repositories.Interfaces;

public interface IAddressRepository
{
    IQueryable<CustomerAddress> GetCustomerAddresses(Guid guid);
    Task<bool> ExistAnyAddress(Guid customerId);
    Task<CustomerAddress?> GetByIdAsync(int id);

    Task<CustomerAddress?> GetByDefault(Guid guid);
    void Add(CustomerAddress address);
    void Update(CustomerAddress address);
}