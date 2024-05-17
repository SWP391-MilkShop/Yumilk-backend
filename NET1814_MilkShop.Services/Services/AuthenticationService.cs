using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Repositories;
using NET1814_MilkShop.Repositories.UnitOfWork;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NET1814_MilkShop.Services.Services
{
    public interface IAuthenticationService
    {
        Task<ResponseModel> SignUpAsync(SignUpModel model);
        Task<ResponseModel> CreateUserAsync(CreateUserModel model);
        Task<ResponseLoginModel> LoginAsync(RequestLoginModel model);
        Task<string> GetVerificationTokenAsync(string userName);
        Task<ResponseModel> VerifyTokenAsync(string token);
        Task<ResponseModel> ForgotPasswordAsync(ForgotPasswordModel request);
        Task<ResponseModel> RestPasswordAsync(ResetPasswordModel request);
    }

    public sealed class AuthenticationService : IAuthenticationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly string Key = "qwertyuiopasdfghjklzxcvbnmasdasdasdasdasdasdasdas";
        private readonly IAuthenticationRepository _authenticationRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IEmailService _emailService;

        public AuthenticationService(IServiceProvider serviceProvider)
        {
            _unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
            _userRepository = serviceProvider.GetRequiredService<IUserRepository>();
            _customerRepository = serviceProvider.GetRequiredService<ICustomerRepository>();
            _authenticationRepository = serviceProvider.GetRequiredService<IAuthenticationRepository>();
            _refreshTokenRepository = serviceProvider.GetRequiredService<IRefreshTokenRepository>();
            _emailService = serviceProvider.GetRequiredService<IEmailService>();
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
            Random res = new Random();
            string str = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            int size = 32;
            string token = "";
            for (int i = 0; i < size; i++)
            {
                // Chon index ngau nhien tren str
                int x = res.Next(str.Length);
                token = token + str[x];
            }
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = model.Username,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                RoleId = 1,
                VerificationToken = token,
                IsActive = false,
                CreatedAt = DateTime.UtcNow,
                DeletedAt = DateTime.UtcNow.AddHours(2)
            };
            var customer = new Customer
            {
                UserId = user.Id,
                Points = 0,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber
            };
            _userRepository.Add(user);
            _customerRepository.Add(customer); // Khong nen add vao customer khi chua verify
            _emailService.SendVerificationEmail(model.Email,token);
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

        public async Task<string> GetVerificationTokenAsync(string userName)
        {
            var verifyToken = await _userRepository.GetVerificationTokenAsync(userName);
            if(verifyToken is null)
            {
                throw new ArgumentException();
            }
            return verifyToken;
        }

        public async Task<ResponseModel> VerifyTokenAsync(string token)
        {
            var user = await _userRepository.VerifyTokenAsync(token);
            if(user is not null)
            {
                /*                var customer = new Customer
                                {
                                    UserId = user.Id,
                                    Points = 0,
                                    Email = user.email,
                                    PhoneNumber = user.PhoneNumber
             };*/// Lẽ ra khúc này add customer nhưng lại không lấy được mail và phoneNumber
            //tại khi lưu user, user không có email và phonenumber -> không lấy lại user và add vô customer được 
                return new ResponseModel { Status = "Success", Message = "Xác thực tài khoản thành công" };
            }
            return new ResponseModel {Status = "Error", Message = "Có lỗi xảy ra trong quá trình xác thực hoặc link đã được dùng rồi" };
        }

        public async Task<ResponseModel> ForgotPasswordAsync(ForgotPasswordModel request)
        {
            var customer = await _customerRepository.GetByEmailAsync(request.Email);
            if(customer is not null)
            {
                var allUser = await _userRepository.GetUsersAsync();
                var user = allUser.Where(x => x.Id == customer.UserId).FirstOrDefault();
                Random res = new Random();
                string str = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                int size = 32;
                string token = "";
                for (int i = 0; i < size; i++)
                {
                    // Chon index ngau nhien tren str
                    int x = res.Next(str.Length);
                    token = token + str[x];
                }
                user.VerificationToken = token;
                _userRepository.Update(user);
                _emailService.SendPasswordResetEmail(customer.Email, token);//Có link token ở header nhưng phải tự nhập ở swagger để change pass
                await _unitOfWork.SaveChangesAsync();
                return new ResponseModel { Status = "Success", Message = "Đã gửi link reset password vui lòng check mail" };
            }
            return new ResponseModel { Status = "Error", Message = "Mail không tồn tại" };
        }

        public async Task<ResponseModel> RestPasswordAsync(ResetPasswordModel request)
        {
            var user = await _userRepository.GetUsersAsync();
            var validTokenUser = user.FirstOrDefault(x => x.VerificationToken == request.token);
            if (validTokenUser != null)
            {
                validTokenUser.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
                validTokenUser.VerificationToken = null;
                _userRepository.Update(validTokenUser);
                await _unitOfWork.SaveChangesAsync();
                return new ResponseModel
                {
                    Status = "Success",
                    Message = "Successfully change password"
                };
            }
            return new ResponseModel() { Status = "Error", Message = "Token invalid" };
        }
    }
}
