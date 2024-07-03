using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NET1814_MilkShop.Repositories.Data.Entities;

[Table("groups")]
public class Group
{
    [Key]
    public string Name { get; set; }

    public ICollection<Connection> Connections { get; set; } = new List<Connection>();
}