using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NET1814_MilkShop.Repositories.CoreHelpers.Constants;
using NET1814_MilkShop.Repositories.CoreHelpers.Enum;
using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Models.CartModels;
using NET1814_MilkShop.Repositories.Repositories.Interfaces;
using NET1814_MilkShop.Repositories.UnitOfWork.Interfaces;
using NET1814_MilkShop.Services.CoreHelpers;
using NET1814_MilkShop.Services.CoreHelpers.Extensions;
using NET1814_MilkShop.Services.Services.Interfaces;

namespace NET1814_MilkShop.Services.Services.Implementations;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly ICartDetailRepository _cartDetailRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IVoucherRepository _voucherRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CartService(
        ICartRepository cartRepository,
        ICartDetailRepository cartDetailRepository,
        IUnitOfWork unitOfWork,
        IProductRepository productRepository,
        ICustomerRepository customerRepository, IVoucherRepository voucherRepository)
    {
        _cartRepository = cartRepository;
        _cartDetailRepository = cartDetailRepository;
        _productRepository = productRepository;
        _customerRepository = customerRepository;
        _voucherRepository = voucherRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseModel> AddToCartAsync(Guid customerId, AddToCartModel model)
    {
        var customerExist = await _customerRepository.IsExistAsync(customerId);
        if (!customerExist)
        {
            return ResponseModel.BadRequest(ResponseConstants.NotFound("Khách hàng"));
        }

        var product = await _productRepository.GetByIdNoIncludeAsync(model.ProductId);
        // Check if product is not exist or not active
        if (product is not { IsActive: true })
        {
            return ResponseModel.BadRequest(ResponseConstants.NotFound("Sản phẩm"));
        }

        if (product.StatusId == (int)ProductStatusId.OutOfStock)
        {
            return ResponseModel.BadRequest(ResponseConstants.OutOfStock);
        }

        // Check if product is in pre-order status
        if (product.StatusId == (int)ProductStatusId.Preordered)
        {
            return ResponseModel.BadRequest(ResponseConstants.NoPreorderCart);
        }

        if (model.Quantity > product.Quantity)
        {
            return ResponseModel.BadRequest(ResponseConstants.NotEnoughQuantity);
        }

        var cart = await _cartRepository.GetByCustomerIdAsync(customerId, false);
        if (cart == null)
        {
            cart = new Cart
            {
                CustomerId = customerId,
            };
            _cartRepository.Add(cart);
            var createResult = await _unitOfWork.SaveChangesAsync();
            if (createResult <= 0)
            {
                return ResponseModel.Error(ResponseConstants.AddToCart(false));
            }
        }

        var cartItem = cart.CartDetails.FirstOrDefault(x => x.ProductId == model.ProductId);
        if (cartItem == null)
        {
            cartItem = new CartDetail
            {
                CartId = cart.Id,
                ProductId = model.ProductId,
                Quantity = model.Quantity
            };
            _cartDetailRepository.Add(cartItem);
        }
        else
        {
            if (cartItem.Quantity + model.Quantity > product.Quantity)
            {
                return ResponseModel.BadRequest(
                    $"Số lượng thêm vào đã vượt quá số lượng tồn kho. Số lượng hiện có trong giỏ hàng: {cartItem.Quantity}");
            }

            cartItem.Quantity += model.Quantity;
            _cartDetailRepository.Update(cartItem);
        }

        var result = await _unitOfWork.SaveChangesAsync();
        if (result > 0)
        {
            return ResponseModel.Success(ResponseConstants.AddToCart(true), null);
        }

        return ResponseModel.Error(ResponseConstants.AddToCart(false));
    }

    public async Task<ResponseModel> UpdateCartAsync(Guid customerId)
    {
        var cart = await _cartRepository.GetByCustomerIdAsync(customerId, true);
        if (cart == null)
        {
            return ResponseModel.BadRequest(ResponseConstants.NotFound("Giỏ hàng"));
        }

        var messages = new List<string>();
        var isChanged = false;
        foreach (var cartDetail in cart.CartDetails)
        {
            if (!cartDetail.Product.IsActive)
            {
                _cartDetailRepository.Remove(cartDetail);
                messages.Add($"Sản phẩm {cartDetail.Product.Name} không còn tồn tại");
                isChanged = true;
                continue;
            }

            if (cartDetail.Product.StatusId == (int)ProductStatusId.OutOfStock || cartDetail.Product.Quantity == 0)
            {
                _cartDetailRepository.Remove(cartDetail);
                messages.Add($"Sản phẩm {cartDetail.Product.Name} đã hết hàng");
                isChanged = true;
                continue;
            }

            // Check if product is in pre-order status
            if (cartDetail.Product.StatusId == (int)ProductStatusId.Preordered)
            {
                _cartDetailRepository.Remove(cartDetail);
                messages.Add($"Sản phẩm {cartDetail.Product.Name} đang trong chế độ đặt trước");
                isChanged = true;
                continue;
            }

            if (cartDetail.Quantity > cartDetail.Product.Quantity)
            {
                cartDetail.Quantity = cartDetail.Product.Quantity;
                messages.Add(
                    $"Sản phẩm {cartDetail.Product.Name} chỉ còn {cartDetail.Product.Quantity} sản phẩm"
                );
                isChanged = true;
                _cartDetailRepository.Update(cartDetail);
            }
        }

        if (isChanged)
        {
            var result = await _unitOfWork.SaveChangesAsync();
            if (result > 0)
            {
                return ResponseModel.Success(
                    ResponseConstants.Update("giỏ hàng", true),
                    messages
                );
            }

            return ResponseModel.Error(ResponseConstants.Update("giỏ hàng", false));
        }

        return ResponseModel.Success(ResponseConstants.NoChangeIsMade, null);
    }

    public async Task<ResponseModel> GetCartAsync(Guid customerId, CartQueryModel model)
    {
        var customer = await _customerRepository.GetCustomersQuery()
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.UserId == customerId);
        if (customer!.User.IsBanned)
        {
            return ResponseModel.BadRequest(ResponseConstants.Banned);
        }

        if (!customer.User.IsActive)
        {
            return ResponseModel.BadRequest(ResponseConstants.UserNotActive);
        }

        var cart = await _cartRepository
            .GetCartQuery()
            .FirstOrDefaultAsync(x => x.CustomerId == customerId);
        if (cart == null)
        {
            var newCart = new CartModel
            {
                CustomerId = customerId,
                TotalPrice = 0,
                TotalQuantity = 0,
                TotalGram = 0,
                CartItems = PagedList<CartDetailModel>.Create(
                    new List<CartDetailModel>().AsQueryable(),
                    model.Page,
                    model.PageSize)
            };
            return ResponseModel.Success(ResponseConstants.Get("giỏ hàng", true), newCart);
        }

        var searchTerm = StringExtension.Normalize(model.SearchTerm) ?? "";
        var cartDetailQuery = _cartDetailRepository
            .GetCartDetailQuery()
            .Include(x => x.Product)
            .ThenInclude(x => x.Unit)
            .Where(x => x.CartId == cart.Id && x.Product.Name.Contains(searchTerm));
        var cartItems = cartDetailQuery.Select(x => new CartDetailModel
        {
            ProductId = x.ProductId,
            Quantity = x.Quantity,
            ProductName = x.Product.Name,
            ProductQuantity = x.Product.Quantity,
            Thumbnail = x.Product.Thumbnail,
            OriginalPrice = x.Product.OriginalPrice,
            SalePrice = x.Product.SalePrice,
            Gram = x.Product.Unit!.Gram
        });
        if (model.SortOrder == "desc")
        {
            cartItems = cartItems.OrderByDescending(GetSortProperty(model));
        }
        else
        {
            cartItems = cartItems.OrderBy(GetSortProperty(model));
        }

        var pagedList = await PagedList<CartDetailModel>.CreateAsync(
            cartItems,
            model.Page,
            model.PageSize
        );
        var totalPrice = pagedList.Items.Sum(x => (x.SalePrice > 0 ? x.SalePrice : x.OriginalPrice) * x.Quantity);
        var totalPriceAfterDiscount = totalPrice;
        var voucherDiscount = 0;
        var voucherMessage = "";
        var voucherDiscountPercent = 0;
        if (model.VoucherId != Guid.Empty)
        {
            var voucher = await _voucherRepository.GetByIdAsync(model.VoucherId);
            if (voucher == null)
            {
                return ResponseModel.BadRequest(ResponseConstants.NotFound("Voucher"));
            }

            voucherDiscountPercent = voucher.Percent;
            var handleVoucher = DiscountExtension.ApplyVoucher(voucher, totalPriceAfterDiscount);
            // if (handleVoucher.StatusCode != 200) return handleVoucher;
            voucherDiscount = (int)(handleVoucher.Data ?? 0);
            voucherMessage = handleVoucher.Message;
            totalPriceAfterDiscount -= voucherDiscount;
        }

        var pointDiscount = 0;
        var pointMessage = "";
        if (model.IsUsingPoint)
        {
            var handlePoint = DiscountExtension.ApplyPoint(customer, totalPriceAfterDiscount);
            // if (handlePoint.StatusCode != 200) return handlePoint;
            pointDiscount = (int)(handlePoint.Data ?? 0);
            pointMessage = handlePoint.Message;
            totalPriceAfterDiscount -= pointDiscount;
        }

        var cartModel = new CartModel
        {
            Id = cart.Id,
            CustomerId = cart.CustomerId,
            TotalPrice = totalPrice, // tổng tiền trước khi giảm giá
            TotalPriceAfterDiscount = totalPriceAfterDiscount, // tổng tiền sau khi giảm giá
            TotalQuantity = pagedList.Items.Sum(x => x.Quantity),
            TotalGram = pagedList.Items.Sum(x => x.Gram * x.Quantity),
            CartItems = pagedList,
            VoucherId = model.VoucherId,
            IsUsingPoint = model.IsUsingPoint,
            VoucherDiscountPercent = voucherDiscountPercent,
            VoucherDiscount = voucherDiscount,
            PointDiscount = pointDiscount,
            VoucherMessage = voucherMessage,
            PointMessage = pointMessage
        };
        return ResponseModel.Success(ResponseConstants.Get("giỏ hàng", true), cartModel);
    }

    private static Expression<Func<CartDetailModel, object>> GetSortProperty(
        CartQueryModel model
    ) =>
        model.SortColumn?.ToLower().Replace(" ", "") switch
        {
            "price" => item => item.SalePrice > 0 ? item.SalePrice : item.OriginalPrice,
            _ => item => item.ProductName
        };

    public async Task<ResponseModel> DeleteCartItemAsync(Guid customerId, Guid productId)
    {
        var cart = await _cartRepository.GetByCustomerIdAsync(customerId, false);
        if (cart == null)
        {
            return ResponseModel.BadRequest(ResponseConstants.NotFound("Giỏ hàng"));
        }

        var cartItem = cart.CartDetails.FirstOrDefault(x => x.ProductId == productId);
        if (cartItem == null)
        {
            return ResponseModel.BadRequest(
                ResponseConstants.NotFound("Sản phẩm trong giỏ hàng")
            );
        }

        //Hard delete
        _cartDetailRepository.Remove(cartItem);
        var result = await _unitOfWork.SaveChangesAsync();
        if (result > 0)
        {
            return ResponseModel.Success(
                ResponseConstants.Delete("sản phẩm trong giỏ hàng", true),
                null
            );
        }

        return ResponseModel.Error(ResponseConstants.Delete("sản phẩm trong giỏ hàng", false));
    }

    public async Task<ResponseModel> UpdateCartItemAsync(
        Guid customerId,
        Guid productId,
        UpdateCartItemModel model
    )
    {
        var cart = await _cartRepository.GetByCustomerIdAsync(customerId, true);
        if (cart == null)
        {
            return ResponseModel.BadRequest(ResponseConstants.NotFound("Giỏ hàng"));
        }

        var cartItem = cart.CartDetails.FirstOrDefault(x => x.ProductId == productId);
        if (cartItem == null)
        {
            return ResponseModel.BadRequest(
                ResponseConstants.NotFound("Sản phẩm trong giỏ hàng")
            );
        }

        if (model.Quantity > cartItem.Product.Quantity)
        {
            return ResponseModel.BadRequest(ResponseConstants.NotEnoughQuantity);
        }

        cartItem.Quantity = model.Quantity;
        _cartDetailRepository.Update(cartItem);
        // _unitOfWork.Detach(cartItem.Product); // Detach product to prevent update product
        var result = await _unitOfWork.SaveChangesAsync();
        if (result > 0)
        {
            return ResponseModel.Success(
                ResponseConstants.Update("sản phẩm trong giỏ hàng", true),
                null
            );
        }

        return ResponseModel.Error(ResponseConstants.Update("sản phẩm trong giỏ hàng", false));
    }

    public async Task<ResponseModel> ClearCartAsync(Guid customerId)
    {
        var cart = await _cartRepository.GetByCustomerIdAsync(customerId, false);
        if (cart == null)
        {
            return ResponseModel.BadRequest(ResponseConstants.NotFound("Giỏ hàng"));
        }

        _cartDetailRepository.RemoveRange(cart.CartDetails);
        var result = await _unitOfWork.SaveChangesAsync();
        if (result > 0)
        {
            return ResponseModel.Success(ResponseConstants.Delete("giỏ hàng", true), null);
        }

        return ResponseModel.Error(ResponseConstants.Delete("giỏ hàng", false));
    }
}