using NET1814_MilkShop.Repositories.CoreHelpers.Constants;
using NET1814_MilkShop.Repositories.CoreHelpers.Enum;
using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Models.CheckoutModels;
using NET1814_MilkShop.Repositories.Models.OrderModels;
using NET1814_MilkShop.Repositories.Models.PaymentModels;
using NET1814_MilkShop.Repositories.Repositories.Interfaces;
using NET1814_MilkShop.Repositories.UnitOfWork.Interfaces;
using NET1814_MilkShop.Services.Services.Interfaces;
using Newtonsoft.Json;

namespace NET1814_MilkShop.Services.Services.Implementations;

public class CheckoutService : ICheckoutService
{
    private readonly ICartRepository _cartRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPaymentService _paymentService;
    private readonly IPreorderProductRepository _preorderProductRepository;
    private readonly IEmailService _emailService;
    private readonly IUserRepository _userRepository;
    private readonly ICartDetailRepository _cartDetailRepository;


    public CheckoutService(
        IUnitOfWork unitOfWork,
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        ICartRepository cartRepository,
        ICustomerRepository customerRepository,
        IPaymentService paymentService,
        IPreorderProductRepository preorderProductRepository, IEmailService emailService,
        IUserRepository userRepository, ICartDetailRepository cartDetailRepository
    )
    {
        _customerRepository = customerRepository;
        _cartRepository = cartRepository;
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _paymentService = paymentService;
        _preorderProductRepository = preorderProductRepository;
        _emailService = emailService;
        _userRepository = userRepository;
        _cartDetailRepository = cartDetailRepository;
    }

    /// <summary>
    /// checkout cart
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    public async Task<ResponseModel> Checkout(Guid userId, CheckoutModel model)
    {
        var userActive = await _userRepository.GetByIdAsync(userId);
        if (userActive!.IsBanned)
        {
            return ResponseModel.BadRequest(ResponseConstants.Banned);
        }

        if (!userActive.IsActive)
        {
            return ResponseModel.BadRequest(ResponseConstants.UserNotActive);
        }

        var cart = await _cartRepository.GetCartByCustomerId(userId);
        if (cart == null)
        {
            return ResponseModel.BadRequest(ResponseConstants.NotFound("Giỏ hàng"));
        }

        if (!cart.CartDetails.Any())
        {
            return ResponseModel.Success(ResponseConstants.CartIsEmpty, cart.CartDetails);
        }

        //check quantity coi còn hàng không
        List<CheckoutQuantityResponseModel> unavailableItems = [];
        foreach (var c in cart.CartDetails)
        {
            if (c.Quantity > c.Product.Quantity)
            {
                unavailableItems.Add(new CheckoutQuantityResponseModel
                {
                    ProductName = c.Product.Name,
                    Quantity = c.Quantity,
                    Message =
                        $"Số lượng sản phẩm bạn mua ({c.Quantity}) đã vượt quá số lượng sản phẩm còn lại của cửa hàng ({c.Product.Quantity}). Vui lòng kiểm tra lại giỏ hàng của quý khách!"
                });
            }
            else if (c.Product.StatusId is (int)ProductStatusId.OutOfStock or (int)ProductStatusId.Preorder)
            {
                unavailableItems.Add(new CheckoutQuantityResponseModel
                {
                    ProductName = c.Product.Name,
                    Quantity = c.Quantity,
                    Message =
                        "Sản phẩm đã hết hàng hoặc đang trong quá trình pre-order. Vui lòng kiểm tra lại giỏ hàng của quý khách!"
                });
            }
        }

        if (unavailableItems.Count != 0)
        {
            return ResponseModel.BadRequest(ResponseConstants.OverLimit("Số lượng sản phẩm"), unavailableItems);
        }

        // lấy address theo address id
        var customerAddress = await _customerRepository.GetCustomerAddressById(model.AddressId);
        if (customerAddress == null || customerAddress.UserId != userId)
        {
            return ResponseModel.BadRequest(ResponseConstants.NotFound("Địa chỉ"));
        }

        var customerEmail = await _customerRepository.GetCustomerEmail(userId);
        // thêm vào order
        var orders = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = userId,
            TotalPrice = GetTotalPrice(cart.CartDetails.ToList()),
            ShippingFee = model.ShippingFee,
            TotalAmount = model.PaymentMethod == "COD"
                ? GetTotalPrice(cart.CartDetails.ToList())
                : GetTotalPrice(cart.CartDetails.ToList()) + model.ShippingFee,
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
            StatusId = (int)OrderStatusId.Pending,
            OrderCode = model.PaymentMethod == "COD" ? null : await GenerateOrderCode(),
            TotalGram = GetTotalGram(cart.CartDetails.ToList()),
            Email = customerEmail,
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
                    : x.Product.SalePrice), //check sale price va original price
            Thumbnail = x.Product.Thumbnail
        });
        var cartTemp = cart.CartDetails.ToList();
        _orderRepository.AddRange(orderDetailsList);
        // xóa cart detail

        _cartDetailRepository.RemoveRange(cart.CartDetails); ////tạo hàm mẫu ở order repo

        // cập nhật quantity trong product
        foreach (var c in cart.CartDetails)
        {
            c.Product.Quantity -= c.Quantity;
            if (c.Product.Quantity == 0)
            {
                c.Product.StatusId = (int)ProductStatusId.OutOfStock;
            }

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
                Email = customerEmail,
                TotalAmount = orders.TotalAmount,
                TotalGram = orders.TotalGram,
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
                resp.CheckoutUrl = paymentData.CheckoutUrl;
            }

            await _emailService.SendPurchaseEmailAsync(customerEmail, userActive.FirstName);
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
        var userActive = await _userRepository.GetByIdAsync(userId);
        if (userActive!.IsBanned)
        {
            return ResponseModel.BadRequest(ResponseConstants.Banned);
        }

        if (!userActive.IsActive)
        {
            return ResponseModel.BadRequest(ResponseConstants.UserNotActive);
        }

        var product = await _preorderProductRepository.GetByProductIdAsync(model.ProductId);
        if (product == null || product.Product.StatusId != (int)ProductStatusId.Preorder)
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

        var customerEmail = await _customerRepository.GetCustomerEmail(userId);
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
            StatusId = (int)OrderStatusId.Pending,
            OrderCode = await GenerateOrderCode(),
            TotalGram = product.Product.Unit!.Gram * model.Quantity,
            Email = customerEmail,
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
                    : product.Product.SalePrice), //check sale price va original price
            Thumbnail = product.Product.Thumbnail
        };
        _orderRepository.Add(preOrderDetail);
        product.Product.Quantity += model.Quantity;
        _productRepository.Update(product.Product);
        var res = await _unitOfWork.SaveChangesAsync();
        if (res > 0)
        {
            var resp = new CheckoutResponseModel
            {
                OrderId = preOrder.Id,
                CustomerId = preOrder.CustomerId,
                FullName = preOrder.ReceiverName,
                Email = customerEmail,
                TotalAmount = preOrder.TotalAmount,
                TotalGram = preOrder.TotalGram,
                ShippingFee = preOrder.ShippingFee,
                Address = preOrder.Address,
                PhoneNumber = preOrder.PhoneNumber,
                Note = preOrder.Note,
                PaymentMethod = preOrder.PaymentMethod,
                CreatedAt = preOrder.CreatedAt,
                OrderDetail = new CheckoutOrderDetailModel
                {
                    ProductId = preOrderDetail.ProductId,
                    ProductName = preOrderDetail.ProductName,
                    Quantity = preOrderDetail.Quantity,
                    UnitPrice = preOrderDetail.UnitPrice,
                    ItemPrice = preOrderDetail.ItemPrice,
                    Thumbnail = preOrderDetail.Product.Thumbnail
                },
            };
            var paymentLink = await _paymentService.CreatePaymentLink(preOrder.OrderCode.Value);
            if (paymentLink.Status == "ERROR")
            {
                return ResponseModel.Error(ResponseConstants.Create("đơn hàng", false));
            }

            var json = JsonConvert.SerializeObject(paymentLink.Data);
            var paymentData = JsonConvert.DeserializeObject<PaymentDataModel>(json);
            resp.OrderCode = paymentData!.OrderCode;
            resp.CheckoutUrl = paymentData.CheckoutUrl;
            await _emailService.SendPurchaseEmailAsync(customerEmail, userActive.FirstName);
            return ResponseModel.Success(ResponseConstants.Create("đơn hàng", true), resp);
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

    private int GetTotalGram(List<CartDetail> list)
    {
        var totalGram = 0;
        foreach (var x in list)
        {
            var gram = x.Product.Unit!.Gram;
            totalGram += x.Quantity * gram;
        }

        return totalGram;
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
                * (x.Product.SalePrice == 0 ? x.Product.OriginalPrice : x.Product.SalePrice),
            Thumbnail = x.Product.Thumbnail
        });
        return res;
    }
}