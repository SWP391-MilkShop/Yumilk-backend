using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NET1814_MilkShop.Repositories.Models
{
    public class ForgotPasswordModel
    {
        [
        Required(ErrorMessage = "Email is required!"),
        EmailAddress(ErrorMessage = "Must be email format!")
        ]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = null!;
    }
}
