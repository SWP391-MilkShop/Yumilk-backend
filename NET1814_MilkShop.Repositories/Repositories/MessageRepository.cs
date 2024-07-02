using Microsoft.EntityFrameworkCore;
using NET1814_MilkShop.Repositories.Data;
using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Models.MessageModels;

namespace NET1814_MilkShop.Repositories.Repositories;

public interface IMessageRepository
{
    void Add(Message message);
    void Delete(Message message);
    void Update(Message message);
    Task<Message?> GetMessageById(Guid id);
    Task<List<Message>> GetMessageThread(Guid currentUserId, Guid recipientUserId);
}

public class MessageRepository : Repository<Message>, IMessageRepository
{
    public MessageRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<Message?> GetMessageById(Guid id)
    {
        return await _context.Messages.SingleOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<Message>> GetMessageThread(Guid currentUserId, Guid recipientUserId)
    {
        return await _query.Where(x =>
                x.SenderId == currentUserId && x.RecipientId == recipientUserId ||
                x.SenderId == recipientUserId && x.RecipientId == currentUserId)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync();
    }
}