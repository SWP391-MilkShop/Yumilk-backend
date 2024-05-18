using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NET1814_MilkShop.Services.Services
{
    public interface ICustomerService
    {
        Task<CustomerModel> GetByEmailAsync(string email);
    }
    public sealed class CustomerService : ICustomerService
    {
        private ICustomerRepository _customerRepository;
        public CustomerService(ICustomerRepository customerRepository)
        {

            _customerRepository = customerRepository;
        }

        public async Task<CustomerModel> GetByEmailAsync(string email)
        {
            var customer = await _customerRepository.GetByEmailAsync(email);
            var customerModel = new CustomerModel
            {
                CustomerId = customer.UserId.ToString(), //trong model là string, entity là uid,
                GoogleId = customer.GoogleId,
                PhoneNumber = customer.PhoneNumber,
                Email = customer.Email,
                Points = customer.Points,
                ProfilePictureUrl = customer.ProfilePictureUrl,
            };
            return customerModel;
        }
    }
}
