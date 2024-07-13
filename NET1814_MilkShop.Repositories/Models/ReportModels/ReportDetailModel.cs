using NET1814_MilkShop.Repositories.Data.Entities;

namespace NET1814_MilkShop.Repositories.Models.ReportModels;

public class ReportDetailModel
{
    public Guid Id { get; set; }
    
    public int ReportTypeId { get; set; }
    
    public string ReportTypeName { get; set; } = null!;
    
    public string? ReportTypeDescription { get; set; }
    public Guid CustomerId { get; set; }
    
    public DateTime? ResolvedAt { get; set; }
    
    public Guid ResolvedBy { get; set; }

    public string ResolverName { get; set; } = "";
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime? ModifiedAt { get; set; }
    
    public Guid ProductId { get; set; }
    
    public string ProductName { get; set; } = null!;
}