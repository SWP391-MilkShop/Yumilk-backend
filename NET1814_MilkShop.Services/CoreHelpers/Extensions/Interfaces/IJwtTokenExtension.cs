using NET1814_MilkShop.Repositories.Data.Entities;

namespace NET1814_MilkShop.Services.CoreHelpers.Extensions.Interfaces;

public interface IJwtTokenExtension
{
    string CreateJwtToken(User user, TokenType tokenType);
    string CreateVerifyCode();
}