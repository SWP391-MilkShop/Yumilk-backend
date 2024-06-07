using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace NET1814_MilkShop.Repositories.Models.ProductReviewModels
{
    public class ReviewQueryModel
    {
        /// <summary>
        /// Range 1-5 (default is 0 which include all reviews)
        /// </summary>
        [Range(0, 5, ErrorMessage = "Rating must be between 0 and 5")]
        public int Rating { get; set; } = 0;
        /// <summary>
        /// Default is null (include all reviews)
        /// </summary>
        public bool? IsActive { get; set; }
        /// <summary>
        /// Sort by rating, created at (default is created at)
        /// </summary>
        public string? SortColumn { get; set; }

        /// <summary>
        /// Sort order ("desc" for descending) (default is ascending)
        /// </summary>
        public string? SortOrder { get; set; }

        /// <summary>
        /// Page number (default is 1)
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// Number of items per page (default is 10)
        /// </summary>
        public int PageSize { get; set; } = 10;
    }
}
