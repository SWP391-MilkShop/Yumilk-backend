using NET1814_MilkShop.Repositories.Data.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NET1814_MilkShop.Repositories.Data.Entities
{
    [Table("preorder_product")]
    public class PreorderProduct : IAuditableEntity
    {
        [Column("product_id")]
        [Key]
        [ForeignKey("Product")]
        public Guid ProductId { get; set; }

        [Column("max_preorder_quantity")] 
        public int MaxPreOrderQuantity { get; set; } = 1000; //default 1000

        [Column("start_date", TypeName ="datetime2")]
        public DateTime StartDate { get; set; } //ngay bat dau preorder

        [Column("end_date", TypeName = "datetime2")]
        public DateTime EndDate { get; set; } //ngay ket thuc preorder

        [Column("expected_preorder_days")]
        public int ExpectedPreOrderDays { get; set; } //so ngay du kien nhap hang

        public virtual Product Product { get; set; } = null!;

        [Column("created_at", TypeName = "datetime2")]
        public DateTime CreatedAt { get; set; }

        [Column("modified_at", TypeName = "datetime2")]
        public DateTime? ModifiedAt { get; set; }

        [Column("deleted_at", TypeName = "datetime2")]
        public DateTime? DeletedAt { get; set; }
    }
}
