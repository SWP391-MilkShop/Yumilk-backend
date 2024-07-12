using System.ComponentModel.DataAnnotations;

namespace NET1814_MilkShop.Repositories.Models.ReportModels;

public class CreateReportModel
{
    [Required(ErrorMessage = "Report Type Id is required")]
    public int ReportTypeId { get; set; }
    
    [Required(ErrorMessage = "Product Id is required")]
    public Guid ProductId { get; set; }
    
}