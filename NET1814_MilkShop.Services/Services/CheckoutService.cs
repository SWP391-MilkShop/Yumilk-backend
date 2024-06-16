using NET1814_MilkShop.Repositories.CoreHelpers.Constants;
using NET1814_MilkShop.Repositories.CoreHelpers.Enum;
using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Models.CheckoutModels;
using NET1814_MilkShop.Repositories.Models.OrderModels;
using NET1814_MilkShop.Repositories.Repositories;
using NET1814_MilkShop.Repositories.UnitOfWork;

namespace NET1814_MilkShop.Services.Services;

public interface ICheckoutService
{
    Task<ResponseModel> Checkout(Guid userId, CheckoutModel model);
}

public class CheckoutService : ICheckoutService
{
    private readonly ICartRepository _cartRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPaymentService _paymentService;

    public CheckoutService(
        IUnitOfWork unitOfWork,
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        ICartRepository cartRepository,
        ICustomerRepository customerRepository,
        IPaymentService paymentService
    )
    {
        _customerRepository = customerRepository;
        _cartRepository = cartRepository;
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _paymentService = paymentService;
    }

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
            return ResponseModel.Success(ResponseConstants.OverLimit("số lượng sản phẩm"), resp);
        }

        // lấy address theo address id
        var address = await _customerRepository.GetCustomerAddressById(model.AddressId);
        if (address == null)
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
            ReceiverName = address.ReceiverName ?? "",
            Address =
                address.Address
                + ", "
                + address.WardName
                + ", "
                + address.DistrictName
                + ", "
                + address.ProvinceName,
            WardCode = address.WardCode,
            DistrictId = address.DistrictId,
            PhoneNumber = address.PhoneNumber + "", //cộng thêm này để chắc chắn ko null (ko báo lỗi biên dịch)
            Note = model.Note,
            PaymentMethod = model.PaymentMethod,
            StatusId = (int)OrderStatusId.PENDING,
            OrderCode = await GenerateOrderCode(),
            TotalGram = GetTotalGram(cart.CartDetails.ToList())
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
                TotalGram = orders.TotalGram,
                ShippingFee = orders.ShippingFee,
                Address = orders.Address,
                PhoneNumber = orders.PhoneNumber,
                Note = orders.Note,
                OrderDetail = ToOrderDetailModel(cartTemp),
            };
            if (model.PaymentMethod == "PAYOS")
            {
                var paymentLink = await _paymentService.CreatePaymentLink(orders.OrderCode.Value);
                return ResponseModel.Success(ResponseConstants.Create("đơn hàng", true), paymentLink);
            }

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
    private int  GetTotalGram(List<CartDetail> list)
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
                * (x.Product.SalePrice == 0 ? x.Product.OriginalPrice : x.Product.SalePrice)
        });
        return res;
    }
}