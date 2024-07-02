using Microsoft.EntityFrameworkCore;
using NET1814_MilkShop.Repositories.Data;
using NET1814_MilkShop.Repositories.Data.Entities;

namespace NET1814_MilkShop.Repositories.Repositories;

public interface IMessageRepository
{
    void Add(Message message);
    void Delete(Message message);
    Task<Message?> GetMessageById(Guid id);
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
}