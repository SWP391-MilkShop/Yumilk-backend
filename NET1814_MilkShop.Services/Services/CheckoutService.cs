using Net.payOS.Types;
using NET1814_MilkShop.Repositories.CoreHelpers.Constants;
using NET1814_MilkShop.Repositories.CoreHelpers.Enum;
using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Models.CheckoutModels;
using NET1814_MilkShop.Repositories.Models.OrderModels;
using NET1814_MilkShop.Repositories.Models.PaymentModels;
using NET1814_MilkShop.Repositories.Repositories;
using NET1814_MilkShop.Repositories.UnitOfWork;
using Newtonsoft.Json;

namespace NET1814_MilkShop.Services.Services;

public interface ICheckoutService
{
    Task<ResponseModel> Checkout(Guid userId, CheckoutModel model);
    Task<ResponseModel> PreOrderCheckout(Guid userId, PreorderCheckoutModel model);
}

public class CheckoutService : ICheckoutService
{
    private readonly ICartRepository _cartRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPaymentService _paymentService;
    private readonly IPreorderProductRepository _preorderProductRepository;

    public CheckoutService(
        IUnitOfWork unitOfWork,
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        ICartRepository cartRepository,
        ICustomerRepository customerRepository,
        IPaymentService paymentService,
        IPreorderProductRepository preorderProductRepository
    )
    {
        _customerRepository = customerRepository;
        _cartRepository = cartRepository;
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _paymentService = paymentService;
        _preorderProductRepository = preorderProductRepository;
    }

    /// <summary>
    /// checkout cart
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    public async Task<ResponseModel> Checkout(Guid userId, CheckoutModel model)
    {
        var cart = await _cartRepository.GetByCustomerIdAsync(userId, true);
        if (cart == null)
        {
            return ResponseModel.BadRequest(ResponseConstants.NotFound("Giỏ hàng"));
        }

        if (!cart.CartDetails.Any())
        {
            return ResponseModel.Success(ResponseConstants.CartIsEmpty, cart.CartDetails);
        }

        //check quantity coi còn hàng không
        List<CartDetail> unavailableItems = new List<CartDetail>();
        foreach (var c in cart.CartDetails)
        {
            if (c.Quantity > c.Product.Quantity)
            {
                unavailableItems.Add(c);
            }
        }

        if (unavailableItems.Any())
        {
            var resp = unavailableItems.Select(x => new CheckoutQuantityResponseModel()
            {
                ProductName = x.Product.Name,
                Quantity = x.Quantity,
                Message =
                    $"Số lượng sản phẩm bạn mua ({x.Quantity}) đã vượt quá số lượng sản phẩm còn lại của cửa hàng ({x.Product.Quantity}). Vui lòng kiểm tra lại giỏ hàng của quý khách!"
            });
            return ResponseModel.BadRequest(ResponseConstants.OverLimit("Số lượng sản phẩm"), resp);
        }

        // lấy address theo address id
        var customerAddress = await _customerRepository.GetCustomerAddressById(model.AddressId);
        if (customerAddress == null || customerAddress.UserId != userId)
        {
            return ResponseModel.BadRequest(ResponseConstants.NotFound("Địa chỉ"));
        }

        // thêm vào order
        var orders = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = userId,
            TotalPrice = GetTotalPrice(cart.CartDetails.ToList()),
            ShippingFee = model.ShippingFee,
            TotalAmount = GetTotalPrice(cart.CartDetails.ToList()) + model.ShippingFee,
            VoucherId = 1, // de tam 1 voucher
            ReceiverName = customerAddress.ReceiverName ?? "",
            Address =
                customerAddress.Address
                + ", "
                + customerAddress.WardName
                + ", "
                + customerAddress.DistrictName
                + ", "
                + customerAddress.ProvinceName,
            WardCode = customerAddress.WardCode,
            DistrictId = customerAddress.DistrictId,
            PhoneNumber = customerAddress.PhoneNumber + "", //cộng thêm này để chắc chắn ko null (ko báo lỗi biên dịch)
            Note = model.Note,
            PaymentMethod = model.PaymentMethod,
            StatusId = (int)OrderStatusId.PENDING,
            OrderCode = await GenerateOrderCode()
        };
        _orderRepository.Add(orders);

        //thêm vào order detail
        var orderDetailsList = cart.CartDetails.Select(x => new OrderDetail
        {
            OrderId = orders.Id,
            ProductId = x.ProductId,
            Quantity = x.Quantity,
            UnitPrice = x.Product.SalePrice == 0 ? x.Product.OriginalPrice : x.Product.SalePrice,
            ProductName = x.Product.Name,
            ItemPrice =
                x.Quantity
                * (x.Product.SalePrice == 0
                    ? x.Product.OriginalPrice
                    : x.Product.SalePrice) //check sale price va original price
        });
        var cartTemp = cart.CartDetails.ToList();
        _orderRepository.AddRange(orderDetailsList);

        // xóa cart detail
        _cartRepository.RemoveRange(cart.CartDetails); ////tạo hàm mẫu ở order repo

        // cập nhật quantity trong product
        foreach (var c in cart.CartDetails)
        {
            c.Product.Quantity -= c.Quantity;
            _productRepository.Update(c.Product);
        }

        var res = await _unitOfWork.SaveChangesAsync();
        if (res > 0)
        {
            var resp = new CheckoutResponseModel
            {
                OrderId = orders.Id,
                CustomerId = orders.CustomerId,
                FullName = orders.ReceiverName,
                TotalAmount = orders.TotalAmount,
                ShippingFee = orders.ShippingFee,
                Address = orders.Address,
                PhoneNumber = orders.PhoneNumber,
                Note = orders.Note,
                PaymentMethod = orders.PaymentMethod,
                CreatedAt = orders.CreatedAt,
                OrderDetail = ToOrderDetailModel(cartTemp),
            };
            if (model.PaymentMethod == "PAYOS")
            {
                var paymentLink = await _paymentService.CreatePaymentLink(orders.OrderCode.Value);
                if (paymentLink.Status == "ERROR")
                {
                    return ResponseModel.Error(ResponseConstants.Create("đơn hàng", false));
                }

                var json = JsonConvert.SerializeObject(paymentLink.Data);
                var paymentData = JsonConvert.DeserializeObject<PaymentDataModel>(json);
                resp.OrderCode = paymentData!.OrderCode;
                resp.CheckoutUrl = paymentData!.CheckoutUrl;
            }

            return ResponseModel.Success(ResponseConstants.Create("đơn hàng", true), resp);
        }

        return ResponseModel.Error(ResponseConstants.Create("đơn hàng", false));
    }

    /// <summary>
    /// checkout preorder
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    public async Task<ResponseModel> PreOrderCheckout(Guid userId, PreorderCheckoutModel model)
    {
        var product = await _preorderProductRepository.GetByProductIdAsync(model.ProductId);
        if (product == null || product.Product.StatusId != (int)ProductStatusId.PREORDER)
        {
            return ResponseModel.BadRequest(ResponseConstants.NotFound("Sản phẩm") +
                                            " hoặc sản phẩm đang không trong quá trình Pre-order");
        }

        if (DateTime.Now < product.StartDate ||
            DateTime.Now > product.EndDate)
        {
            return ResponseModel.BadRequest(ResponseConstants.NotInPreOrder);
        }

        if (model.Quantity + product.Product.Quantity > product.MaxPreOrderQuantity)
        {
            var resp = new CheckoutQuantityResponseModel
            {
                ProductName = product.Product.Name,
                Quantity = model.Quantity,
                Message =
                    $"Số lượng sản phẩm bạn mua ({model.Quantity}) đã vượt quá " +
                    $"số lượng sản phẩm tối đa cho phép ({product.MaxPreOrderQuantity - product.Product.Quantity})."
            };
            return ResponseModel.BadRequest(ResponseConstants.OverLimit("Số lượng sản phẩm"), resp);
        }

        var customerAddress = await _customerRepository.GetCustomerAddressById(model.AddressId);
        if (customerAddress == null || customerAddress.UserId != userId)
        {
            return ResponseModel.BadRequest(ResponseConstants.NotFound("Địa chỉ"));
        }

        var preOrder = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = userId,
            TotalPrice = product.Product.SalePrice == 0
                ? (product.Product.OriginalPrice * model.Quantity)
                : (product.Product.SalePrice * model.Quantity),
            ShippingFee = model.ShippingFee,
            TotalAmount = product.Product.SalePrice == 0
                ? (product.Product.OriginalPrice * model.Quantity)
                : (product.Product.SalePrice * model.Quantity) + model.ShippingFee,
            VoucherId = 1, // de tam 1 voucher
            ReceiverName = customerAddress.ReceiverName + "",
            Address =
                customerAddress.Address
                + ", "
                + customerAddress.WardName
                + ", "
                + customerAddress.DistrictName
                + ", "
                + customerAddress.ProvinceName,
            WardCode = customerAddress.WardCode,
            DistrictId = customerAddress.DistrictId,
            PhoneNumber = customerAddress.PhoneNumber + "", //cộng thêm này để chắc chắn ko null (ko báo lỗi biên dịch)
            Note = model.Note,
            PaymentMethod = "PAYOS",
            StatusId = (int)OrderStatusId.PENDING,
            OrderCode = await GenerateOrderCode(),
            TotalGram = product.Product.Unit!.Gram * model.Quantity,
        };
        _orderRepository.Add(preOrder);
        var preOrderDetail = new OrderDetail
        {
            OrderId = preOrder.Id,
            ProductId = product.ProductId,
            Quantity = model.Quantity,
            UnitPrice = product.Product.SalePrice == 0 ? product.Product.OriginalPrice : product.Product.SalePrice,
            ProductName = product.Product.Name,
            ItemPrice =
                model.Quantity
                * (product.Product.SalePrice == 0
                    ? product.Product.OriginalPrice
                    : product.Product.SalePrice) //check sale price va original price
        };
        _orderRepository.Add(preOrderDetail);
        product.Product.Quantity += model.Quantity;
        _productRepository.Update(product.Product);
        var res = await _unitOfWork.SaveChangesAsync();
        if (res > 0)
        {
            var paymentLink = await _paymentService.CreatePaymentLink(preOrder.OrderCode.Value);
            return paymentLink;
        }

        return ResponseModel.Error(ResponseConstants.Create("đơn hàng", false));
    }

    private async Task<int> GenerateOrderCode()
    {
        Random random = new Random();
        int orderCode;
        do
        {
            orderCode = random.Next(0, 10000000);
        } while (await _orderRepository.IsExistOrderCode(orderCode));

        return orderCode;
    }

    private int GetTotalPrice(List<CartDetail> list)
    {
        int total = 0;
        foreach (var x in list)
        {
            var price = x.Product.SalePrice == 0 ? x.Product.OriginalPrice : x.Product.SalePrice;
            total += x.Quantity * price;
        }

        return total;
    }

    private IEnumerable<CheckoutOrderDetailModel> ToOrderDetailModel(List<CartDetail> list)
    {
        var res = list.Select(x => new CheckoutOrderDetailModel
        {
            ProductId = x.ProductId,
            ProductName = x.Product.Name,
            Quantity = x.Quantity,
            UnitPrice = x.Product.SalePrice == 0 ? x.Product.OriginalPrice : x.Product.SalePrice,
            ItemPrice =
                x.Quantity
                * (x.Product.SalePrice == 0 ? x.Product.OriginalPrice : x.Product.SalePrice)
        });
        return res;
    }
}