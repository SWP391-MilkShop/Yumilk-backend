using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NET1814_MilkShop.Repositories.Models.OrderModels
{
    public class OrderQueryModel : QueryModel
    {
        [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Total amount must be >= 0")]
        public decimal TotalAmount { get; set; } = 0;

        public string? Email { get; set; }

        public DateTime? ToOrderDate { get; set; }

        public string? OrderStatus { get; set; }

    }
}
