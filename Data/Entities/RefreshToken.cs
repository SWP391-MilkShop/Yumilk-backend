using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SWP391_DEMO.Entities;
[Table("RefreshTokens")]
public partial class RefreshToken
{
    [Key]
    public int Id { get; set; }

    public string? Token { get; set; }

    public DateTime? Expires { get; set; }

    public Guid? UserId { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual User? User { get; set; }
}
