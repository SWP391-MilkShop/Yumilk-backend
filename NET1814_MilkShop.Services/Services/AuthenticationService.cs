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
        Task<ResponseModel> LoginAsync(RequestLoginModel model);
        Task<ResponseModel> VerifyAccountAsync(string token);
        Task<ResponseModel> ForgotPasswordAsync(ForgotPasswordModel request);
        Task<ResponseModel> ResetPasswordAsync(ResetPasswordModel request);
    }

    public sealed class AuthenticationService : IAuthenticationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly string Key = "qwertyuiopasdfghjklzxcvbnmasdasdasdasdasdasdasdas";
        private readonly IAuthenticationRepository _authenticationRepository;
        private readonly IEmailService _emailService;

        public AuthenticationService(IServiceProvider serviceProvider)
        {
            _unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
            _userRepository = serviceProvider.GetRequiredService<IUserRepository>();
            _customerRepository = serviceProvider.GetRequiredService<ICustomerRepository>();
            _authenticationRepository = serviceProvider.GetRequiredService<IAuthenticationRepository>();
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
            string token = CreateVerifyToken();
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = model.Username,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                RoleId = 1,
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
            var jwtVeriryToken = CreateVerifyJwtToken(user, token);
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
        private string CreateVerifyJwtToken(User isUserExisted, string userToken)
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
                    new Claim("Token", userToken)
                }),
                Expires = DateTime.UtcNow.AddSeconds(5),
                SigningCredentials = credential,
            };
            var token = tokenHandler.CreateToken(tokenDescription);
            return tokenHandler.WriteToken(token);
        }

        public async Task<ResponseModel> LoginAsync(RequestLoginModel model)
        {
            var existingUser = await _authenticationRepository.GetUserByUserNameNPassword(model.UserName, model.Password);
            if (existingUser != null)
            {
                var token = CreateJwtToken(existingUser);
                var refreshToken = CreateJwtRefreshToken(existingUser);
                return new ResponseModel
                {
                    Status = "Success",
                    Message = "Đăng nhập thành công",
                    Data = token,
                };
            }
            return new ResponseModel
            {
                Status = "Error",
                Message = "Tên đăng nhập hoặc mật khẩu sai "
            };
        }

        private string CreateJwtRefreshToken(User isUserExisted)
        {
            var randomByte = new Byte[64];
            var token = Convert.ToBase64String(randomByte);
            //var refreshToken = new RefreshToken
            //{
            //    Id = new Random().Next(0, 10000000),
            //    Token = token,
            //    Expires = DateTime.UtcNow.AddDays(3),
            //    UserId = isUserExisted.Id,
            //    CreatedAt = DateTime.UtcNow,
            //    DeletedAt = DateTime.UtcNow.AddDays(3),
            //};
            //_refreshTokenRepository.Add(refreshToken);
            return token;
        }
        /// <summary>
        /// Create JWT Token
        /// </summary>
        /// <param name="isUserExisted"></param>
        /// <returns></returns>
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


        public async Task<ResponseModel> VerifyAccountAsync(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token);
            var tokenS = jsonToken as JwtSecurityToken;
            var userID = tokenS.Claims.First(claim => claim.Type == "UserID").Value;
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
                string token = CreateVerifyToken();
                user.ResetPasswordCode = token;
                _userRepository.Update(user);
                var result = await _unitOfWork.SaveChangesAsync();
                if (result > 0)
                {
                    var verifyToken = CreateVerifyJwtToken(user, token);
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
            var userID = tokenS.Claims.First(claim => claim.Type == "UserID").Value;
            var verifyToken = tokenS.Claims.First(claim => claim.Type == "Token").Value;
            var exp = tokenS.Claims.First(claim => claim.Type == "exp").Value;
            var expirationTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(exp)).UtcDateTime;
            var isExist = await _userRepository.GetById(Guid.Parse(userID));
            if (expirationTime < DateTime.UtcNow)
            {
                return new ResponseModel { Status = "Error", Message = "Hết hạn" };
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
        /// <summary>
        /// Tạo mã xác thực ngẫu nhiên 6 ký tự
        /// </summary>
        /// <returns></returns>
        private static string CreateVerifyToken()
        {
            Random res = new Random();
            string str = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            int size = 6;
            string token = "";
            for (int i = 0; i < size; i++)
            {
                // Chon index ngau nhien tren str
                int x = res.Next(str.Length);
                token = token + str[x];
            }
            return token;
        }
    }
}
