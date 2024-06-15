using Microsoft.EntityFrameworkCore;
using NET1814_MilkShop.Repositories.Data;
using NET1814_MilkShop.Repositories.Data.Entities;

namespace NET1814_MilkShop.Repositories.Repositories
{
    public interface IPostRepository
    {
        Task<Post?> GetByIdAsync(Guid id);
        IQueryable<Post> GetPostQuery(bool includeAuthor);
        Task<IEnumerable<Post>> GetByAuthorId(Guid authorId);
        void Add(Post post);
        void Update(Post post);
        void Delete(Post post);
    }
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
}
