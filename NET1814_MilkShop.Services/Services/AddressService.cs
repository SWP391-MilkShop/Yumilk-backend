using Microsoft.EntityFrameworkCore;
using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Models.AddressModels;
using NET1814_MilkShop.Repositories.Repositories;
using NET1814_MilkShop.Repositories.UnitOfWork;

namespace NET1814_MilkShop.Services.Services;

public interface IAddressService
{
    Task<ResponseModel> CreateAddressAsync(Guid customerId, CreateAddressModel model);
    Task<ResponseModel> GetAddressesByCustomerId(Guid customerId);
    Task<ResponseModel> UpdateAddressAsync(Guid customerId, int id, UpdateAddressModel model);
    Task<ResponseModel> DeleteAddressAsync(Guid customerId, int id);
}

public class AddressService : IAddressService
{
    private readonly IAddressRepository _addressRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddressService(IAddressRepository addressRepository, IUnitOfWork unitOfWork)
    {
        _addressRepository = addressRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseModel> CreateAddressAsync(Guid customerId, CreateAddressModel model)
    {
        var countCustomerAddress = _addressRepository.GetCustomerAddresses(customerId).Count();
        if (countCustomerAddress >= 3)
            return new ResponseModel
            {
                Status = "Error",
                Message = "Không được tạo quá 3 địa chỉ"
            };
        var existAnyAddress = await _addressRepository.ExistAnyAddress(customerId);
        if (existAnyAddress == false) //Dia chi dau tien gan lam mac dinh
            model.IsDefault = true;
        if (model.IsDefault && existAnyAddress)
        {
            var getDefaultAddress = await _addressRepository.GetByDefault(customerId);
            if (getDefaultAddress != null)
            {
                getDefaultAddress.IsDefault = false;
                _addressRepository.Update(getDefaultAddress);
            }
        }

        var address = new CustomerAddress
        {
            ReceiverName = model.ReceiverName,
            PhoneNumber = model.ReceiverPhone,
            Address = model.Address,
            WardCode = model.WardId,
            WardName = model.WardName,
            DistrictId = model.DistrictId,
            DistrictName = model.DistrictName,
            ProvinceName = model.ProvinceName,
            ProvinceId = model.ProvinceId,
            UserId = customerId,
            IsDefault = model.IsDefault
        };
        _addressRepository.Add(address);
        var result = await _unitOfWork.SaveChangesAsync();
        if (result > 0)
            return new ResponseModel
            {
                Data = model,
                Message = "Tạo địa chỉ mới thành công",
                Status = "Success"
            };
        return new ResponseModel
        {
            Status = "Error",
            Message = "An error occurred while creating address"
        };
    }

    public async Task<ResponseModel> GetAddressesByCustomerId(Guid customerId)
    {
        var customerAddresses = _addressRepository.GetCustomerAddresses(customerId);
        var result = customerAddresses.Select(customerAddress => new AddressModel
        {
            Id = customerAddress.Id,
            ReceiverName = customerAddress.ReceiverName,
            ReceiverPhone = customerAddress.PhoneNumber,
            Address = customerAddress.Address,
            WardName = customerAddress.WardName,
            DistrictName = customerAddress.DistrictName,
            ProvinceName = customerAddress.ProvinceName,
            IsDefault = customerAddress.IsDefault
        });
        return new ResponseModel
        {
            Status = "Success",
            Message = "Get addresses successfully",
            Data = result
        };
    }

    public async Task<ResponseModel> UpdateAddressAsync(Guid customerId, int id, UpdateAddressModel model)
    {
        var customerAddresses = _addressRepository.GetCustomerAddresses(customerId);
        var address = await customerAddresses.FirstOrDefaultAsync(x => x.Id == id);
        if (address == null)
            return new ResponseModel
            {
                Status = "Error",
                Message = "Không tìm thấy địa chỉ"
            };
        var countCustomerAddress = _addressRepository.GetCustomerAddresses(customerId).Count();
        if (countCustomerAddress == 1) model.IsDefault = true;
        if (model.IsDefault && countCustomerAddress > 1)
        {
            var getDefaultAddress = await _addressRepository.GetByDefault(customerId);
            if (getDefaultAddress != null)
            {
                getDefaultAddress.IsDefault = false;
                _addressRepository.Update(getDefaultAddress);
            }
        }

        address.ReceiverName = model.ReceiverName;
        address.PhoneNumber = model.ReceiverPhone;
        address.Address = model.Address;
        address.WardCode = model.WardId;
        address.WardName = model.WardName;
        address.DistrictId = model.DistrictId;
        address.DistrictName = model.DistrictName;
        address.ProvinceName = model.ProvinceName;
        address.ProvinceId = model.ProvinceId;
        address.IsDefault = model.IsDefault;
        _addressRepository.Update(address);
        var result = await _unitOfWork.SaveChangesAsync();
        if (result > 0)
            return new ResponseModel
            {
                Data = model,
                Message = "Thay đổi địa chỉ thành công",
                Status = "Success"
            };
        return new ResponseModel
        {
            Status = "Error",
            Message = "An error occurred while updating address"
        };
    }

    public async Task<ResponseModel> DeleteAddressAsync(Guid customerId, int id)
    {
        var customerAddresses = _addressRepository.GetCustomerAddresses(customerId);
        var address = await customerAddresses.FirstOrDefaultAsync(x => x.Id == id);
        switch (address)
        {
            case null:
                return new ResponseModel
                {
                    Status = "Error",
                    Message = "Không tìm thấy địa chỉ"
                };
            case { IsDefault: true }:
                return new ResponseModel
                {
                    Status = "Error",
                    Message = "Không thể xóa địa chỉ mặc định"
                };
            default:
                address.DeletedAt = DateTime.Now;
                _addressRepository.Update(address);
                var result = await _unitOfWork.SaveChangesAsync();
                if (result > 0)
                    return new ResponseModel
                    {
                        Status = "Success",
                        Message = "Xóa địa chỉ thành công"
                    };
                break;
        }

        return new ResponseModel
        {
            Status = "Error",
            Message = "An error occurred while deleting address"
        };
    }
}