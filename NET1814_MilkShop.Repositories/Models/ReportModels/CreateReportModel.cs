using System.ComponentModel.DataAnnotations;

namespace NET1814_MilkShop.Repositories.Models.ReportModels;

public class CreateReportModel
{
    [Required(ErrorMessage = "Report Type Id is required")]
    public int ReportTypeId { get; set; }
    
    [Required(ErrorMessage = "Title is required")]
    public string Title { get; set; } = null!;

    public string? Description { get; set; } = "";
}