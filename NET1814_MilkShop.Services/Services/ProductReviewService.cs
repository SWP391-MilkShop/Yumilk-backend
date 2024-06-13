using NET1814_MilkShop.Repositories.CoreHelpers.Constants;
using NET1814_MilkShop.Repositories.CoreHelpers.Enum;
using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Models.ProductReviewModels;
using NET1814_MilkShop.Repositories.Repositories;
using NET1814_MilkShop.Repositories.UnitOfWork;
using NET1814_MilkShop.Services.CoreHelpers;
using System.Linq.Expressions;

namespace NET1814_MilkShop.Services.Services
{
    public interface IProductReviewService
    {
        Task<ResponseModel> GetProductReviewsAsync(Guid productId, ReviewQueryModel queryModel);
        Task<ResponseModel> CreateProductReviewAsync(Guid productId, CreateReviewModel model);
        Task<ResponseModel> UpdateProductReviewAsync(int id, UpdateReviewModel model);
        Task<ResponseModel> DeleteProductReviewAsync(int id);
    }
    public class ProductReviewService : IProductReviewService
    {
        private readonly IProductReviewRepository _productReviewRepository;
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;
        public ProductReviewService(IProductReviewRepository productReviewRepository, IProductRepository productRepository, IOrderRepository orderRepository, IUnitOfWork unitOfWork)
        {
            _productReviewRepository = productReviewRepository;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
        }
        /// <summary>
        /// Get sort property as expression
        /// </summary>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        private static Expression<Func<ProductReview, object>> GetSortProperty(
            ReviewQueryModel queryModel
        ) =>
            queryModel.SortColumn?.ToLower().Replace(" ", "") switch
            {
                "rating" => x => x.Rating,
                _ => x => x.CreatedAt
            };

        public async Task<ResponseModel> CreateProductReviewAsync(Guid productId, CreateReviewModel model)
        {
            var IsProductExist = await _productRepository.IsExistAsync(productId);
            if (!IsProductExist)
            {
                return ResponseModel.BadRequest(ResponseConstants.NotFound("Sản phẩm"));
            }
            //include order details
            var order = await _orderRepository.GetByIdAsync(model.OrderId, includeDetails: true);
            if (order == null)
            {
                return ResponseModel.BadRequest(ResponseConstants.NotFound("Đơn hàng"));
            }
            var IsContain = order.OrderDetails.Any(x => x.ProductId == productId);
            if (!IsContain || order.StatusId != (int)OrderStatusId.DELIVERED) // Để tạm 4 là đã giao hàng
            {
                return ResponseModel.BadRequest(ResponseConstants.ReviewConstraint);
            }
            var existingReview = await _productReviewRepository.GetByOrderIdAndProductIdAsync(model.OrderId, productId);
            if (existingReview != null)
            {
                return ResponseModel.BadRequest(ResponseConstants.ReviewPerOrder);
            }
            var review = new ProductReview
            {
                CustomerId = order.CustomerId!.Value,
                ProductId = productId,
                OrderId = model.OrderId,
                Review = model.Review,
                Rating = model.Rating,
                IsActive = true,
            };
            _productReviewRepository.Add(review);
            var result = await _unitOfWork.SaveChangesAsync();
            if (result > 0)
            {
                return ResponseModel.Success(ResponseConstants.Create("đánh giá sản phẩm", true), null);
            }
            return ResponseModel.Error(ResponseConstants.Create("đánh giá sản phẩm", false));
        }

        public async Task<ResponseModel> DeleteProductReviewAsync(int id)
        {
            var review = await _productReviewRepository.GetByIdAsync(id);
            if (review == null)
            {
                return ResponseModel.BadRequest(ResponseConstants.NotFound("Đánh giá"));
            }
            _productReviewRepository.Delete(review);
            var result = await _unitOfWork.SaveChangesAsync();
            if (result > 0)
            {
                return ResponseModel.Success(ResponseConstants.Delete("đánh giá sản phẩm", true), null);
            }
            return ResponseModel.Error(ResponseConstants.Delete("đánh giá sản phẩm", false));
        }

        public async Task<ResponseModel> GetProductReviewsAsync(Guid productId, ReviewQueryModel queryModel)
        {
            var IsProductExist = await _productRepository.IsExistAsync(productId);
            if (!IsProductExist)
            {
                return ResponseModel.BadRequest(ResponseConstants.NotFound("Sản phẩm"));
            }
            var query = _productReviewRepository.GetProductReviewQuery();
            query = query.Where(x => x.ProductId == productId
                            && queryModel.Rating == 0 || x.Rating == queryModel.Rating
                            && queryModel.IsActive == null || x.IsActive == queryModel.IsActive);
            if (queryModel.SortOrder == "desc")
            {
                query = query.OrderByDescending(GetSortProperty(queryModel));
            }
            else
            {
                query = query.OrderBy(GetSortProperty(queryModel));
            }
            var reviewModelQuery = query.Select(x => new ReviewModel
            {
                Id = x.Id,
                CustomerId = x.CustomerId,
                ProductId = x.ProductId,
                Review = x.Review,
                Rating = x.Rating,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.ModifiedAt
            });
            var productReviews = await PagedList<ReviewModel>.CreateAsync(reviewModelQuery, queryModel.Page, queryModel.PageSize);
            return ResponseModel.Success(ResponseConstants.Get("Danh sách đánh giá sản phẩm", productReviews.TotalCount > 0), productReviews);
        }

        public async Task<ResponseModel> UpdateProductReviewAsync(int id, UpdateReviewModel model)
        {
            var review = await _productReviewRepository.GetByIdAsync(id);
            if (review == null)
            {
                return ResponseModel.BadRequest(ResponseConstants.NotFound("Đánh giá"));
            }
            review.Review = model.Review ?? review.Review;
            review.Rating = model.Rating == 0 ? review.Rating : model.Rating;
            review.IsActive = model.IsActive ?? review.IsActive;
            _productReviewRepository.Update(review);
            var result = await _unitOfWork.SaveChangesAsync();
            if (result > 0)
            {
                return ResponseModel.Success(ResponseConstants.Update("đánh giá sản phẩm", true), null);
            }
            return ResponseModel.Error(ResponseConstants.Update("đánh giá sản phẩm", false));
        }
    }
}
