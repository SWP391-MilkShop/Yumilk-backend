using System.ComponentModel.DataAnnotations;

namespace NET1814_MilkShop.Repositories.Models.OrderModels
{
    public class OrderQueryModel : QueryModel
    {
        [Range(
            typeof(decimal),
            "0",
            "79228162514264337593543950335",
            ErrorMessage = "Total amount must be >= 0"
        )]
        public decimal TotalAmount { get; set; } = 0;

        public string? Email { get; set; }

        public DateTime? FromOrderDate { get; set; }
        public DateTime? ToOrderDate { get; set; }
        public string? PaymentMethod { get; set; }
        public string? OrderStatus { get; set; }
    }
}