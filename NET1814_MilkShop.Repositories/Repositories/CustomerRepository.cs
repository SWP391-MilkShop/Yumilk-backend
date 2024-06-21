using Microsoft.EntityFrameworkCore;
using NET1814_MilkShop.Repositories.Data;
using NET1814_MilkShop.Repositories.Data.Entities;

namespace NET1814_MilkShop.Repositories.Repositories
{
    public interface ICustomerRepository
    {
        /*Task<List<Customer>> GetCustomersAsync();*/
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

    public sealed class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        public CustomerRepository(AppDbContext context)
            : base(context)
        {
        }

        public async Task<Customer?> GetByEmailAsync(string email)
        {
            //use AsNoTracking for read-only operations
            //var customer = await _context
            //    .Customers.AsNoTracking()
            //    .Include(x => x.User)
            //    .FirstOrDefaultAsync(x => string.Equals(email, x.Email));
            var customer = await _query
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => string.Equals(email, x.Email));
            return customer;
        }

        public IQueryable<Customer> GetCustomersQuery()
        {
            //Them role de return role thay vi roleId
            var query = _query.Include(x => x.User).ThenInclude(x => x.Role);
            return query;
        }

        public override async Task<Customer?> GetByIdAsync(Guid id)
        {
            //return await _context
            //    .Customers.Include(x => x.User)
            //    .FirstOrDefaultAsync(x => x.UserId == id);
            return await _query.Include(x => x.User)
                .ThenInclude(o => o.Role)
                .FirstOrDefaultAsync(x => x.UserId == id);
        }

        public async Task<bool> IsExistAsync(Guid id)
        {
            //return await _context.Customers.AnyAsync(e => e.UserId == id);
            return await _query.AnyAsync(e => e.UserId == id);
        }

        // Tach PhoneNumber va Email de handle loi rieng tren frontend
        public async Task<bool> IsExistPhoneNumberAsync(string phoneNumber)
        {
            //return await _context.Customers.AnyAsync(e => e.PhoneNumber == phoneNumber);
            return await _query.AnyAsync(e => e.PhoneNumber == phoneNumber);
        }

        public async Task<bool> IsExistEmailAsync(string email)
        {
            //return await _context.Customers.AnyAsync(e => e.Email == email);
            return await _query.AnyAsync(e => e.Email == email);
        }

        public async Task<CustomerAddress?> GetCustomerAddressById(int addressId)
        {
            return await _context.CustomerAddresses.FirstOrDefaultAsync(x => x.Id == addressId);
        }

        public async Task<string?> GetCustomerEmail(Guid userId)
        {
            return await _context.Customers.Where(x => x.UserId == userId).Select(x => x.Email).FirstOrDefaultAsync();
        }
        /*public async Task<bool> IsCustomerExistAsync(string email, string phoneNumber)
        {
            return await _context.Customers.AnyAsync(e => e.Email == email || e.PhoneNumber == phoneNumber);
        }*/
    }
}