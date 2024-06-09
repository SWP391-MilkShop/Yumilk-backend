using Quartz;
using Microsoft.Extensions.Logging;
using Net.payOS.Types;
using NET1814_MilkShop.Repositories.Repositories;
using NET1814_MilkShop.Repositories.UnitOfWork;
using NET1814_MilkShop.Services.Services;

namespace NET1814_MilkShop.Services.BackgroundJobs;
[DisallowConcurrentExecution] //Nếu chưa ra kết quả trong khoảng thời gian đưa
                              //thì chờ cho đến khi Job trước đó hoàn thành
public class CheckPaymentStatusJob : IJob
{
    private readonly ILogger<CheckPaymentStatusJob> _logger;
    private readonly IPaymentService _paymentService;
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CheckPaymentStatusJob(ILogger<CheckPaymentStatusJob> logger,
        IPaymentService paymentService,
        IProductRepository productRepository,
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _paymentService = paymentService;
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("{UtcNow} - Check payment status job is running", DateTime.Now);
        var orders = await _orderRepository.GetAllCodeAsync();
        
        if (orders == null)
        {
            _logger.LogInformation("No order found");
            return;
        }

        foreach (var order in orders)
        {
            try
            {
                if (order.OrderCode == null)
                {
                    _logger.LogInformation("Order code is null for order {OrderId}", order.Id);
                    continue;
                }

                if (order.StatusId == 5)
                {
                    _logger.LogInformation("OrderId {OrderId} code {OrderCode} is already cancelled and updated in product quantity",
                        order.Id,order.OrderCode.Value);
                    continue;
                }
                
                //Gọi API lấy payment status của PayOS
                await Task.Delay(300); //Tranh request qua nhieu trong thoi gian ngan tranh bi block
                var paymentStatus = await _paymentService.GetPaymentLinkInformation(order.OrderCode.Value);
                _logger.LogInformation($"OrderId:{order.Id.ToString()} code:{order.OrderCode.Value} --> " + paymentStatus.Message);
                if (paymentStatus.StatusCode == 500)
                {
                    continue;
                }
                
                _logger.LogInformation("Type of paymentStatus.Data: {Type}", paymentStatus.Data.GetType());
                _logger.LogInformation("paymentStatus.Data: {Data}", paymentStatus.Data);
                
                var paymentData = paymentStatus.Data as PaymentLinkInformation;
                if (!"CANCELLED".Equals(paymentData.status) && !"EXPIRED".Equals(paymentData.status)) continue;
                
                _logger.LogInformation("Payment for order {OrderId} is cancelled or expired", order.Id);
                
                foreach (var orderDetail in order.OrderDetails)
                {
                    var product = await _productRepository.GetIdNoIncludeAsync(orderDetail.ProductId);
                    product.Quantity += orderDetail.Quantity;
                    orderDetail.Product.Quantity = product.Quantity;
                }   
                order.StatusId = 5; // Cancelled
                _orderRepository.Update(order);
                var result = await _unitOfWork.SaveChangesAsync();
                foreach (var orderDetail in order.OrderDetails)
                {
                    _unitOfWork.Detach(orderDetail.Product);
                }
                
                _logger.LogInformation(
                    result > 0
                        ? "Update order status for order {OrderId} successfully"
                        : "Update order status for order {OrderId} failed", order.Id);
                
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
        }
    }
}