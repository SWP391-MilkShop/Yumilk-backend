using System.ComponentModel.DataAnnotations;

namespace NET1814_MilkShop.Repositories.Models.ProductModels
{
    public class ProductQueryModel : QueryModel
    {
        public string? Category { get; set; }
        public string? Brand { get; set; }
        public string? Unit { get; set; }
        /// <summary>
        /// SELLING | PREORDER | OUT OF STOCK
        /// <para>Default is SELLING</para>
        /// </summary>
        public string? Status { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Min price must be greater than 0")]
        public int MinPrice { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Max price must be greater than 0")]
        public int MaxPrice { get; set; }
        public bool? IsActive { get; set; }

        /// <summary>
        /// Sort by id, name, quantity, sale price, created at, rating, order count (default is id)
        /// </summary>
        public new string? SortColumn { get; set; }

        /// <summary>
        /// Search by name, description, brand, unit, category
        /// </summary>
        public new string? SearchTerm { get; set; }
    }
}
