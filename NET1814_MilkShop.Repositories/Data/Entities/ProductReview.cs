using NET1814_MilkShop.Repositories.Data.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NET1814_MilkShop.Repositories.Data.Entities
{
    [Table("product_reviews")]
    public class ProductReview : IAuditableEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Column("customer_id")]
        public Guid CustomerId { get; set; }
        [Required]
        [Column("product_id")]
        public Guid ProductId { get; set; }
        [Required]
        [Column("order_id")]
        public Guid OrderId { get; set; }
        [MaxLength(255)]
        public string Review { get; set; } = "";
        public int Rating { get; set; }
        [Column("is_active")]
        public bool IsActive { get; set; } = true;
        [Column("created_at", TypeName = "datetime2")]
        public DateTime CreatedAt { get; set; }
        [Column("modified_at", TypeName = "datetime2")]
        public DateTime? ModifiedAt { get; set; }
        [Column("deleted_at", TypeName = "datetime2")]
        public DateTime? DeletedAt { get; set; }

        public virtual Customer? Customer { get; set; }
        public virtual Product? Product { get; set; }
        public virtual Order? Order { get; set; }
    }
}
