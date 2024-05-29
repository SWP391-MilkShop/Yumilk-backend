using NET1814_MilkShop.Repositories.Models.BrandModels;

namespace NET1814_MilkShop.Repositories.CoreHelpers.Constants
{
    public class ResponseConstants
    {
        public static string Create(string name, bool result)
        {
            return result ? $"Tạo {name} thành công" : $"Tạo {name} không thành công";
        }
        public static string Update(string name, bool result)
        {
            return result ? $"Cập nhật {name} thành công" : $"Cập nhật {name} không thành công";
        }
        public static string Delete(string name, bool result)
        {
            return result ? $"Xóa {name} thành công" : $"Xóa {name} không thành công";
        }
        public static string Get(string name, bool result)
        {
            return result ? $"Lấy {name} thành công" : $"Lấy {name} không thành công";
        }
        public static string NotFound(string name)
        {
            return $"{name} không tồn tại";
        }
        public static string Login(bool result)
        {
            return result ? "Đăng nhập thành công" : "Đăng nhập không thành công";
        }
        public static string Register(bool result)
        {
            return result ? "Đăng ký thành công" : "Đăng ký không thành công";
        }
        public static string ChangePassword(bool result)
        {
            return result ? "Đổi mật khẩu thành công" : "Đổi mật khẩu không thành công";
        }
        public static string ChangeInfo(bool result)
        {
            return result ? "Thay đổi thông tin thành công" : "Thay đổi thông tin không thành công";
        }
        public static string Verify(bool result)
        {
            return result ? "Xác thực thành công" : "Xác thực không thành công";
        }
        public static string Exist(string name)
        {
            return $"{name} đã tồn tại";
        }
        public static string WrongFormat(string name)
        {
            return $"{name} không đúng định dạng";
        }
    }
}
