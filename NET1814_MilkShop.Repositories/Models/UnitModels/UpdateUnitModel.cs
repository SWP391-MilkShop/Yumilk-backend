namespace NET1814_MilkShop.Repositories.Models.UnitModels;

public class UpdateUnitModel
{
    public string Name { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    public bool? IsActive { get; set; }
}