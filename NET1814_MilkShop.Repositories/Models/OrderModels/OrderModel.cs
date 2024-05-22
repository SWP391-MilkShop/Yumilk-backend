using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NET1814_MilkShop.Repositories.Models.OrderModels
{
    public class OrderModel
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public decimal TotalAmount { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }

        public string? OrderStatus { get; set; }

        public DateTime? CreatedDate { get; set; }
        public DateTime? PaymentDate { get; set; }

    }
}
