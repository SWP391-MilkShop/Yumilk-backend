using NET1814_MilkShop.Repositories.CoreHelpers.Constants;
using NET1814_MilkShop.Repositories.CoreHelpers.Enum;
using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Models.UserModels;
using NET1814_MilkShop.Repositories.Repositories;
using NET1814_MilkShop.Repositories.UnitOfWork;
using NET1814_MilkShop.Services.CoreHelpers.Extensions;
using System.IdentityModel.Tokens.Jwt;
using FirebaseAdmin.Auth;
using Microsoft.IdentityModel.Tokens;
using NET1814_MilkShop.Repositories.Models.GoogleAuthenticationModels;
using Newtonsoft.Json;

namespace NET1814_MilkShop.Services.Services
{
    public interface IAuthenticationService
    {
        Task<ResponseModel> SignUpAsync(SignUpModel model);
        Task<ResponseModel> LoginAsync(RequestLoginModel model);
        Task<ResponseModel> VerifyAccountAsync(string token);
        Task<ResponseModel> ForgotPasswordAsync(ForgotPasswordModel request);
        Task<ResponseModel> ResetPasswordAsync(ResetPasswordModel request);
        Task<ResponseModel> RefreshTokenAsync(string token);
        Task<ResponseModel> ActivateAccountAsync(string email);
        Task<ResponseModel> DashBoardLoginAsync(RequestLoginModel model);
        Task<ResponseModel> GoogleLoginAsync(string token);
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
        public async Task<ResponseModel> SignUpAsync(SignUpModel model)
        {
            var existingUser = await _userRepository.GetByUsernameAsync(model.Username);
            if (existingUser != null)
            {
                return ResponseModel.BadRequest(ResponseConstants.Exist("Tên đăng nhập"));
            }

            /*var IsCustomerExist = await _customerRepository.IsCustomerExistAsync(model.Email, model.PhoneNumber);*/
            var isPhoneNumberExist = await _customerRepository.IsExistPhoneNumberAsync(
                model.PhoneNumber
            );
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
                RoleId = (int)RoleId.CUSTOMER,
                VerificationCode = token,
                IsActive = false,
            };
            var customer = new Customer
            {
                UserId = user.Id,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber
            };
            _userRepository.Add(user);
            _customerRepository.Add(customer); // Khong nen add vao customer khi chua verify
            var result = await _unitOfWork.SaveChangesAsync();
            var jwtVeriryToken = _jwtTokenExtension.CreateJwtToken(user, TokenType.Authentication);
            if (result > 0)
            {
                await _emailService.SendVerificationEmailAsync(model.Email, jwtVeriryToken, model.FirstName);
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
            if (existingUser != null && existingUser.RoleId == (int)RoleId.CUSTOMER)
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
                    Role = existingUser.Role!.Name,
                    AccessToken = token,
                    RefreshToken = refreshToken,
                    IsActive = existingUser.IsActive,
                    IsBanned = existingUser.IsBanned
                };
                var customer = await _customerRepository.GetByIdAsync(existingUser.Id);
                if (customer != null)
                {
                    responseLogin.Email = customer.Email;
                    responseLogin.PhoneNumber = customer.PhoneNumber;
                    responseLogin.ProfilePictureUrl = customer.ProfilePictureUrl;
                    responseLogin.GoogleId = customer.GoogleId;
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
            var userId = tokenS.Claims.First(claim => claim.Type == "UserId").Value;
            var verifyToken = tokenS.Claims.First(claim => claim.Type == "Token").Value;
            var exp = tokenS.Claims.First(claim => claim.Type == "exp").Value;
            var expirationTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(exp)).UtcDateTime;
            var isExist = await _userRepository.GetByIdAsync(Guid.Parse(userId));
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

        public async Task<ResponseModel> ForgotPasswordAsync(
            ForgotPasswordModel request
        )
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
                    await _emailService.SendPasswordResetEmailAsync(customer.Email, verifyToken,
                        customer.User.FirstName); //Có link token ở header nhưng phải tự nhập ở swagger để change pass
                    return ResponseModel.Success(ResponseConstants.ResetPasswordLink, null);
                }
            }

            return ResponseModel.BadRequest(ResponseConstants.NotFound("Email"));
        }

        public async Task<ResponseModel> ResetPasswordAsync(ResetPasswordModel request)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(request.token);
            var tokenS = jsonToken as JwtSecurityToken;
            var userId = tokenS.Claims.First(claim => claim.Type == "UserId").Value;
            var verifyToken = tokenS.Claims.First(claim => claim.Type == "Token").Value;
            var isExist = await _userRepository.GetByIdAsync(Guid.Parse(userId));
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
            var userExisted = await _userRepository.GetByIdAsync(Guid.Parse(userId));
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

        public async Task<ResponseModel> ActivateAccountAsync(string email)
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
                    await _emailService.SendVerificationEmailAsync(customer.Email, verifyToken,
                        customer.User.FirstName); //Có link token ở header nhưng phải tự nhập ở swagger để change pass
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
            if (existingUser != null && existingUser.RoleId != (int)RoleId.CUSTOMER)
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
                    Role = existingUser.Role!.Name,
                    AccessToken = token,
                    RefreshToken = refreshToken,
                    IsBanned = existingUser.IsBanned,
                    IsActive = existingUser.IsActive
                };
                return ResponseModel.Success(ResponseConstants.Login(true), responseLogin);
            }

            return ResponseModel.BadRequest(ResponseConstants.Login(false));
        }

        public async Task<ResponseModel> GoogleLoginAsync(string token)
        {
            FirebaseToken decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
            if (decodedToken == null)
            {
                return ResponseModel.BadRequest("Đã xảy ra lỗi trong quá trình xác thực tài khoản Google");
            }

            var firebase = JsonConvert.DeserializeObject<FirebaseModel>(decodedToken.Claims["firebase"].ToString()!);
            var googleId = firebase!.Identities.GoogleId[0];
            var isVerifyEmail = decodedToken.Claims["email_verified"].ToString();
            // kiểm tra email đã đc google verify ?
            if (isVerifyEmail == "False")
            {
                return ResponseModel.BadRequest("Tài khoản Google chưa được xác thực email");
            }

            User tempUser = null!;
            var userEmail = decodedToken.Claims["email"].ToString();
            var isExistUser = await _customerRepository.GetByEmailAsync(userEmail!);
            if (isExistUser == null) // nếu chưa tồn tại người dùng thì tự động đăng ký
            {
                var userFullName = decodedToken.Claims["name"].ToString()!.Trim();
                var userFirstName = userFullName.Contains(' ')
                    ? userFullName.Substring(0, userFullName.IndexOf(' ')).Trim()
                    : "";
                var userLastName = userFullName.Contains(' ')
                    ? userFullName.Substring(userFullName.IndexOf(' ')).Trim()
                    : userFullName.Trim();
                var username = Guid.NewGuid().ToString();
                //check trùng username
                while (await _userRepository.GetByUsernameAsync(username) != null)
                {
                    username = Guid.NewGuid().ToString();
                }

                var password = Guid.NewGuid().ToString();
                // tạo tài khoản mới
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Username = username,
                    Password = BCrypt.Net.BCrypt.HashPassword(password),
                    FirstName = userFirstName,
                    LastName = userLastName,
                    RoleId = (int)RoleId.CUSTOMER,
                    IsActive = true
                };

                var pictureUrl = decodedToken.Claims["picture"].ToString(); // lấy ảnh đại diện từ google
                var customerGoogle = new Customer
                {
                    UserId = user.Id,
                    Email = userEmail,
                    GoogleId = googleId,
                    ProfilePictureUrl = pictureUrl
                };
                tempUser = user;
                _userRepository.Add(user);
                _customerRepository.Add(customerGoogle);
                // gửi email username và password cho người dùng
                await _emailService.SendGoogleAccountAsync(userEmail!, userFullName, username, password);
            }
            else // đã tồn tại gmail trong hệ thống
            {
                tempUser = isExistUser.User;
                if (isExistUser.User.IsBanned)
                {
                    return ResponseModel.BadRequest(ResponseConstants.Banned);
                }

                // kiểm tra có ggid chưa ?
                if (string.IsNullOrEmpty(isExistUser.GoogleId))
                {
                    // kiểm tra isactive
                    if (!isExistUser.User.IsActive)
                    {
                        isExistUser.User.IsActive = true;
                        await _emailService.SendActiveEmailAsync(isExistUser.Email!,
                            isExistUser.User.FirstName + " " + isExistUser.User.LastName);
                    }

                    isExistUser.GoogleId = googleId;
                    _userRepository.Update(isExistUser.User);
                }
                else if (isExistUser.GoogleId != googleId)
                {
                    return ResponseModel.BadRequest("Tài khoản Google đã được liên kết với một tài khoản khác");
                }
            }


            await _unitOfWork.SaveChangesAsync();
            var jwtToken = _jwtTokenExtension.CreateJwtToken(tempUser, TokenType.Access);
            var refreshToken = _jwtTokenExtension.CreateJwtToken(
                tempUser,
                TokenType.Refresh
            );
            var responseLogin = new ResponseLoginModel
            {
                UserID = tempUser.Id.ToString(),
                Username = tempUser.Username,
                FirstName = tempUser.FirstName,
                LastName = tempUser.LastName,
                Role = RoleId.CUSTOMER.ToString(),
                AccessToken = jwtToken,
                RefreshToken = refreshToken,
                IsActive = tempUser.IsActive,
                IsBanned = tempUser.IsBanned
            };
            var customer = await _customerRepository.GetByIdAsync(tempUser.Id);
            if (customer != null)
            {
                responseLogin.Email = customer.Email;
                responseLogin.PhoneNumber = customer.PhoneNumber;
                responseLogin.ProfilePictureUrl = customer.ProfilePictureUrl;
                responseLogin.GoogleId = customer.GoogleId;
            }

            return ResponseModel.Success(ResponseConstants.Login(true), responseLogin);
        }
    }
}