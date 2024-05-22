using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Models.UserModels;
using NET1814_MilkShop.Repositories.Repositories;
using NET1814_MilkShop.Repositories.UnitOfWork;

namespace NET1814_MilkShop.Services.Services
{
    public interface ICustomerService
    {
        Task<ResponseModel> GetCustomersAsync();
        Task<ResponseModel> GetByEmailAsync(string email);
        Task<ResponseModel> GetByIdAsync(Guid id);
        Task<ResponseModel> ChangeInfoAsync(Guid userId, ChangeUserInfoModel changeUserInfoModel);
        Task<bool> IsExistAsync(Guid id);
    }
    public sealed class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CustomerService(ICustomerRepository customerRepository, IUnitOfWork unitOfWork)
        {
            _customerRepository = customerRepository;
            _unitOfWork = unitOfWork;
        }
        private static CustomerModel ToCustomerModel(Customer customer, User user)
        {
            return new CustomerModel
            {
                UserID = customer.UserId.ToString(),
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                GoogleId = customer.GoogleId,
                PhoneNumber = customer.PhoneNumber,
                Email = customer.Email,
                Points = customer.Points,
                RoleId = user.RoleId,
                ProfilePictureUrl = customer.ProfilePictureUrl,
            };
        }
        public async Task<ResponseModel> GetByEmailAsync(string email)
        {
            var customer = await _customerRepository.GetByEmailAsync(email);
            if (customer == null)
            {
                return new ResponseModel
                {
                    Data = null,
                    Message = "Customer not found",
                    Status = "Error"
                };
            }
            var customerModel = ToCustomerModel(customer, customer.User);
            return new ResponseModel
            {
                Data = customerModel,
                Message = "Get customer by email successfully",
                Status = "Success"
            };
        }

        public async Task<ResponseModel> GetCustomersAsync()
        {
            var customers = await _customerRepository.GetCustomersAsync();
            var list = new List<CustomerModel>();
            foreach (var customer in customers)
            {
                list.Add(ToCustomerModel(customer, customer.User));
            }
            return new ResponseModel
            {
                Data = list,
                Message = "Get all customers successfully",
                Status = "Success"
            };
        }

        public async Task<ResponseModel> GetByIdAsync(Guid id)
        {
            var customer = await _customerRepository.GetById(id);
            if (customer == null)
            {
                return new ResponseModel
                {
                    Data = null,
                    Message = "Customer not found",
                    Status = "Error"
                };
            }
            var customerModel = ToCustomerModel(customer, customer.User);
            return new ResponseModel
            {
                Data = customerModel,
                Message = "Get customer by id successfully",
                Status = "Success"
            };
        }

        public async Task<ResponseModel> ChangeInfoAsync(Guid userId, ChangeUserInfoModel changeUserInfoModel)
        {
            var customer = await _customerRepository.GetById(userId);
            if (customer == null)
            {
                return new ResponseModel
                {
                    Data = null,
                    Message = "Customer not found",
                    Status = "Error"
                };
            }
            //Only change the info that is not null or whitespace
            customer.User.Username = !string.IsNullOrWhiteSpace(changeUserInfoModel.Username) ? changeUserInfoModel.Username : customer.User.Username;
            customer.User.FirstName = !string.IsNullOrWhiteSpace(changeUserInfoModel.FirstName) ? changeUserInfoModel.FirstName : customer.User.FirstName;
            customer.User.LastName = !string.IsNullOrWhiteSpace(changeUserInfoModel.LastName) ? changeUserInfoModel.LastName : customer.User.LastName;
            customer.Email = !string.IsNullOrWhiteSpace(changeUserInfoModel.Email) ? changeUserInfoModel.Email : customer.Email;
            customer.PhoneNumber = !string.IsNullOrWhiteSpace(changeUserInfoModel.PhoneNumber) ? changeUserInfoModel.PhoneNumber : customer.PhoneNumber;
            customer.ProfilePictureUrl = !string.IsNullOrWhiteSpace(changeUserInfoModel.ProfilePictureUrl) ? changeUserInfoModel.ProfilePictureUrl : customer.ProfilePictureUrl;
            _customerRepository.Update(customer);
            var result = await _unitOfWork.SaveChangesAsync();
            if (result > 0)
            {
                return new ResponseModel
                {
                    Data = ToCustomerModel(customer, customer.User),
                    Message = "Change user info successfully",
                    Status = "Success"
                };
            }
            return new ResponseModel
            {
                Message = "Change user info failed",
                Status = "Error"
            };
        }

        public async Task<bool> IsExistAsync(Guid id)
        {
            return await _customerRepository.IsExistAsync(id);
        }
    }
}
