using NET1814_MilkShop.Repositories.CoreHelpers.Constants;
using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Models.CheckoutModels;
using NET1814_MilkShop.Repositories.Repositories;
using NET1814_MilkShop.Repositories.UnitOfWork;

namespace NET1814_MilkShop.Services.Services;

public interface ICheckoutService
{
    Task<ResponseModel> Checkout(Guid userId, CheckoutModel model);
}

public class CheckoutService : ICheckoutService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CheckoutService(IUnitOfWork unitOfWork, IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseModel> Checkout(Guid userId, CheckoutModel model)
    {
        if (model.PaymentMethod == "PAYOS")
        {
            // tạo link payos trong đây
        }

        var cart = await _GetCartByUserId(userId);
        if (cart == null)
        {
            return ResponseModel.Success(ResponseConstants.NotFound("giỏ hàng"), null);
        }

        List<CartDetail> cartItems = await _GetCartDetails(cart.CartId);
        if (!cartItems.Any())
        {
            return ResponseModel.Success(ResponseConstants.Get("giỏ hàng", false), null);
        }
        
        //check quantity coi còn hàng không

        // thêm vào order
        var orders = new Order
        {
            Id = Guid.NewGuid(),
            CustomerId = userId,
            TotalPrice = GetTotalPrice(cartItems),
            ShippingFee = model.ShippingFee,
            TotalAmount = GetTotalPrice(cartItems) + model.ShippingFee,
            VoucherId = 1, // de tam 1 voucher
            Address = model.Address,
            PhoneNumber = model.PhoneNumber,
            Note = model.Note,
            PaymentMethod = model.PaymentMethod,
            StatusId = 1, //mac dinh la pending
        };
        _orderRepository.Add(orders);

        //thêm vào order detail
        var orderDetailsList = cartItems.Select(x =>
            new OrderDetail
            {
                OrderId = orders.Id,
                ProductId = x.ProductId,
                Quantity = x.Quantity,
                UnitPrice = x.Product.OriginalPrice,
                ProductName = x.Product.Name,
                ItemPrice = x.Quantity * x.Product.OriginalPrice //check sale price va original price
            }
        );
        _orderRepository.AddRange(orderDetailsList);

        // xóa cart detail
        _cartRepository.RemoveRange(cartItems);
        
        // cập nhật quantity trong product

        var res = await _unitOfWork.SaveChangesAsync();
        if (res > 0)
        {
            return ResponseModel.Success(ResponseConstants.Create("đơn hàng", true), items);
        }

        return ResponseModel.Error(ResponseConstants.Create("đơn hàng", false));
    }

    private decimal GetTotalPrice(List<CartDetail> list)
    {
        decimal total = 0;
        foreach (var a in list)
        {
            var price = a.Product.OriginalPrice;
            total += a.Quantity * price;
        }

        return total;
    }
}