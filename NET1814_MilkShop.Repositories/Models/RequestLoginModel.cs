using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NET1814_MilkShop.Repositories.Models
{
    public class RequestLoginModel
    {
        [Required(ErrorMessage = "UserName là bắt buộc")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password là bắt buộc")]
        public string Password { get; set; }
    }
}
