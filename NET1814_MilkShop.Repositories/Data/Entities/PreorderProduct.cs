using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NET1814_MilkShop.Repositories.Data.Entities
{
    [Table("preorder_product")]
    public class PreorderProduct
    {
        [Column("product_id")]
        [Key]
        [ForeignKey("Product")]
        public Guid ProductId { get; set; }

        [Column("max_preorder_quantity")]
        public int MaxPreOrderQuantity { get; set; } //khi add thi cong vao, khi ship thi tru di

        [Column("start_date", TypeName ="datetime2")]
        public DateTime StartDate { get; set; } //ngay bat dau preorder

        [Column("end_date", TypeName = "datetime2")]
        public DateTime EndDate { get; set; } //ngay ket thuc preorder

        [Column("expected_preorder_days")]
        public int ExpectedPreOrderDays { get; set; } //so ngay du kien nhap hang

        public virtual Product Product { get; set; } = null!;
    }
}
