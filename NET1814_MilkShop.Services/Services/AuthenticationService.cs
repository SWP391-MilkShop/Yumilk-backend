using NET1814_MilkShop.Repositories.CoreHelpers.Constants;
using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Models.UserModels;
using NET1814_MilkShop.Repositories.Repositories;
using NET1814_MilkShop.Repositories.UnitOfWork;
using NET1814_MilkShop.Services.CoreHelpers.Extensions;
using System.IdentityModel.Tokens.Jwt;

namespace NET1814_MilkShop.Services.Services
{
    public interface IAuthenticationService
    {
        Task<ResponseModel> SignUpAsync(SignUpModel model, string environment);
        Task<ResponseModel> LoginAsync(RequestLoginModel model);
        Task<ResponseModel> VerifyAccountAsync(string token);
        Task<ResponseModel> ForgotPasswordAsync(ForgotPasswordModel request, string environment);
        Task<ResponseModel> ResetPasswordAsync(ResetPasswordModel request);
        Task<ResponseModel> RefreshTokenAsync(string token);
        Task<ResponseModel> ActivateAccountAsync(string email, string environment);
        Task<ResponseModel> DashBoardLoginAsync(RequestLoginModel model);
    }

    public sealed class AuthenticationService : IAuthenticationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IAuthenticationRepository _authenticationRepository;
        private readonly IEmailService _emailService;
        private readonly IJwtTokenExtension _jwtTokenExtension;

        public AuthenticationService(
            IUnitOfWork unitOfWork,
            IUserRepository userRepository,
            ICustomerRepository customerRepository,
            IAuthenticationRepository authenticationRepository,
            IEmailService emailService,
            IJwtTokenExtension jwtTokenExtension
        )
        {
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
            _customerRepository = customerRepository;
            _authenticationRepository = authenticationRepository;
            _emailService = emailService;
            _jwtTokenExtension = jwtTokenExtension;
        }

        

        /// <summary>
        /// Người dùng đăng ký tài khoản
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ResponseModel> SignUpAsync(SignUpModel model, string environment)
        {
            var existingUser = await _userRepository.GetByUsernameAsync(model.Username);
            if (existingUser != null)
            {
                return ResponseModel.BadRequest(ResponseConstants.Exist("Tên đăng nhập"));
            }

            /*var IsCustomerExist = await _customerRepository.IsCustomerExistAsync(model.Email, model.PhoneNumber);*/
            var isPhoneNumberExist = await _customerRepository.IsExistPhoneNumberAsync(model.PhoneNumber);
            if (isPhoneNumberExist)
            {
                return ResponseModel.BadRequest(ResponseConstants.Exist("Số điện thoại"));
            }

            var isEmailExist = await _customerRepository.IsExistEmailAsync(model.Email);
            if (isEmailExist)
            {
                return ResponseModel.BadRequest(ResponseConstants.Exist("Email"));
            }
            /*if (IsCustomerExist)
            {
                return new ResponseModel
                {
                    Status = "Error",
                    Message = "Email hoặc số điện thoại đã tồn tại trong hệ thống!"
                };
            }*/

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
                _emailService.SendVerificationEmail(model.Email, jwtVeriryToken, environment);
                return ResponseModel.Success(ResponseConstants.Register(true), null);
            }
            return ResponseModel.Error(ResponseConstants.Register(false));
        }

        public async Task<ResponseModel> LoginAsync(RequestLoginModel model)
        {
            var existingUser = await _authenticationRepository.GetUserByUserNameNPassword(
                model.Username,
                model.Password
            );
            if (existingUser != null && existingUser.RoleId == 3)
            //Only customer can login, others will say wrong username or password
            {
                //check if user is banned
                if (existingUser.IsBanned)
                {
                    return ResponseModel.BadRequest(ResponseConstants.Banned);
                }

                var token = _jwtTokenExtension.CreateJwtToken(existingUser, TokenType.Access);
                var refreshToken = _jwtTokenExtension.CreateJwtToken(
                    existingUser,
                    TokenType.Refresh
                );
                var responseLogin = new ResponseLoginModel
                {
                    UserID = existingUser.Id.ToString(),
                    Username = existingUser.Username,
                    FirstName = existingUser.FirstName,
                    LastName = existingUser.LastName,
                    RoleId = existingUser.RoleId,
                    AccessToken = token.ToString(),
                    RefreshToken = refreshToken.ToString(),
                    IsActive = existingUser.IsActive
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

                return ResponseModel.Success(ResponseConstants.Login(true), responseLogin);
            }
            return ResponseModel.BadRequest(ResponseConstants.Login(false));
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
                return ResponseModel.BadRequest(ResponseConstants.Expired("Token"));
            }
            if (isExist == null)
            {
                return ResponseModel.Success(ResponseConstants.NotFound("Người dùng"), null);
            }
            if (verifyToken.Equals(isExist.VerificationCode))
            {
                isExist.IsActive = true;
                isExist.VerificationCode = null;
                _userRepository.Update(isExist);
                var result = await _unitOfWork.SaveChangesAsync();
                if (result > 0)
                {
                    return ResponseModel.Success(ResponseConstants.Verify(true), null);
                }
                return ResponseModel.Error(ResponseConstants.Verify(false));
            }
            return ResponseModel.BadRequest(ResponseConstants.WrongCode);
        }

        public async Task<ResponseModel> ForgotPasswordAsync(ForgotPasswordModel request,
                                                                      string environment)
        {
            var customer = await _customerRepository.GetByEmailAsync(request.Email);
            if (customer != null)
            {
                string token = _jwtTokenExtension.CreateVerifyCode();
                customer.User.ResetPasswordCode = token;
                _userRepository.Update(customer.User);
                var result = await _unitOfWork.SaveChangesAsync();
                if (result > 0)
                {
                    var verifyToken = _jwtTokenExtension.CreateJwtToken(
                        customer.User,
                        TokenType.Reset
                    );
                    _emailService.SendPasswordResetEmail(customer.Email,
                        verifyToken, environment); //Có link token ở header nhưng phải tự nhập ở swagger để change pass
                    return ResponseModel.Success(ResponseConstants.ResetPasswordLink, null);
                }
            }
            return ResponseModel.Success(ResponseConstants.NotFound("Email"), null);
        }

        public async Task<ResponseModel> ResetPasswordAsync(ResetPasswordModel request)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(request.token);
            var tokenS = jsonToken as JwtSecurityToken;
            var userID = tokenS.Claims.First(claim => claim.Type == "UserId").Value;
            var verifyToken = tokenS.Claims.First(claim => claim.Type == "Token").Value;
            var isExist = await _userRepository.GetById(Guid.Parse(userID));
            if (isExist == null)
            {
                return ResponseModel.Success(ResponseConstants.NotFound("Người dùng"), null);
            }
            if (verifyToken.Equals(isExist.ResetPasswordCode))
            {
                isExist.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
                isExist.ResetPasswordCode = null;
                _userRepository.Update(isExist);
                var result = await _unitOfWork.SaveChangesAsync();
                if (result > 0)
                {
                    return ResponseModel.Success(ResponseConstants.ChangePassword(true), null);
                }
                return ResponseModel.Error(ResponseConstants.ChangePassword(false));
            }
            return ResponseModel.BadRequest(ResponseConstants.WrongCode);
        }
        public async Task<ResponseModel> RefreshTokenAsync(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token);
            var tokenS = jsonToken as JwtSecurityToken;
            var userId = tokenS.Claims.First(claim => claim.Type == "UserId").Value;
            var tokenType = tokenS.Claims.First(claim => claim.Type == "tokenType").Value;
            var exp = tokenS.Claims.First(claim => claim.Type == "exp").Value;
            var expirationTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(exp)).UtcDateTime;
            var userExisted = await _userRepository.GetById(Guid.Parse(userId));
            if (userExisted == null)
            {
                return ResponseModel.Success(ResponseConstants.NotFound("Người dùng"), null);
            }

            if (tokenType != TokenType.Refresh.ToString())
            {
                return ResponseModel.BadRequest(ResponseConstants.WrongFormat("Refresh token"));
            }

            if (expirationTime < DateTime.UtcNow)
            {
                return ResponseModel.BadRequest(ResponseConstants.Expired("Refresh token"));
            }
            var newToken = _jwtTokenExtension.CreateJwtToken(userExisted, TokenType.Access);
            return ResponseModel.Success(ResponseConstants.Create("Access Token", true), newToken);
        }

        public async Task<ResponseModel> ActivateAccountAsync(string email, string environment)
        {
            var customer = await _customerRepository.GetByEmailAsync(email);
            if (customer == null)
            {
                return ResponseModel.Success(ResponseConstants.NotFound("Email"), null);
            }
            if (!customer.User.IsActive)
            {
                string token = _jwtTokenExtension.CreateVerifyCode();
                customer.User.VerificationCode = token;
                _userRepository.Update(customer.User);
                var result = await _unitOfWork.SaveChangesAsync();
                if (result > 0)
                {
                    var verifyToken = _jwtTokenExtension.CreateJwtToken(
                        customer.User,
                        TokenType.Authentication
                    );
                    _emailService.SendVerificationEmail(customer.Email,
                        verifyToken, environment); //Có link token ở header nhưng phải tự nhập ở swagger để change pass
                    return ResponseModel.Success(ResponseConstants.ActivateAccountLink, null);
                }
            }
            return ResponseModel.BadRequest(ResponseConstants.AccountActivated);
        }

        public async Task<ResponseModel> DashBoardLoginAsync(RequestLoginModel model)
        {
            var existingUser = await _authenticationRepository.GetUserByUserNameNPassword(
                model.Username,
                model.Password
            );
            if (existingUser != null && existingUser.RoleId != 3)
            //Only admin,staff can login others will response wrong username or password
            {
                //check if user is banned
                if (existingUser.IsBanned)
                {
                    return ResponseModel.BadRequest(ResponseConstants.Banned);
                }

                var token = _jwtTokenExtension.CreateJwtToken(existingUser, TokenType.Access);
                var refreshToken = _jwtTokenExtension.CreateJwtToken(
                    existingUser,
                    TokenType.Refresh
                );
                var responseLogin = new ResponseLoginModel
                {
                    UserID = existingUser.Id.ToString(),
                    Username = existingUser.Username,
                    FirstName = existingUser.FirstName,
                    LastName = existingUser.LastName,
                    RoleId = existingUser.RoleId,
                    AccessToken = token.ToString(),
                    RefreshToken = refreshToken.ToString(),
                    IsActive = existingUser.IsActive
                };
                return ResponseModel.Success(ResponseConstants.Login(true), responseLogin);
            }
            return ResponseModel.BadRequest(ResponseConstants.Login(false));
        }
    }
}