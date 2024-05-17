using Microsoft.IdentityModel.Tokens;
using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Repositories;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NET1814_MilkShop.Services.Services
{
    public interface IAuthenticationService
    {
        Task<ResponseLoginModel> LoginAsync(RequestLoginModel model);
    }
    public sealed class AuthenticationService : IAuthenticationService
    {
        private readonly string Key = "qwertyuiopasdfghjklzxcvbnmasdasdasdasdasdasdasdas";
        private readonly IAuthenticationRepository _authenticationRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public AuthenticationService(IAuthenticationRepository authenticationRepository, IRefreshTokenRepository refreshTokenRepository)
        {
            _authenticationRepository = authenticationRepository;
            _refreshTokenRepository = refreshTokenRepository;
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
                    Message = "Tên đăng nhập hoặc mật khẩu sai"
                };
            }
        }

        private string CreateJwtRefreshToken(User isUserExisted)
        {
            var randomByte = new Byte[64];
            var token = Convert.ToBase64String(randomByte);
            var refreshToken = new RefreshToken
            {
                Id = int.Parse(Guid.NewGuid().ToString()),
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
                    new Claim(ClaimTypes.Role, isUserExisted.Role.Name)
                }),
                Expires = DateTime.UtcNow.AddMinutes(5),
                SigningCredentials = credential,
            };
            var token = tokenHandler.CreateToken(tokenDescription);
            return tokenHandler.WriteToken(token);
        }
    }
}

