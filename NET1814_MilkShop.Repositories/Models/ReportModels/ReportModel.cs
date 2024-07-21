namespace NET1814_MilkShop.Repositories.Models.ReportModels;

public class ReportModel
{
    public Guid Id { get; set; }
    
    public int ReportTypeId { get; set; }
    
    public string ReportTypeName { get; set; } = null!;
    
    public Guid CustomerId { get; set; }
    
    public Guid ProductId { get; set; }

    public string ProductName { get; set; } = null!;
    
    public DateTime? ResolvedAt { get; set; }
    
    public Guid ResolvedBy { get; set; }
    
    public DateTime CreatedAt { get; set; }
}