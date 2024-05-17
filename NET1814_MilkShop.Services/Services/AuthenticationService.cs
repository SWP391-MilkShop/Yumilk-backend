using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Repositories;
using NET1814_MilkShop.Repositories.UnitOfWork;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NET1814_MilkShop.Services.Services
{
    public interface IAuthenticationService
    {
        Task<ResponseModel> SignUpAsync(SignUpModel model);
        Task<ResponseModel> CreateUserAsync(CreateUserModel model);
        Task<ResponseLoginModel> LoginAsync(RequestLoginModel model);
    }

    public sealed class AuthenticationService : IAuthenticationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly string Key = "qwertyuiopasdfghjklzxcvbnmasdasdasdasdasdasdasdas";
        private readonly IAuthenticationRepository _authenticationRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public AuthenticationService(IServiceProvider serviceProvider)
        {
            _unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
            _userRepository = serviceProvider.GetRequiredService<IUserRepository>();
            _customerRepository = serviceProvider.GetRequiredService<ICustomerRepository>();
            _authenticationRepository = serviceProvider.GetRequiredService<IAuthenticationRepository>();
            _refreshTokenRepository = serviceProvider.GetRequiredService<IRefreshTokenRepository>();
        }

        /// <summary>
        /// Admin có thể tạo tài khoản cho nhân viên hoặc admin khác
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ResponseModel> CreateUserAsync(CreateUserModel model)
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
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = model.Username,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                RoleId = model.RoleId,
                IsActive = true, //no activation required
                CreatedAt = DateTime.UtcNow
            };
            _userRepository.Add(user);
            var result = await _unitOfWork.SaveChangesAsync();
            if (result > 0)
            {
                return new ResponseModel
                {
                    Status = "Success",
                    Message = "Tạo tài khoản thành công!"
                };
            }
            return new ResponseModel { Status = "Error", Message = "Tạo tài khoản thất bại" };
        }

        /// <summary>
        /// Người dùng đăng ký tài khoản
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
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
                return new ResponseModel { Status = "Error", Message = "Email đã tồn tại!" };
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
                    Message =
                        "Đăng ký tài khoản thành công, vui lòng kiểm tra email để xác thực tài khoản!"
                };
            }
            return new ResponseModel { Status = "Error", Message = "Đăng ký tài khoản thất bại" };
        }
        public async Task<ResponseLoginModel> LoginAsync(RequestLoginModel model)
        {
            var isUserExisted = await _authenticationRepository.GetUserByUserNameNPassword(model.UserName, model.Password);
            if (isUserExisted != null)
            {
                var token = CreateJwtToken(isUserExisted);
                var refreshToken = CreateJwtRefreshToken(isUserExisted);
                return new ResponseLoginModel
                {
                    FirstName = isUserExisted.FirstName,
                    LastName = isUserExisted.LastName,
                    Token = token,
                    RefreshToken = refreshToken,
                    Message = "Đăng nhập thành công"
                };
            }
            else
            {
                return new ResponseLoginModel
                {
                    Message = "Tên đăng nhập hoặc mật khẩu sai " + BCrypt.Net.BCrypt.HashPassword("string")
                };
            }
        }

        private string CreateJwtRefreshToken(User isUserExisted)
        {
            var randomByte = new Byte[64];
            var token = Convert.ToBase64String(randomByte);
            var refreshToken = new RefreshToken
            {
                Id = new Random().Next(0, 10000000),
                Token = token,
                Expires = DateTime.UtcNow.AddDays(3),
                UserId = isUserExisted.Id,
                CreatedAt = DateTime.UtcNow,
                DeletedAt = DateTime.UtcNow.AddDays(3),
            };
            _refreshTokenRepository.Add(refreshToken);
            return token;
        }

        private string CreateJwtToken(User isUserExisted)
        {
            var key = Encoding.UTF8.GetBytes(Key);
            var securityKey = new SymmetricSecurityKey(key);
            var credential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescription = new SecurityTokenDescriptor
            {
                Audience = "",
                Issuer = "",
                Subject = new ClaimsIdentity(new[] {
                    new Claim("UserID", isUserExisted.Id.ToString()),
                    new Claim(ClaimTypes.Role, isUserExisted.RoleId.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(5),
                SigningCredentials = credential,
            };
            var token = tokenHandler.CreateToken(tokenDescription);
            return tokenHandler.WriteToken(token);
        }
    }
}
