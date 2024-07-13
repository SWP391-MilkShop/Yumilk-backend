using System.ComponentModel.DataAnnotations;

namespace NET1814_MilkShop.Repositories.Models.ReportTypeModels;

public class CreateReportTypeModel
{
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}