using Microsoft.EntityFrameworkCore;
using NET1814_MilkShop.Repositories.Data;
using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Repositories.Interfaces;

namespace NET1814_MilkShop.Repositories.Repositories.Implementations;

public class PostRepository : Repository<Post>, IPostRepository
{
    public PostRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Post>> GetByAuthorId(Guid authorId)
    {
        var posts = await _query.Where(p => p.AuthorId == authorId).ToListAsync();
        return posts;
    }

    public IQueryable<Post> GetPostQuery(bool includeAuthor)
    {
        return includeAuthor ? _query.Include(p => p.Author) : _query;
    }
}