using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Models.PostModel;

namespace NET1814_MilkShop.Services.Services.Interfaces;

public interface IPostService
{
    Task<ResponseModel> GetPostsAsync(PostQueryModel queryModel);
    Task<ResponseModel> CreatePostAsync(Guid authorId, CreatePostModel model);
    Task<ResponseModel> UpdatePostAsync(Guid userId, Guid postId, UpdatePostModel model);
    Task<ResponseModel> DeletePostAsync(Guid postId);
}