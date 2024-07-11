using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Models.UnitModels;

namespace NET1814_MilkShop.Services.Services.Interfaces;

public interface IUnitService
{
    Task<ResponseModel> GetUnitsAsync(UnitQueryModel request);
    Task<ResponseModel> GetUnitByIdAsync(int id);
    Task<ResponseModel> CreateUnitAsync(CreateUnitModel createUnitModel);
    Task<ResponseModel> UpdateUnitAsync(int id, UpdateUnitModel unitModel);
    Task<ResponseModel> DeleteUnitAsync(int id);
}