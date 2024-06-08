using System.Linq.Expressions;
using Azure;
using Microsoft.EntityFrameworkCore;
using NET1814_MilkShop.Repositories.CoreHelpers.Constants;
using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Models.OrderModels;
using NET1814_MilkShop.Repositories.Repositories;
using NET1814_MilkShop.Services.CoreHelpers;

namespace NET1814_MilkShop.Services.Services
{
    public interface IOrderService
    {
        Task<ResponseModel> GetOrderAsync(OrderQueryModel model);
        Task<ResponseModel> GetOrderHistoryAsync(Guid customerId, OrderHistoryQueryModel model);
    }

    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        /// <summary>
        /// đơn đặt hàng của các khách hàng
        /// </summary>
        /// <param name="model"></param>
        /// <returns>trả về danh sách các order của hệ thống</returns>
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

            if (model.FromOrderDate == null && model.ToOrderDate != null)
            {
                return ResponseModel.BadRequest("Phải có ngày bắt đầu trong trường hợp có ngày kết thúc");
            }

            if (model.FromOrderDate != null && model.ToOrderDate == null)
            {
                if (model.FromOrderDate.Value.Date > DateTime.Now.Date)
                {
                    return ResponseModel.BadRequest(ResponseConstants.InvalidFilterDate);
                }

                query = query.Where(o =>
                    o.CreatedAt.Date <= DateTime.Now.Date && o.CreatedAt.Date >= model.FromOrderDate.Value.Date);
            }

            if (model.FromOrderDate != null && model.ToOrderDate != null)
            {
                if (model.FromOrderDate.Value.Date > model.ToOrderDate.Value.Date)
                {
                    return ResponseModel.BadRequest(ResponseConstants.InvalidFilterDate);
                }

                query = query.Where(o =>
                    o.CreatedAt.Date <= model.ToOrderDate.Value.Date && o.CreatedAt >= model.FromOrderDate.Value.Date);
            }

            if (!string.IsNullOrEmpty(model.PaymentMethod))
            {
                query = query.Where(o => o.PaymentMethod == model.PaymentMethod);
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
                PaymentMethod = x.PaymentMethod,
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
            /*return new ResponseModel
            {
                Data = orders,
                Message = orders.TotalCount > 0 ? "Get orders successfully" : "No brands found",
                Status = "Success"
            };*/

            #endregion

            return ResponseModel.Success(
                ResponseConstants.Get("đơn hàng", orders.TotalCount > 0),
                orders
            );
        }

        /// <summary>
        /// lịch sử đặt hàng của khách hàng cụ thể
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="model"></param>
        /// <returns>trả về danh sách các sản phẩm đã được đặt (toàn bộ danh sách kể cả hủy)</returns>
        public async Task<ResponseModel> GetOrderHistoryAsync(Guid customerId, OrderHistoryQueryModel model)
        {
            var query = _orderRepository.GetOrderHistory(customerId);

            #region filter

            if (model.SearchTerm != null)
            {
                query = query.Where(o => o.OrderDetails.Any(c => c.Product.Name.Contains(model.SearchTerm)));
            }

            if (model.TotalAmount > 0)
            {
                query = query.Where(o => o.TotalAmount > model.TotalAmount);
            }

            if (model.FromOrderDate == null && model.ToOrderDate != null)
            {
                return ResponseModel.BadRequest("Phải có ngày bắt đầu trong trường hợp có ngày kết thúc");
            }

            if (model.FromOrderDate != null && model.ToOrderDate == null)
            {
                if (model.FromOrderDate.Value.Date > DateTime.Now.Date)
                {
                    return ResponseModel.BadRequest(ResponseConstants.InvalidFilterDate);
                }

                query = query.Where(o =>
                    o.CreatedAt.Date <= DateTime.Now.Date && o.CreatedAt.Date >= model.FromOrderDate.Value.Date);
            }

            if (model.FromOrderDate != null && model.ToOrderDate != null)
            {
                if (model.FromOrderDate.Value.Date > model.ToOrderDate.Value.Date)
                {
                    return ResponseModel.BadRequest(ResponseConstants.InvalidFilterDate);
                }

                query = query.Where(o =>
                    o.CreatedAt.Date <= model.ToOrderDate.Value.Date && o.CreatedAt >= model.FromOrderDate.Value.Date);
            }

            if (model.OrderStatus.HasValue)
            {
                query = query.Where(o => o.StatusId == model.OrderStatus);
            }

            #endregion

            #region sort

            query = "desc".Equals(model.SortOrder?.ToLower())
                ? query.OrderByDescending(GetSortProperty(model))
                : query.OrderBy(GetSortProperty(model));

            #endregion

            #region ToOrderHistoryModel

            var orderHistoryQuery = query.Select(o => new OrderHistoryModel
            {
                OrderId = o.Id,
                TotalAmount = o.TotalAmount,
                OrderStatus = o.Status!.Name,
                ProductList = null // do là iqueryable nên ko sử dụng được trực tiếp
            });

            #endregion

            #region Paging

            var pagedOrders = await PagedList<OrderHistoryModel>.CreateAsync(
                orderHistoryQuery,
                model.Page,
                model.PageSize
            );

            // gán ngược lại productlist
            foreach (var orderHistory in pagedOrders.Items)
            {
                orderHistory.ProductList = await GetProductByOrderIdAsync(orderHistory.OrderId);
            }

            #endregion

            return ResponseModel.Success(
                ResponseConstants.Get("lịch sử đơn hàng", pagedOrders.TotalCount > 0),
                pagedOrders
            );
        }

        private async Task<List<string>> GetProductByOrderIdAsync(Guid id)
        {
            List<string> list = new();
            var order = await _orderRepository.GetByOrderIdAsync(id);
            foreach (var a in order!.OrderDetails)
            {
                list.Add(a.Product.Name);
            }

            return list;
        }

        private static Expression<Func<Order, object>> GetSortProperty<T>(
            T queryModel
        ) where T : QueryModel =>
            queryModel.SortColumn?.ToLower().Replace(" ", "") switch
            {
                "totalamount" => order => order.TotalAmount,
                "createdat" => order => order.CreatedAt,
                "paymentdate" => order =>
                    order.PaymentDate, //cái này có thể null, chưa thống nhất (TH paymentmethod là COD thì giao xong mới lưu thông tin vô db hay lưu thông tin vô db lúc đặt hàng thành công luôn)
                _ => order => order.Id, //chưa biết mặc định sort theo cái gì nên để tạm là id
            };
    }
}