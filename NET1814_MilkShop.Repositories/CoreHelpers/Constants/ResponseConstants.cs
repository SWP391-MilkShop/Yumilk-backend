namespace NET1814_MilkShop.Repositories.CoreHelpers.Constants
{
    public static class ResponseConstants
    {
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
        /// $"Lấy {name} thành công" : $"Lấy {name} không thành công"
        /// </summary>
        /// <param name="name"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static string Get(string name, bool result)
        {
            return result ? $"Lấy {name} thành công" : $"Lấy {name} không thành công";
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
            return result ? "Đăng ký thành công, vui lòng kiểm tra email để xác thực tài khoản!" : "Đăng ký không thành công";
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
        public const string ResetPasswordLink = "Link đặt lại mật khẩu đã được gửi đến email của bạn";
        public const string ActivateAccountLink = "Link kích hoạt tài khoản đã được gửi đến email của bạn";
        public const string Banned = "Tài khoản của bạn đã bị khóa";
        public const string AccountActivated = "Tài khoản của bạn đã được kích hoạt";
        public const string WrongCode = "Mã xác thực không đúng";
    }
}
