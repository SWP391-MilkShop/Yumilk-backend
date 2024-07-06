using Microsoft.Extensions.Logging;
using Net.payOS.Types;
using NET1814_MilkShop.Repositories.CoreHelpers.Enum;
using NET1814_MilkShop.Repositories.Repositories.Interfaces;
using NET1814_MilkShop.Repositories.UnitOfWork.Interfaces;
using NET1814_MilkShop.Services.Services.Interfaces;
using Quartz;

namespace NET1814_MilkShop.Services.BackgroundJobs;

[DisallowConcurrentExecution] //Nếu chưa ra kết quả trong khoảng thời gian đưa
//thì chờ cho đến khi Job trước đó hoàn thành
public class CheckPaymentStatusJob : IJob
{
    private readonly ILogger<CheckPaymentStatusJob> _logger;
    private readonly IPaymentService _paymentService;
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IShippingService _shippingService;
    private readonly IUnitOfWork _unitOfWork;

    public CheckPaymentStatusJob(ILogger<CheckPaymentStatusJob> logger,
        IPaymentService paymentService,
        IShippingService shippingService,
        IProductRepository productRepository,
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _paymentService = paymentService;
        _shippingService = shippingService;
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

                switch (order.StatusId)
                {
                    case (int)OrderStatusId.Cancelled:
                        _logger.LogInformation(
                            "OrderId {OrderId} code {OrderCode} is already cancelled and updated to {cancelled}",
                            order.Id, order.OrderCode.Value, OrderStatusId.Cancelled.ToString());
                        continue;
                    case (int)OrderStatusId.Processing:
                        _logger.LogInformation(
                            "OrderId {OrderId} code {OrderCode} is already paid and updated to {processing}",
                            order.Id, order.OrderCode.Value, OrderStatusId.Processing.ToString());
                        continue;
                    case (int)OrderStatusId.Shipping:
                        _logger.LogInformation(
                            "OrderId {OrderId} code {OrderCode} is in {shipping} status",
                            order.Id, order.OrderCode.Value, OrderStatusId.Shipping.ToString());
                        continue;
                    case (int)OrderStatusId.Preorder:
                        _logger.LogInformation(
                            "OrderId {OrderId} code {OrderCode} is in {preorder} status",
                            order.Id, order.OrderCode.Value, OrderStatusId.Preorder.ToString());
                        continue;
                }

                //Gọi API lấy payment status của PayOS
                await Task.Delay(300); //Tranh request qua nhieu trong thoi gian ngan tranh bi block
                var paymentStatus = await _paymentService.GetPaymentLinkInformation(order.Id);
                _logger.LogInformation($"OrderId:{order.Id} code:{order.OrderCode.Value} --> " + paymentStatus.Message);
                if (paymentStatus.StatusCode == 500)
                {
                    continue;
                }

                _logger.LogInformation("Type of paymentStatus.Data: {Type}", paymentStatus.Data?.GetType());
                _logger.LogInformation("paymentStatus.Data: {Data}", paymentStatus.Data);

                var paymentData = paymentStatus.Data as PaymentLinkInformation;

                if ("PAID".Equals(paymentData!.status))
                {
                    _logger.LogInformation("Payment for order {OrderId} is paid", order.Id);
                    var existOrder = await _orderRepository.GetByIdNoInlcudeAsync(order.Id);
                    //Check if order has preorder product
                    var existPreorder =
                        await _orderRepository.IsExistPreorderProductAsync(order.Id);
                    if (existPreorder)
                    {
                        _logger.LogInformation("Order {OrderId} has preorder product", order.Id);
                        existOrder.StatusId = (int)OrderStatusId.Preorder; //Preorder
                    }
                    else
                    {
                        existOrder!.StatusId = (int)OrderStatusId.Processing; //Processing
                    }

                    existOrder.PaymentDate = DateTime.UtcNow;
                    _orderRepository.Update(existOrder);
                    var payResult = await _unitOfWork.SaveChangesAsync();
                    if (payResult < 0)
                    {
                        _logger.LogInformation("Update order status for order {OrderId} failed", order.Id);
                    }

                    continue;
                }

                if (!"CANCELLED".Equals(paymentData.status) && !"EXPIRED".Equals(paymentData.status)) continue;

                _logger.LogInformation("Payment for order {OrderId} is cancelled or expired", order.Id);

                foreach (var orderDetail in order.OrderDetails)
                {
                    var product = await _productRepository.GetByIdNoIncludeAsync(orderDetail.ProductId);
                    if (product!.OrderDetails.Any(x => x.Product.StatusId == (int)OrderStatusId.Preorder))
                    {
                        product.Quantity -= orderDetail.Quantity;
                    }
                    else
                    {
                        product.Quantity += orderDetail.Quantity;
                    }

                    _productRepository.Update(product);
                }


                order.StatusId = (int)OrderStatusId.Cancelled;
                _orderRepository.Update(order);
                var result = await _unitOfWork.SaveChangesAsync();
                foreach (var orderDetail in order.OrderDetails)
                {
                    _unitOfWork.Detach(orderDetail.Product);
                }

                if (result < 0)
                {
                    _logger.LogInformation("Update order status for order {OrderId} failed", order.Id);
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