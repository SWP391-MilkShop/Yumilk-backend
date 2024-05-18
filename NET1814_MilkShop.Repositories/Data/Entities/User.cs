using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NET1814_MilkShop.Repositories.Data.Entities;

[Table("users")]
public partial class User
{
    [Key]
    public Guid Id { get; set; }

    public string Username { get; set; } = null!;

    [Column(TypeName = "nvarchar(255)")]
    public string Password { get; set; } = null!;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }
    [Column("verification_code",TypeName = "nvarchar(6)")]
    public string? VerificationCode { get; set; }
    [Column("reset_password_code",TypeName = "nvarchar(6)")]
    public string? ResetPasswordCode { get; set; }

    public int RoleId { get; set; }

    [DefaultValue(false)]
    public bool IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = [];

    public virtual Role? Role { get; set; }
}
