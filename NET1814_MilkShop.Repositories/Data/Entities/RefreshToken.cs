using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NET1814_MilkShop.Repositories.Data.Entities;

[Table("RefreshTokens")]
public partial class RefreshToken
{
    [Key]
    public int Id { get; set; }

    public string? Token { get; set; }

    public DateTime? Expires { get; set; } 

    public Guid UserId { get; set; }

    [DefaultValue(false)]
    public bool IsActive { get; set; } 

    public DateTime? CreatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual User? User { get; set; }
}
