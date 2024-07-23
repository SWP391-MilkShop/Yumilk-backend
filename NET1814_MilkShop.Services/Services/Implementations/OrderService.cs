using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NET1814_MilkShop.Repositories.CoreHelpers.Constants;
using NET1814_MilkShop.Repositories.CoreHelpers.Enum;
using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Models.OrderModels;
using NET1814_MilkShop.Repositories.Models.ShippingModels;
using NET1814_MilkShop.Repositories.Repositories.Interfaces;
using NET1814_MilkShop.Repositories.UnitOfWork.Interfaces;
using NET1814_MilkShop.Services.CoreHelpers;
using NET1814_MilkShop.Services.CoreHelpers.Extensions;
using NET1814_MilkShop.Services.Services.Interfaces;
using Newtonsoft.Json;

namespace NET1814_MilkShop.Services.Services.Implementations;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProductRepository _productRepository;
    private readonly IShippingService _shippingService;
    private readonly IPaymentService _paymentService;
    private readonly ICustomerRepository _customerRepository;
    private readonly IOrderLogRepository _orderLogRepository;

    public OrderService(IOrderRepository orderRepository, IUnitOfWork unitOfWork,
        IProductRepository productRepository,
        IShippingService shippingService,
        IPaymentService paymentService,
        IOrderLogRepository orderLogRepository,
        ICustomerRepository customerRepository)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _productRepository = productRepository;
        _shippingService = shippingService;
        _paymentService = paymentService;
        _orderLogRepository = orderLogRepository;
        _customerRepository = customerRepository;
    }

    /// <summary>
    /// admin lấy đơn đặt hàng của các khách hàng
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
            query = query.Where(o =>
                string.Equals(o.Status!.Name, model.OrderStatus));
        }

        if (model.IsPreorder != null)
        {
            query = query.Where(o => o.IsPreorder == model.IsPreorder);
        }

        #endregion

        #region(sorting)

        //mặc định sort giảm dần theo created_at và tăng dần theo order status (PENDING)
        query = "desc".Equals(model.SortOrder?.ToLower())
            ? query.OrderBy(x => x.StatusId).ThenByDescending(GetSortProperty(model))
            : query.OrderBy(x => x.StatusId).ThenBy(GetSortProperty(model));

        // tạm thời để pending lên đầu nhưng created_at sort ko chuẩn
        // var pendingOrders = query.Where(o => o.StatusId == (int)OrderStatusId.PENDING);
        //
        // var otherOrders = query.Where(o => o.StatusId != (int)OrderStatusId.PENDING);
        //
        // query = pendingOrders.Union(otherOrders);

        #endregion

        // chuyển về OrderModel
        var orderModels = query.Select(order => new OrderModel
        {
            Id = order.Id,
            CustomerId = order.Customer!.UserId,
            TotalAmount = order.TotalAmount,
            PhoneNumber = order.PhoneNumber,
            Email = order.Email,
            Address = order.Address,
            PaymentMethod = order.PaymentMethod,
            OrderStatus = order.Status!.Name,
            CreatedDate = order.CreatedAt,
            PaymentDate = order.PaymentDate,
            IsPreorder = order.IsPreorder
        });


        #region(paging)

        var orders = await PagedList<OrderModel>.CreateAsync(
            orderModels,
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

    private async Task<object?> GetInformation(Order order)
    {
        var paymentData = await _paymentService.GetPaymentLinkInformation(order.Id);
        return paymentData.StatusCode == 200 ? paymentData.Data : null;
    }

    private void AddOrderStatusLog(Guid orderId, int statusId)
    {
        switch (statusId)
        {
            case (int)OrderStatusId.Processing:
                var processingLog = new OrderLog
                {
                    OrderId = orderId,
                    NewStatusId = (int)OrderStatusId.Processing,
                    StatusName = OrderStatusId.Processing.ToString(),
                };
                _orderLogRepository.Add(processingLog);
                break;
            case (int)OrderStatusId.Shipped:
                var shippedLog = new OrderLog
                {
                    OrderId = orderId,
                    NewStatusId = (int)OrderStatusId.Shipped,
                    StatusName = OrderStatusId.Shipped.ToString(),
                };
                _orderLogRepository.Add(shippedLog);
                break;
            case (int)OrderStatusId.Delivered:
                var deliveredLog = new OrderLog
                {
                    OrderId = orderId,
                    NewStatusId = (int)OrderStatusId.Delivered,
                    StatusName = OrderStatusId.Delivered.ToString(),
                };
                _orderLogRepository.Add(deliveredLog);
                break;
            case (int)OrderStatusId.Cancelled:
                var cancelLog = new OrderLog
                {
                    OrderId = orderId,
                    NewStatusId = (int)OrderStatusId.Cancelled,
                    StatusName = OrderStatusId.Cancelled.ToString(),
                };
                _orderLogRepository.Add(cancelLog);
                break;
            case (int)OrderStatusId.Preordered:
                var preorderLog = new OrderLog
                {
                    OrderId = orderId,
                    NewStatusId = (int)OrderStatusId.Preordered,
                    StatusName = OrderStatusId.Preordered.ToString(),
                };
                _orderLogRepository.Add(preorderLog);
                break;
        }
    }

    /// <summary>
    /// customer lấy lịch sử đặt hàng của mình
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

        if (model.IsPreorder != null)
        {
            query = query.Where(o => o.IsPreorder == model.IsPreorder);
        }

        #endregion

        #region sort

        query = "desc".Equals(model.SortOrder?.ToLower())
            ? query.OrderByDescending(GetSortProperty(model)).ThenBy(x => x.StatusId)
            : query.OrderBy(GetSortProperty(model)).ThenBy(x => x.StatusId);

        #endregion

        #region ToOrderHistoryModel

        var orderHistoryQuery = query.Select(o => new OrderHistoryModel
        {
            Id = o.Id,
            TotalAmount = o.TotalAmount,
            PaymentMethod = o.PaymentMethod,
            OrderStatus = o.Status!.Name,
            ProductList = o.OrderDetails
                .Where(u => u.OrderId == o.Id)
                .Select(h => new
                {
                    h.Product.Name,
                    h.Thumbnail,
                }),
            CreatedAt = o.CreatedAt,
            IsPreorder = o.IsPreorder
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

    /// <summary>
    /// customer lấy ra chi tiết của 1 đơn hàng cụ thể
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="orderId"></param>
    /// <returns></returns>
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
            UnitPrice = x.UnitPrice,
            ItemPrice = x.ItemPrice,
            Thumbnail = x.Thumbnail
        }).ToList();
        var orderLog = await _orderLogRepository.GetOrderLogQuery(orderId).ToListAsync();
        var orderStatusLogs = orderLog.Select(x => new OrderLogsModel
        {
            Status = x.StatusName,
            CreatedAt = x.CreatedAt
        }).ToList();
        var detail = new OrderDetailModel
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            ReceiverName = order.ReceiverName, //order.RecieverName (do chua update db nen chua co)
            Email = order.Email,
            PhoneNumber = order.PhoneNumber,
            Address = order.Address,
            Note = order.Note,
            OrderDetail = pModel,
            TotalPriceBeforeDiscount =
                order.VoucherAmount + order.PointAmount + order.TotalPrice, //tổng tiền trước khi giảm giá
            VoucherDiscount = order.VoucherAmount,
            PointDiscount = order.PointAmount,
            TotalPriceAfterDiscount = order.TotalPrice, // tổng tiền sau khi giảm giá
            RecievingPoint = order.TotalPrice.ApplyPercentage(1),
            ShippingFee = order.ShippingFee,
            TotalAmount = order.TotalAmount,
            PaymentMethod = order.PaymentMethod,
            OrderStatus = order.Status!.Name,
            CreatedAt = order.CreatedAt,
            PaymentData = order.PaymentMethod == "PAYOS" ? await GetInformation(order) : null,
            Logs = orderStatusLogs,
            IsPreorder = order.IsPreorder,
            ShippingCode = order.ShippingCode
        };
        if (order is { StatusId: (int)OrderStatusId.Shipped or (int)OrderStatusId.Delivered, ShippingCode: not null })
        {
            var orderDetail = await _shippingService.GetOrderDetailAsync(orderId);
            if (orderDetail.StatusCode != 200)
            {
                return ResponseModel.Error(ResponseConstants.Get("chi tiết đơn hàng", false));
            }

            var json = JsonConvert.SerializeObject(orderDetail.Data);
            var shippingData = JsonConvert.DeserializeObject<ExpectedDeliveryTime>(json);
            var expectedDeliveryDate = shippingData!.DeliveryTime;
            detail.ExpectedDeliveryDate = expectedDeliveryDate;
        }

        return ResponseModel.Success(ResponseConstants.Get("chi tiết đơn hàng", true), detail);
    }

    /// <summary>
    /// customer cancel order
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="orderId"></param>
    /// <returns></returns>
    public async Task<ResponseModel> CancelOrderAsync(Guid userId, Guid orderId)
    {
        var message = "";
        var order = await _orderRepository.GetByOrderIdAsync(orderId, false);
        if (order == null || order.CustomerId != userId)
        {
            return ResponseModel.BadRequest(ResponseConstants.NotFound("Đơn hàng"));
        }

        if (order.StatusId == (int)OrderStatusId.Cancelled)
        {
            return ResponseModel.BadRequest("Đơn hàng đã bị hủy từ trước");
        }

        if (order.StatusId != (int)OrderStatusId.Pending && order.StatusId != (int)OrderStatusId.Processing)
        {
            return ResponseModel.BadRequest("Đơn hàng đang trong quá trình giao nên bạn không thể hủy.");
        }

        if (order.PaymentMethod == "PAYOS")
        {
            var cancelResult = await _paymentService.CancelPaymentLink(order.Id);
            if (cancelResult.StatusCode == 200)
            {
                message = "link thanh toán thành công và ";
            }
        }

        order.StatusId = (int)OrderStatusId.Cancelled;
        AddOrderStatusLog(order.Id, (int)OrderStatusId.Cancelled);
        foreach (var o in order.OrderDetails)
        {
            o.Product.Quantity += o.Quantity;
            _productRepository.Update(o.Product);
        }

        _orderRepository.Update(order);
        var res = await _unitOfWork.SaveChangesAsync();
        if (res > 0)
        {
            return ResponseModel.Success(ResponseConstants.Cancel(message + "đơn hàng", true), null);
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
        queryModel.SortColumn?.ToLower() switch
        {
            "totalamount" => order => order.TotalAmount,
            "paymentdate" => order =>
                order.PaymentDate, //cái này có thể null, chưa thống nhất (TH paymentmethod là COD thì giao xong mới lưu thông tin vô db hay lưu thông tin vô db lúc đặt hàng thành công luôn)
            "orderid" => order => order.Id,
            _ => order => order.CreatedAt, //mặc định là sort theo createdat
        };

    public async Task<ResponseModel> UpdateOrderStatusAsync(Guid id, OrderStatusModel model)
    {
        var order = await _orderRepository.GetByOrderIdAsync(id, include: false);
        if (order == null)
        {
            return ResponseModel.BadRequest(ResponseConstants.NotFound("đơn hàng"));
        }

        if (order.StatusId == model.StatusId)
        {
            return ResponseModel.Success(ResponseConstants.NoChangeIsMade, null);
        }

        switch (order.StatusId)
        {
            case (int)OrderStatusId.Pending:
                if (model.StatusId != (int)OrderStatusId.Processing)
                {
                    return ResponseModel.BadRequest(
                        "Đơn hàng ở trạng thái chờ xử lý chỉ có thể chuyển sang trạng thái đang xử lý");
                }

                break;
            case (int)OrderStatusId.Processing:
                if (model.StatusId != (int)OrderStatusId.Shipped)
                {
                    return ResponseModel.BadRequest("Đơn hàng đang xử lý chỉ có thể chuyển sang trạng thái đang giao");
                }

                break;
            case (int)OrderStatusId.Preordered:
                if (model.StatusId != (int)OrderStatusId.Shipped)
                {
                    return ResponseModel.BadRequest("Đơn hàng đặt trước chỉ có thể chuyển sang trạng thái đang giao");
                }

                break;
            case (int)OrderStatusId.Cancelled:
                return ResponseModel.BadRequest("Đơn hàng đã bị hủy từ trước");
            case (int)OrderStatusId.Delivered:
                return ResponseModel.BadRequest("Đơn hàng đã được giao");
            case (int)OrderStatusId.Shipped:
                return ResponseModel.BadRequest("Đơn hàng đang trên đường giao");
        }

        int result;
        if (model.StatusId == (int)OrderStatusId.Shipped)
        {
            // Check if the order is a preorder and needs stock updates
            if (order.StatusId == (int)OrderStatusId.Preordered)
            {
                foreach (var o in order.OrderDetails)
                {
                    o.Product.Quantity -= o.Quantity;
                    if (o.Product.Quantity < 0)
                    {
                        return ResponseModel.Error("Có lỗi xảy ra khi cập nhật số lượng sản phẩm trong kho");
                    }

                    _productRepository.Update(o.Product);
                }

                // Save changes if stock was updated
                var stockUpdateResult = await _unitOfWork.SaveChangesAsync();
                if (stockUpdateResult <= 0)
                {
                    return ResponseModel.Error("Không thể cập nhật số lượng sản phẩm trong kho");
                }
            }

            _unitOfWork.Detach(order); // detach order to prevent tracking
            // order code and shipping status is already updated in the shipping service
            // var orderShipping = await _shippingService.CreateOrderShippingAsync(id);
            // if (orderShipping.StatusCode != 200)
            // {
            //     return orderShipping;
            // }
        }

        order.StatusId = model.StatusId;
        //Add order log status
        AddOrderStatusLog(order.Id, model.StatusId);
        _orderRepository.Update(order);
        result = await _unitOfWork.SaveChangesAsync();
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
        var delivered = query.Where(o => o.StatusId == (int)OrderStatusId.Delivered);
        var totalOrdersPerStatus = await query
            .GroupBy(o => o.Status)
            .Select(g => new OrderStatusCount
            {
                Status = g.Key!.Name.ToUpper(),
                Count = g.Count()
            })
            .ToListAsync();
        var stats = new OrderStatsModel
        {
            TotalOrders = await query.CountAsync(),
            TotalRevenue = await delivered.SumAsync(o => o.TotalPrice),
            TotalShippingFee = await delivered.SumAsync(o => o.ShippingFee),
        };
        foreach (var item in totalOrdersPerStatus)
        {
            // uppercase để đồng bộ case
            var count = stats.TotalOrdersPerStatus.FirstOrDefault(x => x.Status.ToUpper() == item.Status);
            if (count != null) count.Count = item.Count;
        }

        return ResponseModel.Success(ResponseConstants.Get("thống kê đơn hàng", true), stats);
    }

    public async Task<ResponseModel> CancelOrderAdminStaffAsync(Guid id)
    {
        var message = "";
        var order = await _orderRepository.GetByOrderIdAsync(id, false);
        if (order is null)
        {
            return ResponseModel.BadRequest("Không tìm thấy đơn hàng");
        }

        if (order.StatusId == (int)OrderStatusId.Cancelled)
        {
            return ResponseModel.BadRequest("Đơn hàng đã bị hủy từ trước");
        }

        if (order.PaymentMethod == "PAYOS")
        {
            var cancelResult = await _paymentService.CancelPaymentLink(order.Id);
            if (cancelResult.StatusCode == 200)
            {
                message = "link thanh toán và ";
            }
        }

        if (order.PointAmount != 0)
        {
            var customer = await _customerRepository.GetCustomersQuery()
                .FirstOrDefaultAsync(x => x.UserId == order.CustomerId);
            if (customer == null)
            {
                return ResponseModel.BadRequest(ResponseConstants.NotFound("Khách hàng"));
            }

            customer.Point += order.PointAmount;
            _customerRepository.Update(customer);
        }

        order.StatusId = (int)OrderStatusId.Cancelled;
        AddOrderStatusLog(order.Id, (int)OrderStatusId.Cancelled);
        foreach (var o in order.OrderDetails)
        {
            o.Product.Quantity += o.Quantity;
            _productRepository.Update(o.Product);
        }

        // _unitOfWork.Detach(order);
        _orderRepository.Update(order);
        var res = await _unitOfWork.SaveChangesAsync();
        if (res > 0)
        {
            return ResponseModel.Success(order.ShippingCode != null
                ? "Hủy thành công, đơn hàng có mã vận chuyển. Vui lòng hủy bên đơn vị vận chuyển"
                : ResponseConstants.Cancel(message + "đơn hàng", true), null);
        }

        return ResponseModel.Error(ResponseConstants.Cancel("đơn hàng", false));
    }

    /// <summary>
    /// staff, admin lấy chi tiết đơn hàng cụ thể
    /// </summary>
    /// <param name="orderId"></param>
    /// <returns></returns>
    public async Task<ResponseModel> GetOrderHistoryDetailDashBoardAsync(Guid orderId)
    {
        var order = await _orderRepository.GetByOrderIdAsync(orderId, true);
        if (order == null)
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
            Thumbnail = x.Thumbnail
        }).ToList();

        var orderLog = await _orderLogRepository.GetOrderLogQuery(orderId).ToListAsync();
        var orderStatusLogs = orderLog.Select(x => new OrderLogsModel
        {
            Status = x.StatusName,
            CreatedAt = x.CreatedAt
        }).ToList();
        var detail = new OrderDetailModel
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            ReceiverName = order.ReceiverName, //order.RecieverName (do chua update db nen chua co)
            PhoneNumber = order.PhoneNumber,
            Email = order.Email,
            Address = order.Address,
            Note = order.Note,
            OrderDetail = pModel,
            TotalPriceBeforeDiscount =
                order.VoucherAmount + order.PointAmount + order.TotalPrice, //tổng tiền trước khi giảm giá
            VoucherDiscount = order.VoucherAmount,
            PointDiscount = order.PointAmount,
            TotalPriceAfterDiscount = order.TotalPrice, // tổng tiền sau khi giảm giá
            RecievingPoint = order.TotalPrice.ApplyPercentage(1),
            ShippingFee = order.ShippingFee,
            TotalAmount = order.TotalAmount,
            PaymentMethod = order.PaymentMethod,
            OrderStatus = order.Status!.Name,
            CreatedAt = order.CreatedAt,
            PaymentData = order.PaymentMethod == "PAYOS" ? await GetInformation(order) : null,
            Logs = orderStatusLogs,
            IsPreorder = order.IsPreorder,
            ShippingCode = order.ShippingCode
        };
        if (order is { StatusId: (int)OrderStatusId.Shipped or (int)OrderStatusId.Delivered, ShippingCode: not null })
        {
            var orderDetail = await _shippingService.GetOrderDetailAsync(orderId);
            if (orderDetail.StatusCode != 200)
            {
                return ResponseModel.Error(ResponseConstants.Get("chi tiết đơn hàng", false));
            }

            var json = JsonConvert.SerializeObject(orderDetail.Data);
            var shippingData = JsonConvert.DeserializeObject<ExpectedDeliveryTime>(json);
            var expectedDeliveryDate = shippingData!.DeliveryTime;
            detail.ExpectedDeliveryDate = expectedDeliveryDate;
        }

        return ResponseModel.Success(ResponseConstants.Get("chi tiết đơn hàng", true), detail);
    }

    public async Task<ResponseModel> UpdateOrderStatusDeliveredAsync(Guid id)
    {
        var order = await _orderRepository.GetByIdIncludeCustomerAsync(id);
        if (order == null)
        {
            return ResponseModel.BadRequest(ResponseConstants.NotFound("đơn hàng"));
        }

        if (order.StatusId != (int)OrderStatusId.Shipped)
        {
            return ResponseModel.BadRequest("Đơn hàng chưa được giao");
        }

        // Handle point for customer
        // 100 VND = 1 point
        order.Customer!.Point += order.TotalPrice.ApplyPercentage(1);
        _customerRepository.Update(order.Customer);
        // Handle order logs
        order.StatusId = (int)OrderStatusId.Delivered;
        AddOrderStatusLog(order.Id, (int)OrderStatusId.Delivered);
        // Handle payment date
        if (order.PaymentMethod == "COD")
        {
            order.PaymentDate = DateTime.UtcNow;
        }
        _orderRepository.Update(order);
        var result = await _unitOfWork.SaveChangesAsync();
        if (result > 0)
        {
            return ResponseModel.Success(ResponseConstants.Update("trạng thái đơn hàng", true), null);
        }

        return ResponseModel.Error(ResponseConstants.Update("trạng thái đơn hàng", false));
    }

    public async Task<ResponseModel> GetPaymentMethodStats()
    {
        var orders = await _orderRepository.GetOrderQuery().Where(x => x.StatusId == (int)OrderStatusId.Delivered)
            .ToListAsync();
        if (orders.Count == 0)
        {
            return ResponseModel.BadRequest("Không có đơn hàng nào trong hệ thống");
        }

        var totalCod = orders.Count(x => x.PaymentMethod == "COD");
        var totalPayOs = orders.Count(x => x.PaymentMethod == "PayOS");
        var percentCod = totalCod * 100 * 1.0 / orders.Count;
        var percentPayOs = 100 - percentCod;
        return ResponseModel.Success(ResponseConstants.Get("thống kê phương thức thanh toán", true), new
        {
            totalCod, totalPayOs, percentCod, percentPayOs
        });
    }

    public async Task<ResponseModel> GetOrdersStatsByDateAsync(OrderStatsQueryModel queryModel)
    {
        if (queryModel.FromOrderDate > DateTime.Now)
        {
            return ResponseModel.BadRequest(ResponseConstants.InvalidFromDate);
        }

        if (queryModel.FromOrderDate > queryModel.ToOrderDate)
        {
            return ResponseModel.BadRequest(ResponseConstants.InvalidFilterDate);
        }

        // default is from last 30 days
        var from = queryModel.FromOrderDate ?? DateTime.Now.AddDays(-30);
        // default is now
        var to = queryModel.ToOrderDate ?? DateTime.Now;

        var orders = await _orderRepository.GetOrderQuery()
            .Where(o => o.CreatedAt >= from && o.CreatedAt <= to && o.StatusId != (int)OrderStatusId.Cancelled)
            .ToListAsync();
        // order per day of week (theo thứ)
        /*var orderPerDayOfWeek = orders
            .GroupBy(o => o.CreatedAt.DayOfWeek)
            .Select(x => new OrderStatsPerDate
            {
                DateTime = x.Key.ToString() switch
                {
                    "Monday" => "Thứ 2",
                    "Tuesday" => "Thứ 3",
                    "Wednesday" => "Thứ 4",
                    "Thursday" => "Thứ 5",
                    "Friday" => "Thứ 6",
                    "Saturday" => "Thứ 7",
                    "Sunday" => "Chủ nhật",
                    _ => ""
                },
                Count = x.Count()
            }).OrderBy(x => x.DateTime).ToList();*/
        var daysOfWeek = new Dictionary<DayOfWeek, string>
        {
            { DayOfWeek.Monday, "Thứ 2" },
            { DayOfWeek.Tuesday, "Thứ 3" },
            { DayOfWeek.Wednesday, "Thứ 4" },
            { DayOfWeek.Thursday, "Thứ 5" },
            { DayOfWeek.Friday, "Thứ 6" },
            { DayOfWeek.Saturday, "Thứ 7" },
            { DayOfWeek.Sunday, "Chủ nhật" }
        };

        var ordersGroupedByDay = orders
            .GroupBy(o => o.CreatedAt.DayOfWeek)
            .ToDictionary(g => g.Key, g => g.Count());

        var orderPerDayOfWeek = daysOfWeek
            .Select(d => new OrderStatsPerDate
            {
                DateTime = d.Value,
                Count = ordersGroupedByDay.ContainsKey(d.Key) ? ordersGroupedByDay[d.Key] : 0
            })
            .OrderBy(x => x.DateTime)
            .ToList();

        // order per date (theo ngày)
        var orderPerDay = orders
            .GroupBy(x => x.CreatedAt.Date)
            .Select(x => new OrderStatsPerDate
            {
                DateTime = x.Key.ToString("yyyy-MM-dd"),
                Count = x.Count()
            }).OrderBy(x => x.DateTime).ToList();

        // order per month (theo tháng)
        var orderPerMonth = orders
            .GroupBy(x => x.CreatedAt.Month)
            .Select(x => new OrderStatsPerDate
            {
                DateTime = x.Key.ToString(),
                Count = x.Count()
            }).OrderBy(x => x.DateTime).ToList();

        return ResponseModel.Success(ResponseConstants.Get("thống kê đơn hàng theo ngày", true), new
        {
            orderPerDayOfWeek,
            orderPerDay,
            orderPerMonth
        });
    }

    public async Task<ResponseModel> GetRevenueByMonthAsync(int year)
    {
        var orders = _orderRepository.GetOrderQuery();
        var revenueByMonth = await orders
            .Where(x => x.CreatedAt.Year == year && x.StatusId == (int)OrderStatusId.Delivered)
            .GroupBy(x => x.CreatedAt.Month)
            .Select(x => new
            {
                Month = x.Key,
                Revenue = x.Sum(o => o.TotalPrice)
            }).ToListAsync();
        return ResponseModel.Success(ResponseConstants.Get("thống kê doanh thu theo tháng", true), revenueByMonth);
    }
}