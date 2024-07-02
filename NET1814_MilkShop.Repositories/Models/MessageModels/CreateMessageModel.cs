namespace NET1814_MilkShop.Repositories.Models.MessageModels;

public class CreateMessageModel
{
    public Guid RecipientId { get; set; }
    public string Content { get; set; }
}