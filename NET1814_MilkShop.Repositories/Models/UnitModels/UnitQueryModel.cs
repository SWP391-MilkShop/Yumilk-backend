namespace NET1814_MilkShop.Repositories.Models.UnitModels;

public class UnitQueryModel : QueryModel
{
    //SearchTerm se search theo Name, Description
    public bool? IsActive { get; set; }
    /// <summary>
    /// Sort by id, name, description (default is id)
    /// </summary>
    public new string? SortColumn { get; set; }
}
