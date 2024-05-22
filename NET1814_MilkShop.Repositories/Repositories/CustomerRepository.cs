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
        Task<Customer?> GetById(Guid id);
        Task<bool> IsExistAsync(Guid id);
        void Add(Customer customer);
        void Update(Customer customer);
        void Remove(Customer customer);
    }

    public sealed class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        public CustomerRepository(AppDbContext context)
            : base(context) { }

        public async Task<Customer?> GetByEmailAsync(string email)
        {
            //use AsNoTracking for read-only operations
            return await _context
                .Customers.AsNoTracking()
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => email.Equals(x.Email));
        }

        /// <summary>
        /// Get all customers with user information included
        /// </summary>
        /// <returns></returns>
        /*public Task<List<Customer>> GetCustomersAsync()
        {
            return _context.Customers.Include(x => x.User).ToListAsync();
        }*/
        public IQueryable<Customer> GetCustomersQuery()
        {
            var query = _context.Customers.Include(u => u.User).AsNoTracking();
            return query;
        }

        public override async Task<Customer?> GetById(Guid id)
        {
            return await _context
                .Customers.Include(x => x.User)
                .FirstOrDefaultAsync(x => x.UserId == id);
        }

        public async Task<bool> IsExistAsync(Guid id)
        {
            return await _context.Customers.AnyAsync(e => e.UserId == id);
        }
    }
}
