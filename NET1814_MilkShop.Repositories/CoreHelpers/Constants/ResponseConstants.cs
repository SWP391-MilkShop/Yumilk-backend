namespace NET1814_MilkShop.Repositories.CoreHelpers.Constants;

public static class ResponseConstants
{
    #region Common

    /// <summary>
    /// $"Tạo {name} mới thành công" : $"Tạo {name} mới không thành công"
    /// </summary>
    /// <param name="name"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static string Create(string name, bool result)
    {
        return result ? $"Tạo {name} mới thành công" : $"Tạo {name} mới không thành công";
    }

    /// <summary>
    /// $"Cập nhật {name} thành công" : $"Cập nhật {name} không thành công"
    /// </summary>
    /// <param name="name"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static string Update(string name, bool result)
    {
        return result ? $"Cập nhật {name} thành công" : $"Cập nhật {name} không thành công";
    }

    /// <summary>
    /// $"Xóa {name} thành công" : $"Xóa {name} không thành công"
    /// </summary>
    /// <param name="name"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static string Delete(string name, bool result)
    {
        return result ? $"Xóa {name} thành công" : $"Xóa {name} không thành công";
    }

    /// <summary>
    /// $"Hủy {name} thành công" : $"Hủy {name} không thành công"
    /// </summary>
    /// <param name="name"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static string Cancel(string name, bool result)
    {
        return result ? $"Hủy {name} thành công" : $"Hủy {name} không thành công";
    }

    /// <summary>
    /// $"Lấy {name} thành công" : $"Không tìm thấy {name} phù hợp"
    /// </summary>
    /// <param name="name"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static string Get(string name, bool result)
    {
        return result ? $"Lấy {name} thành công" : $"Không tìm thấy {name} phù hợp";
    }

    /// <summary>
    /// $"{name} không tồn tại"
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string NotFound(string name)
    {
        return $"{name} không tồn tại";
    }

    /// <summary>
    /// $"{name} đã tồn tại"
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string Exist(string name)
    {
        return $"{name} đã tồn tại trong hệ thống";
    }

    /// <summary>
    /// $"{name} không đúng định dạng"
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string WrongFormat(string name)
    {
        return $"{name} không đúng định dạng";
    }

    /// <summary>
    /// $"{name} đã hết hạn"
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string Expired(string name)
    {
        return $"{name} đã hết hạn";
    }

    /// <summary>
    /// $"{name} đã vượt quá giới hạn"
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static string OverLimit(string name)
    {
        return $"{name} đã vượt quá giới hạn";
    }

    public const string NoChangeIsMade = "Không có thay đổi nào được thực hiện";
    public const string NotEnoughPermission = "Bạn không có quyền thực hiện thao tác này";

    #endregion

    #region Authen & Author

    /// <summary>
    /// "Đăng nhập thành công" : "Tên đăng nhập hoặc mật khẩu không đúng"
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    public static string Login(bool result)
    {
        return result ? "Đăng nhập thành công" : "Tên đăng nhập hoặc mật khẩu không đúng";
    }

    /// <summary>
    /// "Đăng ký thành công" : "Đăng ký không thành công"
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    public static string Register(bool result)
    {
        return result
            ? "Đăng ký thành công, vui lòng kiểm tra email để xác thực tài khoản!"
            : "Đăng ký không thành công";
    }

    /// <summary>
    /// "Đổi mật khẩu thành công" : "Đổi mật khẩu không thành công"
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    public static string ChangePassword(bool result)
    {
        return result ? "Đổi mật khẩu thành công" : "Đổi mật khẩu không thành công";
    }

    /// <summary>
    /// "Thay đổi thông tin thành công" : "Thay đổi thông tin không thành công"
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    public static string ChangeInfo(bool result)
    {
        return result ? "Thay đổi thông tin thành công" : "Thay đổi thông tin không thành công";
    }

    /// <summary>
    /// "Xác thực thành công" : "Xác thực không thành công"
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    public static string Verify(bool result)
    {
        return result ? "Xác thực thành công" : "Xác thực không thành công";
    }

    public const string ResetPasswordLink =
        "Link đặt lại mật khẩu đã được gửi đến email của bạn";

    public const string ActivateAccountLink =
        "Link kích hoạt tài khoản đã được gửi đến email của bạn";

    public const string Banned = "Tài khoản của bạn đã bị khóa";
    public const string AccountActivated = "Tài khoản của bạn đã được kích hoạt";
    public const string WrongCode = "Mã xác thực không đúng";
    public const string SamePassword = "Mật khẩu cũ và mật khẩu mới không được trùng nhau";
    public const string WrongPassword = "Mật khẩu không chính xác";
    public const string InvalidPhoneNumber = "Số điện thoại không hợp lệ";
    public const string SameUsername = "Tên đăng nhập mới không được trùng với tên đăng nhập hiện tại";

    #endregion

    /// <summary>
    /// $"Tải {name} lên thành công" : $"Tải {name} lên không thành công"
    /// </summary>
    /// <param name="name"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static string Upload(string name, bool result)
    {
        return result ? $"Tải {name} lên thành công" : $"Tải {name} lên không thành công";
    }

    public const string InvalidUrl = "Url không hợp lệ";
    public const string InvalidFilterDate = "Ngày bắt đầu phải trước hoặc trùng với ngày kết thúc";
    public const string InvalidFromDate = "Ngày bắt đầu không thể lớn hơn ngày hiện tại";
    public const string InvalidExpectedPreOrderDays = "Số ngày dự kiến phải lớn hơn 0";

    public const string UserNotActive =
        "Tài khoản của bạn chưa được kích hoạt. Vui lòng kích hoạt tài khoản để tiến hành thanh toán";

    #region Review

    public const string ReviewConstraint =
        "Bạn không thể đánh giá sản phẩm khi chưa mua hàng hoặc đơn hàng chưa được giao";

    public const string ReviewPerOrder = "Bạn chỉ được đánh giá sản phẩm một lần trên mỗi đơn hàng";

    #endregion

    #region Product

    public const string OutOfStock = "Sản phẩm đã hết hàng";
    public const string NotEnoughQuantity = "Số lượng sản phẩm không đủ";
    public const string InvalidSalePrice = "Giá khuyến mãi phải nhỏ hơn giá gốc";
    public const string InvalidQuantity = "Số lượng sản phẩm phải lớn hơn 0";
    public const string NoQuantityPreorder = "Không thể set số lượng tồn cho sản phẩm Pre-order";
    public const string NotInPreOrder = "Sản phẩm đang không trong quá trình Pre-order";
    public const string ProductOrdered = "Sản phẩm đang trong quá trình đặt hàng";
    public const string InvalidMaxPreOrderQuantity = "Số lượng đặt hàng tối đa phải lớn hơn 0";
    public const string DeleteOrderedProduct = "Không thể xóa sản phẩm đã được đặt hàng";

    #endregion

    #region Cart

    /// <summary>
    /// "Thêm sản phẩm vào giỏ hàng thành công" : "Thêm sản phẩm vào giỏ hàng không thành công"
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    public static string AddToCart(bool result)
    {
        return result
            ? "Thêm sản phẩm vào giỏ hàng thành công"
            : "Thêm sản phẩm vào giỏ hàng không thành công";
    }

    public const string NoPreorderCart = "Không thể thêm sản phẩm Pre-order vào giỏ hàng";

    public const string CartIsEmpty = "Giỏ hàng của bạn hiện tại đang rỗng!";

    #endregion
}