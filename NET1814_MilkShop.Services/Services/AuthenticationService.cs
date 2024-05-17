using Microsoft.Extensions.DependencyInjection;
using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Repositories;
using NET1814_MilkShop.Repositories.UnitOfWork;

namespace NET1814_MilkShop.Services.Services
{
    public interface IAuthenticationService
    {
        Task<ResponseModel> SignUpAsync(SignUpModel model);
    }
    public sealed class AuthenticationService : IAuthenticationService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;
        private readonly ICustomerRepository _customerRepository;
        public AuthenticationService(IServiceProvider serviceProvider)
        {
            _unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
            _userRepository = serviceProvider.GetRequiredService<IUserRepository>();
            _customerRepository = serviceProvider.GetRequiredService<ICustomerRepository>();
        }
        public async Task<ResponseModel> SignUpAsync(SignUpModel model)
        {
            var existingUser = await _userRepository.GetByUsernameAsync(model.Username);
            if (existingUser != null)
            {
                return new ResponseModel
                {
                    Status = "Error",
                    Message = "Tên đăng nhập đã tồn tại!"
                };
            }
            var existingCustomer = await _customerRepository.GetByEmailAsync(model.Email);
            if (existingCustomer != null)
            {
                return new ResponseModel
                {
                    Status = "Error",
                    Message = "Email đã tồn tại!"
                };
            }
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = model.Username,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                RoleId = 1,
                IsActive = false,
                CreatedAt = DateTime.UtcNow
            };
            var customer = new Customer
            {
                UserId = user.Id,
                Points = 0,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber
            };
            _userRepository.Add(user);
            _customerRepository.Add(customer);
            var result = await _unitOfWork.SaveChangesAsync();
            if (result > 0)
            {
                return new ResponseModel
                {
                    Status = "Success",
                    Message = "Đăng ký tài khoản thành công, vui lòng kiểm tra email để xác thực tài khoản!"
                };
            }
            return new ResponseModel
            {
                Status = "Error",
                Message = "Đăng ký tài khoản thất bại"
            };
        }
    }
}
