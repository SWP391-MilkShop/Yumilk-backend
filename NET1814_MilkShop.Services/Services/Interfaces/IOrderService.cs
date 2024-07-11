using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Models.OrderModels;

namespace NET1814_MilkShop.Services.Services.Interfaces;

public interface IOrderService
{
    Task<ResponseModel> GetOrderAsync(OrderQueryModel model);
    Task<ResponseModel> GetOrderHistoryAsync(Guid customerId, OrderHistoryQueryModel model);
    Task<ResponseModel> GetOrderHistoryDetailAsync(Guid userId, Guid id);
    Task<ResponseModel> CancelOrderAsync(Guid userId, Guid orderId);
    Task<ResponseModel> UpdateOrderStatusAsync(Guid id, OrderStatusModel model);
    Task<ResponseModel> GetOrderStatsAsync(OrderStatsQueryModel queryModel);
    Task<ResponseModel> CancelOrderAdminStaffAsync(Guid id);
    Task<ResponseModel> GetOrderHistoryDetailDashBoardAsync(Guid orderId);
}