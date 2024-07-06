using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Models.CheckoutModels;

namespace NET1814_MilkShop.Services.Services.Interfaces;

public interface ICheckoutService
{
    Task<ResponseModel> Checkout(Guid userId, CheckoutModel model);
    Task<ResponseModel> PreOrderCheckout(Guid userId, PreorderCheckoutModel model);
}