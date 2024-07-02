using NET1814_MilkShop.Repositories.Data.Entities;

namespace NET1814_MilkShop.Repositories.Models.MessageModels;

public class MessageModel
{
    public Guid Id { get; set; }
    public Guid SenderId { get; set; }
    public string SenderName { get; set; }
    public Guid RecipientId { get; set; }
    public string RecipientName { get; set; }
    public string Content { get; set; }
    public DateTime? DateRead { get; set; }
    public DateTime CreatedAt { get; set; }
}