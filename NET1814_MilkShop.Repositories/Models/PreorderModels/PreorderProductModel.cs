using System.ComponentModel.DataAnnotations;
using NET1814_MilkShop.Repositories.Models.ProductModels;

namespace NET1814_MilkShop.Repositories.Models.PreorderModels;

public class PreorderProductModel : ProductModel
{
    [Range(1, int.MaxValue, ErrorMessage = "MaxPreOrderQuantity must be greater than 0")]
    public int MaxPreOrderQuantity { get; set; }

    [Required(ErrorMessage = "StartDate is required")]
    public DateTime StartDate { get; set; }

    [Required(ErrorMessage = "EndDate is required")]
    public DateTime EndDate { get; set; }

    /// <summary>
    /// So ngay du kien nhap hang toi da la 30 ngay
    /// </summary>
    [Range(1, 30, ErrorMessage = "ExpectedPreOrderDays must be range 1-30")]
    public int ExpectedPreOrderDays { get; set; }
}