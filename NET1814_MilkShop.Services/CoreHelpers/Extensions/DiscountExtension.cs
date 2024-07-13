using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Models;

namespace NET1814_MilkShop.Services.CoreHelpers.Extensions;

public static class DiscountExtension
{
    #region Discount

    /// <summary>
    /// Số point sử dụng = discount price
    /// </summary>
    /// <param name="customer"></param>
    /// <param name="totalPrice"></param>
    /// <returns></returns>
    public static ResponseModel ApplyPoint(Customer customer, int totalPrice)
    {
        if (customer.Point < 100)
        {
            return ResponseModel.BadRequest("Số xu không đủ để sử dụng. Tối thiểu 100 xu để sử dụng");
        }

        var discountPrice = customer.Point;
        var halfPrice = totalPrice.ApplyPercentage(50);
        if (customer.Point > halfPrice)
        {
            discountPrice = halfPrice;
        }

        return ResponseModel.Success("Áp dụng xu thành công", discountPrice);
    }

    public static ResponseModel ApplyVoucher(Voucher voucher, int totalPrice)
    {
        if (voucher.StartDate > DateTime.UtcNow || !voucher.IsActive)
        {
            return ResponseModel.BadRequest("Voucher chưa được kích hoạt");
        }

        if (voucher.EndDate < DateTime.UtcNow)
        {
            return ResponseModel.BadRequest("Voucher đã hết hạn");
        }

        if (voucher.Quantity == 0)
        {
            return ResponseModel.BadRequest("Voucher đã hết lượt sử dụng");
        }

        if (totalPrice < voucher.MinPriceCondition)
        {
            return ResponseModel.BadRequest("Đơn hàng không đủ điều kiện sử dụng voucher");
        }

        var discountPrice = totalPrice.ApplyPercentage(voucher.Percent);
        if (discountPrice > voucher.MaxDiscount && voucher.MaxDiscount != 0)
        {
            discountPrice = voucher.MaxDiscount;
        }

        return ResponseModel.Success("Áp dụng voucher thành công", discountPrice);
    }

    #endregion
}