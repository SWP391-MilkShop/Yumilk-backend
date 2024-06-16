using System.Linq.Expressions;
using System.Numerics;
using Azure;
using Microsoft.EntityFrameworkCore;
using NET1814_MilkShop.Repositories.CoreHelpers.Constants;
using NET1814_MilkShop.Repositories.CoreHelpers.Enum;
using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Models.OrderModels;
using NET1814_MilkShop.Repositories.Repositories;
using NET1814_MilkShop.Repositories.UnitOfWork;
using NET1814_MilkShop.Services.CoreHelpers;
using System.Linq.Expressions;

namespace NET1814_MilkShop.Services.Services
{
    public interface IOrderService
    {
        Task<ResponseModel> GetOrderAsync(OrderQueryModel model);
        Task<ResponseModel> GetOrderHistoryAsync(Guid customerId, OrderHistoryQueryModel model);
        Task<ResponseModel> GetOrderHistoryDetailAsync(Guid userId, Guid id);
        Task<ResponseModel> CancelOrderAsync(Guid userId, Guid orderId);
        Task<ResponseModel> UpdateOrderStatusAsync(Guid id, OrderStatusModel model);
        Task<ResponseModel> GetOrderStatsAsync(OrderStatsQueryModel queryModel);
        Task<ResponseModel> CancelOrderAdminStaffAsync(Guid id);
    }

    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductRepository _productRepository;
        private readonly IShippingService _shippingService;

        public OrderService(IOrderRepository orderRepository, IUnitOfWork unitOfWork,
            IProductRepository productRepository, IShippingService shippingService)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
            _productRepository = productRepository;
            _shippingService = shippingService;
        }

        /// <summary>
        /// đơn đặt hàng của các khách hàng
        /// </summary>
        /// <param name="model"></param>
        /// <returns>trả về danh sách các order của hệ thống</returns>
        public async Task<ResponseModel> GetOrderAsync(OrderQueryModel model)
        {
            var query = _orderRepository.GetOrderQuery();

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
                ProductList = o.OrderDetails
                    .Where(u => u.OrderId == o.Id)
                    .Select(h => new
                    {
                        h.Product.Name,
                        h.Thumbnail,
                        h.CreatedAt
                    })
            });

            #endregion

            #region Paging

            var pagedOrders = await PagedList<OrderHistoryModel>.CreateAsync(
                orderHistoryQuery,
                model.Page,
                model.PageSize
            );

            // gán ngược lại productlist
            // foreach (var orderHistory in pagedOrders.Items)
            // {
            //     var list = await GetProductByOrderIdAsync(orderHistory.OrderId);
            //     orderHistory.ProductList = list.Select(x => x.Name).ToList();
            // }

            #endregion

            return ResponseModel.Success(
                ResponseConstants.Get("lịch sử đơn hàng", pagedOrders.TotalCount > 0),
                pagedOrders
            );
        }

        public async Task<ResponseModel> GetOrderHistoryDetailAsync(Guid userId, Guid orderId)
        {
            var order = await _orderRepository.GetByOrderIdAsync(orderId, true);
            if (order == null || order.CustomerId != userId)
            {
                return ResponseModel.BadRequest(ResponseConstants.NotFound("Đơn hàng"));
            }

            var pModel = order.OrderDetails.Select(x => new CheckoutOrderDetailModel
            {
                ProductId = x.ProductId,
                ProductName = x.ProductName,
                Quantity = x.Quantity,
                UnitPrice = x.Product.SalePrice == 0 ? x.Product.OriginalPrice : x.Product.SalePrice,
                ItemPrice = x.ItemPrice,
                ThumbNail = x.Thumbnail
            }).ToList();

            var detail = new OrderDetailModel
            {
                RecieverName = order.ReceiverName, //order.RecieverName (do chua update db nen chua co)
                PhoneNumber = order.PhoneNumber,
                Address = order.Address,
                Note = order.Note,
                OrderDetail = pModel,
                TotalPrice = order.TotalPrice,
                ShippingFee = order.ShippingFee,
                TotalAmount = order.TotalAmount,
                PaymentMethod = order.PaymentMethod,
                OrderStatus = order.Status!.Name,
                CreatedAt = order.CreatedAt
            };
            return ResponseModel.Success(ResponseConstants.Get("chi tiết đơn hàng", true), detail);
        }

        public async Task<ResponseModel> CancelOrderAsync(Guid userId, Guid orderId)
        {
            var order = await _orderRepository.GetByOrderIdAsync(orderId, false);
            if (order == null || order.CustomerId != userId)
            {
                return ResponseModel.BadRequest(ResponseConstants.NotFound("Đơn hàng"));
            }

            if (order.StatusId == 5)
            {
                return ResponseModel.BadRequest("Đơn hàng đã bị hủy từ trước");
            }

            if (order.StatusId != 1 && order.StatusId != 2)
            {
                return ResponseModel.BadRequest("Đơn hàng đang trong quá trình giao nên bạn không thể hủy.");
            }

            order.StatusId = 5;
            foreach (var o in order.OrderDetails)
            {
                o.Product.Quantity += o.Quantity;
                _productRepository.Update(o.Product);
            }

            _orderRepository.Update(order);
            var res = await _unitOfWork.SaveChangesAsync();
            if (res > 0)
            {
                return ResponseModel.Success(ResponseConstants.Cancel("đơn hàng", true), null);
            }

            return ResponseModel.Error(ResponseConstants.Cancel("đơn hàng", false));
        }

        private async Task<List<Product>> GetProductByOrderIdAsync(Guid id)
        {
            List<Product> list = new();
            var order = await _orderRepository.GetByOrderIdAsync(id, true);
            foreach (var a in order!.OrderDetails)
            {
                list.Add(a.Product);
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

        public async Task<ResponseModel> UpdateOrderStatusAsync(Guid id, OrderStatusModel model)
        {
            var order = await _orderRepository.GetByIdAsync(id, includeDetails: false);
            if (order == null)
            {
                return ResponseModel.BadRequest(ResponseConstants.NotFound("đơn hàng"));
            }

            if (order.StatusId == model.StatusId)
            {
                return ResponseModel.Success(ResponseConstants.NoChangeIsMade, null);
            }

            if (order.StatusId > model.StatusId)
            {
                return ResponseModel.BadRequest(ResponseConstants.Update("trạng thái đơn hàng", false));
            }

            if (model.StatusId == (int)OrderStatusId.SHIPPING)
            {
                var orderShippingAsync = await _shippingService.CreateOrderShippingAsync(id);
                if (orderShippingAsync.StatusCode != 200)
                {
                    return orderShippingAsync;
                }
                order.StatusId = model.StatusId;
                _orderRepository.Update(order); 
                var resultShipping = await _unitOfWork.SaveChangesAsync();
                if (resultShipping < 0)
                {
                    return ResponseModel.BadRequest("Có lỗi xảy ra khi cập nhật trạng thái đơn hàng");
                }
                return ResponseModel.Success(ResponseConstants.Update("trạng thái đơn hàng",true),
                    orderShippingAsync.Data);
            }

            order.StatusId = model.StatusId;
            _orderRepository.Update(order);
            var result = await _unitOfWork.SaveChangesAsync();
            if (result > 0)
            {
                return ResponseModel.Success(ResponseConstants.Update("trạng thái đơn hàng", true), null);
            }

            return ResponseModel.Error(ResponseConstants.Update("trạng thái đơn hàng", false));
        }

        public async Task<ResponseModel> GetOrderStatsAsync(OrderStatsQueryModel queryModel)
        {
            if (queryModel.FromOrderDate > DateTime.Now)
            {
                return ResponseModel.BadRequest(ResponseConstants.InvalidFromDate);
            }

            if (queryModel.FromOrderDate > queryModel.ToOrderDate)
            {
                return ResponseModel.BadRequest(ResponseConstants.InvalidFilterDate);
            }

            var query = _orderRepository.GetOrderQueryWithStatus();
            // default is from last 30 days
            var from = queryModel.FromOrderDate ?? DateTime.Now.AddDays(-30);
            // default is now
            var to = queryModel.ToOrderDate ?? DateTime.Now;
            query = query.Where(o => o.CreatedAt >= from && o.CreatedAt <= to);
            // only count delivered orders
            var delivered = query.Where(o => o.StatusId == (int)OrderStatusId.DELIVERED);
            var totalOrdersPerStatus = await query.GroupBy(o => o.Status)
                .ToDictionaryAsync(g => g.Key!.Name.ToUpper(), g => g.Count());
            var stats = new OrderStatsModel
            {
                TotalOrders = await query.CountAsync(),
                TotalRevenue = await delivered.SumAsync(o => o.TotalPrice),
                TotalShippingFee = await delivered.SumAsync(o => o.ShippingFee),
            };
            foreach (var status in Enum.GetNames(typeof(OrderStatusId)))
            {
                stats.TotalOrdersPerStatus[status] = totalOrdersPerStatus.GetValueOrDefault(status, 0);
            }

            return ResponseModel.Success(ResponseConstants.Get("thống kê đơn hàng", true), stats);
        }
        
        public async Task<ResponseModel> CancelOrderAdminStaffAsync(Guid id)
        {
            var order = await _orderRepository.GetByOrderIdAsync(id, false);
            if (order is null)
            {
                return ResponseModel.BadRequest("Không tìm thấy đơn hàng");
            }
            
            order.StatusId = 5;
             
            foreach (var o in order.OrderDetails)
            {
                o.Product.Quantity += o.Quantity;
                _productRepository.Update(o.Product);
            }
 
            _orderRepository.Update(order);
            var res = await _unitOfWork.SaveChangesAsync();
            if (res > 0)
            {
                return ResponseModel.Success(order.ShippingCode != null ? "Hủy thành công, đơn hàng có mã vận chuyển. Vui lòng hủy bên đơn vị vận chuyển" 
                    : ResponseConstants.Cancel("đơn hàng", true), null);
            }
            return ResponseModel.Error(ResponseConstants.Cancel("đơn hàng", false));           
        }
    }
}