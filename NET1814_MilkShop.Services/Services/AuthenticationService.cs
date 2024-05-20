using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Repositories;
using NET1814_MilkShop.Repositories.UnitOfWork;
using NET1814_MilkShop.Services.CoreHelpers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace NET1814_MilkShop.Services.Services
{
    public interface IAuthenticationService
    {
        Task<ResponseModel> SignUpAsync(SignUpModel model);
        Task<ResponseModel> CreateUserAsync(CreateUserModel model);
        Task<ResponseModel> LoginAsync(RequestLoginModel model);
        Task<ResponseModel> VerifyAccountAsync(string token);
        Task<ResponseModel> ForgotPasswordAsync(ForgotPasswordModel request);
        Task<ResponseModel> ResetPasswordAsync(ResetPasswordModel request);
        Task<ResponseModel> RefreshTokenAsync(string token);
    }

    public sealed class AuthenticationService : IAuthenticationService
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IAuthenticationRepository _authenticationRepository;
        private readonly IEmailService _emailService;
        private readonly IJwtTokenExtension _jwtTokenExtension;
        public AuthenticationService(IServiceProvider serviceProvider)
        {
            _configuration = serviceProvider.GetRequiredService<IConfiguration>();
            _unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
            _userRepository = serviceProvider.GetRequiredService<IUserRepository>();
            _customerRepository = serviceProvider.GetRequiredService<ICustomerRepository>();
            _authenticationRepository = serviceProvider.GetRequiredService<IAuthenticationRepository>();
            _emailService = serviceProvider.GetRequiredService<IEmailService>();
            _jwtTokenExtension = serviceProvider.GetRequiredService<IJwtTokenExtension>();

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
            string token = _jwtTokenExtension.CreateVerifyCode();
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = model.Username,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                RoleId = 3,
                VerificationCode = token,
                IsActive = false,
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
            var result = await _unitOfWork.SaveChangesAsync();
            var jwtVeriryToken = _jwtTokenExtension.CreateJwtToken(user, TokenType.Authentication);
            if (result > 0)
            {
                _emailService.SendVerificationEmail(model.Email, jwtVeriryToken);
                return new ResponseModel
                {
                    Status = "Success",
                    Message =
                        "Đăng ký tài khoản thành công, vui lòng kiểm tra email để xác thực tài khoản!"
                };
            }
            return new ResponseModel { Status = "Error", Message = "Đăng ký tài khoản thất bại" };
        }

        public async Task<ResponseModel> LoginAsync(RequestLoginModel model)
        {
            var existingUser = await _authenticationRepository.GetUserByUserNameNPassword(model.UserName, model.Password);
            if (existingUser != null)
            {
                var token = _jwtTokenExtension.CreateJwtToken(existingUser, TokenType.Access);
                var refreshToken = _jwtTokenExtension.CreateJwtToken(existingUser, TokenType.Refresh);
                var responseLogin = new ResponseLoginModel
                {
                    UserID = existingUser.Id.ToString(),
                    Username = existingUser.Username,
                    FirstName = existingUser.FirstName,
                    LastName = existingUser.LastName,
                    RoleId = existingUser.RoleId.ToString(),
                    AccessToken = token.ToString(),
                    RefreshToken = refreshToken.ToString()
                };
                var customer = await _customerRepository.GetById(existingUser.Id);
                if (customer != null)
                {
                    responseLogin.Email = customer.Email;
                    responseLogin.PhoneNumber = customer.PhoneNumber;
                    responseLogin.ProfilePictureUrl = customer.ProfilePictureUrl;
                    responseLogin.GoogleId = customer.GoogleId;
                    responseLogin.Points = customer.Points;
                }
                return new ResponseModel
                {
                    Status = "Success",
                    Message = "Đăng nhập thành công",
                    Data = responseLogin
                };
            }
            return new ResponseModel
            {
                Status = "Error",
                Message = "Tên đăng nhập hoặc mật khẩu sai "
            };
        }

        public async Task<ResponseModel> VerifyAccountAsync(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token);
            var tokenS = jsonToken as JwtSecurityToken;
            var userID = tokenS.Claims.First(claim => claim.Type == "UserId").Value;
            var verifyToken = tokenS.Claims.First(claim => claim.Type == "Token").Value;
            var exp = tokenS.Claims.First(claim => claim.Type == "exp").Value;
            var expirationTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(exp)).UtcDateTime;
            var isExist = await _userRepository.GetById(Guid.Parse(userID));
            if (expirationTime < DateTime.UtcNow)
            {
                return new ResponseModel { Status = "Error", Message = "Hết hạn" };
            }
            if (isExist != null && verifyToken.Equals(isExist.VerificationCode))
            {
                isExist.IsActive = true;
                isExist.VerificationCode = null;
                _userRepository.Update(isExist);
                var result = await _unitOfWork.SaveChangesAsync();
                if (result > 0)
                {
                    return new ResponseModel { Status = "Success", Message = "Xác thực tài khoản thành công" };
                }
            }
            return new ResponseModel { Status = "Error", Message = "Có lỗi xảy ra trong quá trình xác thực hoặc link đã được dùng rồi" };
        }

        public async Task<ResponseModel> ForgotPasswordAsync(ForgotPasswordModel request)
        {
            var customer = await _customerRepository.GetByEmailAsync(request.Email);
            if (customer != null)
            {
                var user = await _userRepository.GetById(customer.UserId);
                string token = _jwtTokenExtension.CreateVerifyCode();
                user.ResetPasswordCode = token;
                _userRepository.Update(user);
                var result = await _unitOfWork.SaveChangesAsync();
                if (result > 0)
                {
                    var verifyToken = _jwtTokenExtension.CreateJwtToken(user, TokenType.Authentication);
                    _emailService.SendPasswordResetEmail(customer.Email, verifyToken);//Có link token ở header nhưng phải tự nhập ở swagger để change pass
                    return new ResponseModel { Status = "Success", Message = "Đã gửi link reset password vui lòng kiểm tra email" };
                }
            }
            return new ResponseModel { Status = "Error", Message = "Email không tồn tại" };
        }

        public async Task<ResponseModel> ResetPasswordAsync(ResetPasswordModel request)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(request.token);
            var tokenS = jsonToken as JwtSecurityToken;
            var userID = tokenS.Claims.First(claim => claim.Type == "UserId").Value;
            var verifyToken = tokenS.Claims.First(claim => claim.Type == "Token").Value;
            var exp = tokenS.Claims.First(claim => claim.Type == "exp").Value;
            var expirationTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(exp)).UtcDateTime;
            var isExist = await _userRepository.GetById(Guid.Parse(userID));
            if (expirationTime < DateTime.UtcNow)
            {
                return new ResponseModel
                {
                    Status = "Error",
                    Message = "Token hết hạn"
                };
            }
            if (isExist != null && verifyToken.Equals(isExist.ResetPasswordCode))
            {
                isExist.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
                isExist.ResetPasswordCode = null;
                _userRepository.Update(isExist);
                var result = await _unitOfWork.SaveChangesAsync();
                if (result > 0)
                {
                    return new ResponseModel
                    {
                        Status = "Success",
                        Message = "Đổi mật khẩu thành công"
                    };
                }
            }
            return new ResponseModel() { Status = "Error", Message = "Token không hợp lệ" };
        }


        public async Task<ResponseModel> RefreshTokenAsync(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token);
            var tokenS = jsonToken as JwtSecurityToken;
            var userId = tokenS.Claims.First(claim => claim.Type == "UserId").Value;
            var exp = tokenS.Claims.First(claim => claim.Type == "exp").Value;
            var expirationTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(exp)).UtcDateTime;
            var userExisted = await _userRepository.GetById(Guid.Parse(userId));
            if (userExisted == null)
            {
                return new ResponseModel
                {
                    Status = "Error",
                    Message = "Không tồn tại người dùng"
                };
            }
            if (expirationTime < DateTime.UtcNow)
            {
                return new ResponseModel
                {
                    Status = "Error",
                    Message = "Token hết hạn"
                };
            }
            var newToken = _jwtTokenExtension.CreateJwtToken(userExisted, TokenType.Access);
            return new ResponseModel
            {
                Status = "Success",
                Message = "Tạo access token mới thành công",
                Data = newToken.ToString()
            };
        }
    }
}
