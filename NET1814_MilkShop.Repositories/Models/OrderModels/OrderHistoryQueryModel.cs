using System.ComponentModel.DataAnnotations;

namespace NET1814_MilkShop.Repositories.Models.OrderModels;

public class OrderHistoryQueryModel : QueryModel
{
    [Range(0, int.MaxValue, ErrorMessage = "Total amount must be greater than or equal to 0")] // "Total amount must be greater than or equal to 0
    public int TotalAmount { get; set; } = 0;

    public DateTime? FromOrderDate { get; set; }
    public DateTime? ToOrderDate { get; set; }

    /// <summary>
    /// order status theo id từ 1-5
    /// </summary>
    [Range(1, 5, ErrorMessage = "Status Id must be in range 1-5")]
    public int? OrderStatus { get; set; }

    /// <summary>
    /// search theo tên sản phẩm trong order history
    /// </summary>
    public new string? SearchTerm { get; set; }
}