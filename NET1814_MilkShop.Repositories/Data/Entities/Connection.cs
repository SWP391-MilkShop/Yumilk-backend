using System.ComponentModel.DataAnnotations.Schema;

namespace NET1814_MilkShop.Repositories.Data.Entities;

[Table("connections")]
public class Connection
{
    public string ConnectionId { get; set; }
    public Guid UserId { get; set; }
}