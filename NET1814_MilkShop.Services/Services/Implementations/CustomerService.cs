using System.Linq.Expressions;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using NET1814_MilkShop.Repositories.CoreHelpers.Constants;
using NET1814_MilkShop.Repositories.CoreHelpers.Enum;
using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Models.UserModels;
using NET1814_MilkShop.Repositories.Repositories.Interfaces;
using NET1814_MilkShop.Repositories.UnitOfWork.Interfaces;
using NET1814_MilkShop.Services.CoreHelpers;
using NET1814_MilkShop.Services.CoreHelpers.Extensions;
using NET1814_MilkShop.Services.Services.Interfaces;

namespace NET1814_MilkShop.Services.Services.Implementations;

public sealed class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOrderRepository _orderRepository;

    public CustomerService(ICustomerRepository customerRepository, IUnitOfWork unitOfWork,
        IOrderRepository orderRepository)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
        _orderRepository = orderRepository;
    }

    private static CustomerModel ToCustomerModel(Customer customer, User user)
    {
        return new CustomerModel
        {
            UserID = customer.UserId.ToString(),
            Username = user.Username,
            FirstName = user.FirstName,
            LastName = user.LastName,
            GoogleId = customer.GoogleId,
            PhoneNumber = customer.PhoneNumber,
            Email = customer.Email,
            Role = user.Role!.Name,
            ProfilePictureUrl = customer.ProfilePictureUrl,
            IsActive = user.IsActive,
            IsBanned = user.IsBanned,
            Point = customer.Point
        };
    }

    public async Task<ResponseModel> GetCustomersAsync(CustomerQueryModel request)
    {
        var query = _customerRepository.GetCustomersQuery()
            .Include(x => x.User)
            .ThenInclude(x => x.Role).AsQueryable();
        var searchTerm = StringExtension.Normalize(request.SearchTerm);
        //filter
        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(c =>
                c.User.Username.ToLower().Contains(searchTerm)
                || c.Email!.Contains(searchTerm)
                || c.PhoneNumber!.Contains(searchTerm)
                || c.User.FirstName != null && c.User.FirstName.Contains(searchTerm)
                || c.User.LastName != null && c.User.LastName.Contains(searchTerm)
            );
        }

        if (request.IsActive.HasValue || request.IsBanned.HasValue)
        {
            query = query.Where(c =>
                (!request.IsActive.HasValue || c.User.IsActive == request.IsActive.Value)
                && (!request.IsBanned.HasValue || c.User.IsBanned == request.IsBanned.Value)
            );
        }

        //sort
        query = "desc".Equals(request.SortOrder?.ToLower())
            ? query.OrderByDescending(GetSortProperty(request))
            : query.OrderBy(GetSortProperty(request));
        var result = query.Select(c => ToCustomerModel(c, c.User));
        var customers = await PagedList<CustomerModel>.CreateAsync(
            result,
            request.Page,
            request.PageSize
        );
        return ResponseModel.Success(
            ResponseConstants.Get("khách hàng", customers.TotalCount > 0),
            customers
        );
    }

    private static Expression<Func<Customer, object>> GetSortProperty(
        CustomerQueryModel request
    )
    {
        Expression<Func<Customer, object>> keySelector = request.SortColumn?.ToLower().Replace(" ", "") switch
        {
            "email" => customer => customer.Email!,
            "isactive" => customer => customer.User.IsActive,
            "firstname" => customer => customer.User.FirstName!,
            "lastname" => customer => customer.User.LastName!,
            "createdat" => customer => customer.User.CreatedAt,
            _ => customer => customer.UserId
        };
        return keySelector;
    }

    public async Task<ResponseModel> GetByEmailAsync(string email)
    {
        var customer = await _customerRepository.GetByEmailAsync(email);
        if (customer == null)
        {
            /*return new ResponseModel
            {
                Data = null,
                Message = "Customer not found",
                Status = "Error"
            };*/
            return ResponseModel.Success(
                ResponseConstants.Get("khách hàng bằng email", false),
                null
            );
        }

        var customerModel = ToCustomerModel(customer, customer.User);
        /*return new ResponseModel
        {
            Data = customerModel,
            Message = "Get customer by email successfully",
            Status = "Success"
        };*/
        return ResponseModel.Success(
            ResponseConstants.Get("khách hàng bằng email", true),
            customerModel
        );
    }

    public async Task<ResponseModel> GetByIdAsync(Guid id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer == null)
        {
            /*return new ResponseModel { Message = "Customer not found", Status = "Error" };*/
            return ResponseModel.Success(ResponseConstants.Get("khách hàng", false), null);
        }

        var customerModel = ToCustomerModel(customer, customer.User); /*
        return new ResponseModel
        {
            Data = customerModel,
            Message = "Get customer by id successfully",
            Status = "Success"
        };*/
        return ResponseModel.Success(ResponseConstants.Get("khách hàng", true), customerModel);
    }

    public async Task<ResponseModel> ChangeInfoAsync(
        Guid userId,
        ChangeUserInfoModel changeUserInfoModel
    )
    {
        var customer = await _customerRepository.GetByIdAsync(userId);
        if (customer == null)
        {
            /*return new ResponseModel { Message = "Customer not found", Status = "Error" };*/
            return ResponseModel.Success(ResponseConstants.Get("khách hàng", false), null);
        }

        if (!string.IsNullOrWhiteSpace(changeUserInfoModel.PhoneNumber))
        {
            if (!Regex.IsMatch(changeUserInfoModel.PhoneNumber, @"^([0-9]{10})$"))
            {
                /*return new ResponseModel
                {
                    Message = "Invalid Phone Number!",
                    Status = "Error"
                };*/
                return ResponseModel.BadRequest(ResponseConstants.InvalidPhoneNumber);
            }

            if (!customer.PhoneNumber.Equals(changeUserInfoModel.PhoneNumber))
            {
                if (await _customerRepository.IsExistPhoneNumberAsync(changeUserInfoModel.PhoneNumber))
                {
                    return ResponseModel.BadRequest("Số điện thoại đã tồn tại trong hệ thống!");
                }

                customer.PhoneNumber = changeUserInfoModel.PhoneNumber;
            }
        }

        if (!string.IsNullOrWhiteSpace(changeUserInfoModel.ProfilePictureUrl))
        {
            if (
                !Uri.IsWellFormedUriString(
                    changeUserInfoModel.ProfilePictureUrl,
                    UriKind.Absolute
                )
            )
            {
                /*return new ResponseModel { Message = "Invalid URL!", Status = "Error" };*/
                return ResponseModel.BadRequest(ResponseConstants.InvalidUrl);
            }

            customer.ProfilePictureUrl = changeUserInfoModel.ProfilePictureUrl;
        }

        if (!string.IsNullOrWhiteSpace(changeUserInfoModel.FirstName))
        {
            customer.User.FirstName = changeUserInfoModel.FirstName;
        }

        if (!string.IsNullOrWhiteSpace(changeUserInfoModel.LastName))
        {
            customer.User.LastName = changeUserInfoModel.LastName;
        }

        _customerRepository.Update(customer);
        var result = await _unitOfWork.SaveChangesAsync();
        if (result > 0)
        {
            /*return new ResponseModel
            {
                Data = ToCustomerModel(customer, customer.User),
                Message = "Change user info successfully",
                Status = "Success"
            };*/
            return ResponseModel.Success(
                ResponseConstants.ChangeInfo(true),
                ToCustomerModel(customer, customer.User)
            );
        }

        /*return new ResponseModel { Message = "Change user info failed", Status = "Error" };*/
        return ResponseModel.Error(ResponseConstants.ChangeInfo(false));
    }

    public async Task<bool> IsExistAsync(Guid id)
    {
        return await _customerRepository.IsExistAsync(id);
    }

    public async Task<bool> IsExistPhoneNumberAsync(string phoneNumber)
    {
        return await _customerRepository.IsExistPhoneNumberAsync(phoneNumber);
    }

    public async Task<bool> IsExistEmailAsync(string email)
    {
        return await _customerRepository.IsExistEmailAsync(email);
    }

    public async Task<ResponseModel> GetReturnCustomerStatsAsync(int year)
    {
        // get all orders that have been delivered in year
        var orders = _orderRepository.GetOrderQuery()
            .Where(x => x.CreatedAt.Year == year && x.StatusId == (int)OrderStatusId.Delivered);
        var query = from firstorder in orders
            join secondorder in orders on firstorder.CustomerId equals secondorder.CustomerId
            where firstorder.CreatedAt.Date != secondorder.CreatedAt.Date
            // Tính toán quý dựa trên tháng của CreatedAt
            group new { firstorder.CreatedAt, firstorder.CustomerId } by new
                { Quarter = (firstorder.CreatedAt.Month - 1) / 3 + 1 }
            into g
            select new
            {
                g.Key.Quarter, // Quý 3 tháng (4 quý)
                DistinctCustomerCount =
                    g.Select(x => x.CustomerId).Distinct()
                        .Count() // Số lượng khách hàng quay trở lại mua hàng trong mỗi quý
            };
        var result = await query.ToListAsync();
        return ResponseModel.Success(
            ResponseConstants.Get("khách hàng trở lại", result.Count > 0),
            result
        );
    }

    public async Task<ResponseModel> GetTotalPurchaseAsync()
    {
        var orders = _orderRepository.GetOrderQuery();
        var query = await orders.Where(x => x.StatusId == (int)OrderStatusId.Delivered)
            .GroupBy(x => x.CustomerId)
            .Select(x => new
            {
                CustomerId = x.Key,
                TotalPurchase = x.Count(),
                TotalRevenue = (long)x.Sum(x => x.TotalPrice)
            }).OrderByDescending(x => x.TotalRevenue).ToListAsync();
        return ResponseModel.Success(ResponseConstants.Get("doanh thu cao nhất của khách hàng đã đặt hàng", true),
            query);
    }

    public async Task<ResponseModel> GetTotalPurchaseByCustomerAsync(Guid id, int year)
    {
        var isExistCustomer = await _customerRepository.IsExistAsync(id);
        if (!isExistCustomer)
        {
            return ResponseModel.BadRequest(ResponseConstants.NotFound("Khách hàng"));
        }

        var orders = _orderRepository.GetOrderQuery();
        var query = await orders.Where(x =>
                x.CustomerId == id && x.StatusId == (int)OrderStatusId.Delivered && x.CreatedAt.Year == year)
            .GroupBy(x => x.CustomerId)
            .Select(x => new
            {
                CustomerId = x.Key,
                TotalPurchase = x.Count(),
                TotalRevenue = x.Sum(o => o.TotalPrice)
            }).ToListAsync();
        return ResponseModel.Success(ResponseConstants.Get("doanh thu của khách hàng", true), query);
    }


    /*public async Task<bool> IsCustomerExistAsync(string email, string phoneNumber)
    {
        return await _customerRepository.IsCustomerExistAsync(email, phoneNumber);
    }*/
    public async Task<ResponseModel> GetCustomersStatsAsync(CustomersStatsQueryModel model)
    {
        if (model.From > DateTime.Now || model.From > model.To)
        {
            return ResponseModel.BadRequest(ResponseConstants.InvalidFilterDate);
        }

        var query = _customerRepository.GetCustomersQuery()
            .Include(c => c.Orders).AsQueryable();
        // default is from last 30 days
        var from = model.From ?? DateTime.Now.AddDays(-30);
        // default is now
        var to = model.To ?? DateTime.Now;
        var totalCustomerBought = await query.Where(c => c.Orders.Any(o =>
            o.CreatedAt >= from && o.CreatedAt <= to
                                && o.StatusId == (int)OrderStatusId.Delivered)).CountAsync();
        var totalCustomer = await query.Where(c => c.CreatedAt >= from && c.CreatedAt <= to).CountAsync();
        var resp = new CustomerStatsModel
        {
            TotalNewCustomers = totalCustomer,
            TotalCustomerBought = totalCustomerBought
        };
        return ResponseModel.Success(ResponseConstants.Get("thống kê người dùng", true), resp);
    }
}