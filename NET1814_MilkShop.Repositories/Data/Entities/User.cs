using NET1814_MilkShop.Repositories.Data.Interfaces;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NET1814_MilkShop.Repositories.Data.Entities;

[Table("users")]
public partial class User : IAuditableEntity
{
    [Key]
    public Guid Id { get; set; }

    public string Username { get; set; } = null!;

    [Column(TypeName = "nvarchar(255)")]
    public string Password { get; set; } = null!;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    [Column("verification_code", TypeName = "nvarchar(6)")]
    public string? VerificationCode { get; set; }

    [Column("reset_password_code", TypeName = "nvarchar(6)")]
    public string? ResetPasswordCode { get; set; }

    public int RoleId { get; set; }

    [DefaultValue(false)]
    public bool IsActive { get; set; }
    [Column("is_banned")]
    [DefaultValue(false)]
    public bool IsBanned { get; set; }

    [Column("created_at", TypeName = "datetime2")]
    public DateTime CreatedAt { get; set; }

    [Column("modified_at", TypeName = "datetime2")]
    public DateTime? ModifiedAt { get; set; }

    [Column("deleted_at", TypeName = "datetime2")]
    public DateTime? DeletedAt { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual Role? Role { get; set; }
}
