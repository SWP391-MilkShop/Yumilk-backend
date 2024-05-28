using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Models.OrderModels;
using NET1814_MilkShop.Repositories.Repositories;
using NET1814_MilkShop.Services.CoreHelpers;
using System.Linq.Expressions;

namespace NET1814_MilkShop.Services.Services
{
    public interface IOrderService
    {
        Task<ResponseModel> GetOrderAsync(OrderQueryModel model);
    }

    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<ResponseModel> GetOrderAsync(OrderQueryModel model)
        {
            var query = _orderRepository.GetOrdersQuery();

            #region(filter)

            if (!string.IsNullOrEmpty(model.SearchTerm))
            {
                query = query.Where(o =>
                    o.Address.Contains(model
                        .SearchTerm)); // chưa nghĩ ra search theo cái gì nên tạm thời để so với address
            }

            if (!string.IsNullOrEmpty(model.Email))
            {
                query = query.Where(o => string.Equals(o.Customer!.Email, model.Email));
            }

            if (model.TotalAmount > 0)
            {
                query = query.Where(o => o.TotalAmount > model.TotalAmount);
            }

            if (model.ToOrderDate is not null)
            {
                query = query.Where(o => o.CreatedAt <= model.ToOrderDate);
            }

            if (!string.IsNullOrEmpty(model.OrderStatus))
            {
                query = query.Where(o => string.Equals(o.Status!.Name, model.OrderStatus));
            }

            #endregion

            #region(sorting)

            if ("desc".Equals(model.SortOrder?.ToLower()))
            {
                query = query.OrderByDescending(GetSortProperty(model));
            }
            else
            {
                query = query.OrderBy(GetSortProperty(model));
            }

            #endregion

            // chuyển về OrderModel
            var orderModelQuery = query.Select(x => new OrderModel
            {
                Id = x.Id,
                CustomerId = x.Customer!.UserId,
                TotalAmount = x.TotalAmount,
                PhoneNumber = x.PhoneNumber,
                Address = x.Address,
                OrderStatus = x.Status!.Name,
                CreatedDate = x.CreatedAt,
                PaymentDate = x.PaymentDate,
            });

            #region(paging)

            var orders = await PagedList<OrderModel>.CreateAsync(
                orderModelQuery,
                model.Page,
                model.PageSize
            );
            return new ResponseModel
            {
                Data = orders,
                Message = orders.TotalCount > 0 ? "Get orders successfully" : "No brands found",
                Status = "Success"
            };

            #endregion
        }

        private static Expression<Func<Order, object>> GetSortProperty(
            OrderQueryModel queryModel
        ) =>
            queryModel.SortColumn?.ToLower() switch
            {
                "totalamount" => order => order.TotalAmount,
                "createdat" => order => order.CreatedAt,
                "paymentdate" => order =>
                    order.PaymentDate, //cái này có thể null, chưa thống nhất (TH paymentmethod là COD thì giao xong mới lưu thông tin vô db hay lưu thông tin vô db lúc đặt hàng thành công luôn)
                _ => order => order.Id, //chưa biết mặc định sort theo cái gì nên để tạm là id
            };
    }
}