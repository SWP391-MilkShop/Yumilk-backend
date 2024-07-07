using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Models.CartModels;

namespace NET1814_MilkShop.Services.Services.Interfaces;

public interface ICartService
{
    Task<ResponseModel> GetCartAsync(Guid customerId, CartQueryModel model);
    Task<ResponseModel> AddToCartAsync(Guid customerId, AddToCartModel model);

    /// <summary>
    /// Update cart and remove invalid items
    /// </summary>
    /// <param name="customerId"></param>
    /// <returns></returns>
    Task<ResponseModel> UpdateCartAsync(Guid customerId);

    Task<ResponseModel> ClearCartAsync(Guid customerId);

    Task<ResponseModel> UpdateCartItemAsync(
        Guid customerId,
        Guid productId,
        UpdateCartItemModel model
    );

    Task<ResponseModel> DeleteCartItemAsync(Guid customerId, Guid productId);
}