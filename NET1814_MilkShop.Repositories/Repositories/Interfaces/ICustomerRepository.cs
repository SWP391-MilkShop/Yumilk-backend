using NET1814_MilkShop.Repositories.Data.Entities;

namespace NET1814_MilkShop.Repositories.Repositories.Interfaces;

public interface ICustomerRepository
{
    /*Task<List<Customer>> GetCustomersAsync();*/
    /// <summary>
    /// Get default query
    /// </summary>
    /// <returns />
    IQueryable<Customer> GetCustomersQuery();

    Task<Customer?> GetByEmailAsync(string email);
    Task<Customer?> GetByIdAsync(Guid id);

    Task<bool> IsExistAsync(Guid id);

    /*Task<bool> IsCustomerExistAsync(string email, string phoneNumber);*/
    Task<bool> IsExistPhoneNumberAsync(string phoneNumber);
    Task<bool> IsExistEmailAsync(string email);
    void Add(Customer customer);
    void Update(Customer customer);
    void Remove(Customer customer);

    Task<CustomerAddress?> GetCustomerAddressById(int addressId);
    Task<string?> GetCustomerEmail(Guid userId);
}