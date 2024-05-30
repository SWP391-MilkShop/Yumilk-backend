using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NET1814_MilkShop.Repositories.Data.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NET1814_MilkShop.Services.CoreHelpers.Extensions
{
    public enum TokenType
    {
        Access,
        Refresh,
        Authentication,
        Reset
    }

    public interface IJwtTokenExtension
    {
        string CreateJwtToken(User user, TokenType tokenType);
        string CreateVerifyCode();
    }

    public class JwtTokenExtension : IJwtTokenExtension
    {
        private readonly IConfiguration _configuration;
        private readonly string str =
            "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        public JwtTokenExtension(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreateJwtToken(User user, TokenType tokenType)
        {
            var claims = GetClaims(user, tokenType);
            claims.Add(new Claim("tokenType", tokenType.ToString()));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GetKey(tokenType)));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(GetExpiry(tokenType)),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private List<Claim> GetClaims(User user, TokenType tokenType)
        {
            var claims = new List<Claim> { new Claim("UserId", user.Id.ToString()) };
            switch (tokenType)
            {
                case TokenType.Access:
                    claims.Add(new Claim(ClaimTypes.Role, user.RoleId.ToString()));
                    break;
                case TokenType.Refresh:
                    break;
                case TokenType.Authentication:
                    claims.Add(new Claim("Token", user.VerificationCode));
                    break;
                case TokenType.Reset:
                    claims.Add(new Claim("Token", user.ResetPasswordCode));
                    break;
                default:
                    throw new ArgumentException("Invalid token type");
            }
            return claims;
        }

        private string GetKey(TokenType tokenType)
        {
            string key = "";
            switch (tokenType)
            {
                case TokenType.Access:
                    key = _configuration["Jwt:AccessTokenKey"];
                    break;
                case TokenType.Refresh:
                    key = _configuration["Jwt:RefreshTokenKey"];
                    break;
                case TokenType.Authentication:
                    key = _configuration["Jwt:AuthenticationKey"];
                    break;
                case TokenType.Reset:
                    key = _configuration["Jwt:AuthenticationKey"];
                    break;
                default:
                    throw new ArgumentException("Invalid token type");
            }
            return key;
        }

        private int GetExpiry(TokenType tokenType)
        {
            int expiry = 0;
            switch (tokenType)
            {
                case TokenType.Access:
                    expiry = int.Parse(_configuration["Jwt:AccessTokenLifeTime"]);
                    break;
                case TokenType.Refresh:
                    expiry = int.Parse(_configuration["Jwt:RefreshTokenLifeTime"]);
                    break;
                case TokenType.Authentication:
                    expiry = int.Parse(_configuration["Jwt:AuthenticationLifeTime"]);
                    break;
                case TokenType.Reset:
                    expiry = int.Parse(_configuration["Jwt:AuthenticationLifeTime"]);
                    break;
                default:
                    throw new ArgumentException("Invalid token type");
            }
            return expiry;
        }

        /// <summary>
        /// Tạo mã xác thực ngẫu nhiên 6 ký tự
        /// </summary>
        /// <returns></returns>
        public string CreateVerifyCode()
        {
            Random res = new Random();
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
