using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Models.UserModels;
using NET1814_MilkShop.Repositories.Repositories;
using NET1814_MilkShop.Repositories.UnitOfWork;
using NET1814_MilkShop.Services.CoreHelpers;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace NET1814_MilkShop.Services.Services
{
    public interface ICustomerService
    {
        /*Task<ResponseModel> GetCustomersAsync();*/
        Task<ResponseModel> GetCustomersAsync(CustomerQueryModel request);
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
                ProfilePictureUrl = customer.ProfilePictureUrl,
            };
        }

        public async Task<ResponseModel> GetCustomersAsync(CustomerQueryModel request)
        {
            var query = _customerRepository.GetCustomersQuery();
            //filter
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                query = query.Where(c =>
                    c.User.Username.Contains(request.SearchTerm)
                    || c.Email!.Contains(request.SearchTerm)
                    || c.PhoneNumber!.Contains(request.SearchTerm)
                    || c.User.FirstName!.Contains(request.SearchTerm)
                    || c.User.LastName!.Contains(request.SearchTerm)
                );
            }
            if (request.IsActive != null)
            {
                query = query.Where(c => c.User.IsActive == request.IsActive);
            }
            //sort
            query = "desc".Equals(request.SortOrder?.ToLower())
                ? query.OrderByDescending(GetSortProperty(request))
                : query.OrderBy(GetSortProperty(request));
            var result = query.Select(c => new CustomerModel()
            {
                UserID = c.UserId.ToString(),
                FirstName = c.User.FirstName,
                LastName = c.User.LastName,
                IsActive = c.User.IsActive,
                Email = c.Email,
                Points = c.Points,
                Username = c.User.Username,
                PhoneNumber = c.PhoneNumber,
                ProfilePictureUrl = c.ProfilePictureUrl,
                GoogleId = c.GoogleId
            });
            var customers = await PagedList<CustomerModel>.CreateAsync(
                result,
                request.Page,
                request.PageSize
            );
            return new ResponseModel()
            {
                Data = customers,
                Message =
                    customers.TotalCount > 0 ? "Get customers successfully" : "No customers found",
                Status = "success"
            };
        }

        private static Expression<Func<Customer, object>> GetSortProperty(
            CustomerQueryModel request
        )
        {
            Expression<Func<Customer, object>> keySelector = request.SortColumn?.ToLower() switch
            {
                "point" => customer => customer.Points,
                "email" => customer => customer.Email!,
                "isActive" => customer => customer.User.IsActive,
                "firstName" => customer => customer.User.FirstName!,
                "lastName" => customer => customer.User.LastName!,
                _ => customer => customer.UserId
            };
            return keySelector;
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

        /*public async Task<ResponseModel> GetCustomersAsync()
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
        }*/

        public async Task<ResponseModel> GetByIdAsync(Guid id)
        {
            var customer = await _customerRepository.GetById(id);
            if (customer == null)
            {
                return new ResponseModel
                {
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
                    Message = "Customer not found",
                    Status = "Error"
                };
            }

            if (!string.IsNullOrWhiteSpace(changeUserInfoModel.PhoneNumber))
            {
                if (!Regex.IsMatch(changeUserInfoModel.PhoneNumber, @"^([0-9]{10})$"))
                {
                    return new ResponseModel
                    {
                        Message = "Invalid Phone Number!",
                        Status = "Error"
                    };
                }
                customer.PhoneNumber = changeUserInfoModel.PhoneNumber;
            }

            if (!string.IsNullOrWhiteSpace(changeUserInfoModel.ProfilePictureUrl))
            {
                if (!Uri.IsWellFormedUriString(changeUserInfoModel.ProfilePictureUrl, UriKind.Absolute))
                {
                    return new ResponseModel
                    {
                        Message = "Invalid URL!",
                        Status = "Error"
                    };
                }
                customer.ProfilePictureUrl = changeUserInfoModel.ProfilePictureUrl;
            }

            if (!string.IsNullOrWhiteSpace(changeUserInfoModel.FirstName))
            {
                customer.User.FirstName = changeUserInfoModel.FirstName;
            }

            if (!string.IsNullOrWhiteSpace(changeUserInfoModel.LastName))
            {
                customer.User.LastName = changeUserInfoModel.LastName;
            }

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
            return new ResponseModel { Message = "Change user info failed", Status = "Error" };
        }

        public async Task<bool> IsExistAsync(Guid id)
        {
            return await _customerRepository.IsExistAsync(id);
        }
    }
}
