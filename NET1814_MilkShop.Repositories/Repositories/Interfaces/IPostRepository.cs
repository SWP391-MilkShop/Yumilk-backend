using NET1814_MilkShop.Repositories.Data.Entities;

namespace NET1814_MilkShop.Repositories.Repositories.Interfaces;

public interface IPostRepository
{
    Task<Post?> GetByIdAsync(Guid id);
    IQueryable<Post> GetPostQuery(bool includeAuthor);
    Task<IEnumerable<Post>> GetByAuthorId(Guid authorId);
    void Add(Post post);
    void Update(Post post);
    void Delete(Post post);
}