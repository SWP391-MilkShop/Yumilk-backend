using System.ComponentModel.DataAnnotations;

namespace NET1814_MilkShop.Repositories.Models.OrderModels;

public class OrderQueryModel : QueryModel
{
    [Range(0, int.MaxValue, ErrorMessage = "Total amount must be greater than or equal to 0")]
    public int TotalAmount { get; set; } = 0;

    [EmailAddress(ErrorMessage = "Must be email format")]
    public string? Email { get; set; }

    public bool? IsPreorder { get; set; }
    /// <summary>
    /// Format is mm-dd-yyyy or yyyy-mm-dd
    /// </summary>
    public DateTime? FromOrderDate { get; set; }

    /// <summary>
    /// Format is mm-dd-yyyy or yyyy-mm-dd
    /// </summary>
    public DateTime? ToOrderDate { get; set; }

    public string? PaymentMethod { get; set; }
    public string? OrderStatus { get; set; }

    /// <summary>
    /// sort order asc or desc (default is desc by created at)
    /// </summary>
    public new string? SortOrder { get; set; } = "desc";
}