using NET1814_MilkShop.Repositories.CoreHelpers.Constants;
using NET1814_MilkShop.Repositories.CoreHelpers.Enum;
using NET1814_MilkShop.Repositories.Data.Entities;
using NET1814_MilkShop.Repositories.Models;
using NET1814_MilkShop.Repositories.Models.PostModel;
using NET1814_MilkShop.Repositories.Repositories;
using NET1814_MilkShop.Repositories.UnitOfWork;
using NET1814_MilkShop.Services.CoreHelpers;
using NET1814_MilkShop.Services.CoreHelpers.Extensions;
using System.Linq.Expressions;

namespace NET1814_MilkShop.Services.Services
{
    public interface IPostService
    {
        Task<ResponseModel> GetPostsAsync(PostQueryModel queryModel);
        Task<ResponseModel> CreatePostAsync(Guid authorId, CreatePostModel model);
        Task<ResponseModel> UpdatePostAsync(Guid userId, Guid postId, UpdatePostModel model);
        Task<ResponseModel> DeletePostAsync(Guid postId);
    }
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        public PostService(IPostRepository postRepository, IUnitOfWork unitOfWork, IUserRepository userRepository)
        {
            _postRepository = postRepository;
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
        }
        public async Task<ResponseModel> CreatePostAsync(Guid authorId, CreatePostModel model)
        {
            var author = await _userRepository.GetByIdAsync(authorId);
            if (author == null)
            {
                return ResponseModel.BadRequest(ResponseConstants.NotFound("Người dùng"));
            }
            if (author.RoleId == (int)RoleId.CUSTOMER)
            {
                return ResponseModel.BadRequest(ResponseConstants.NotEnoughPermission);
            }
            var post = new Post
            {
                Title = model.Title,
                Content = model.Content,
                AuthorId = authorId,
                MetaTitle = model.MetaTitle ?? model.Title,
                MetaDescription = model.MetaDescription ?? model.Content,
                IsActive = false // default is unpublished
            };
            _postRepository.Add(post);
            var result = await _unitOfWork.SaveChangesAsync();
            if (result > 0)
            {
                return ResponseModel.Success(ResponseConstants.Create("bài viết", true), null);
            }
            return ResponseModel.Error(ResponseConstants.Create("bài viết", false));
        }

        public async Task<ResponseModel> DeletePostAsync(Guid postId)
        {
            var post = await _postRepository.GetByIdAsync(postId);
            if (post == null)
            {
                return ResponseModel.BadRequest(ResponseConstants.NotFound("bài viết"));
            }
            _postRepository.Delete(post);
            var result = await _unitOfWork.SaveChangesAsync();
            if (result > 0)
            {
                return ResponseModel.Success(ResponseConstants.Delete("bài viết", true), null);
            }
            return ResponseModel.Error(ResponseConstants.Delete("bài viết", false));
        }

        public async Task<ResponseModel> GetPostsAsync(PostQueryModel queryModel)
        {
            var query = _postRepository.GetPostQuery(includeAuthor: true);
            var searchTerm = StringExtension.Normalize(queryModel.SearchTerm);
            var authorId = queryModel.AuthorId;
            query = query.Where(p => (!queryModel.IsActive.HasValue || p.IsActive == queryModel.IsActive.Value)
            && (string.IsNullOrEmpty(searchTerm)
                || p.Title.Contains(searchTerm)
                || p.Content.Contains(searchTerm)
                || p.MetaTitle.Contains(searchTerm)
                || p.MetaDescription.Contains(searchTerm))
            && (authorId == Guid.Empty || p.AuthorId == authorId));
            if("desc".Equals(queryModel.SortOrder?.ToLower()))
            {
                query = query.OrderByDescending(GetSortProperty(queryModel));
            }
            else
            {
                query = query.OrderBy(GetSortProperty(queryModel));
            }
            var postModelQuery = query.Select(p => new PostModel
            {
                Id = p.Id,
                Title = p.Title,
                Content = p.Content,
                AuthorId = p.AuthorId,
                AuthorName = p.Author.FirstName + " " + p.Author.LastName,
                MetaTitle = p.MetaTitle,
                MetaDescription = p.MetaDescription,
                IsActive = p.IsActive,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.ModifiedAt ?? null
            });
            var posts = await PagedList<PostModel>.CreateAsync(
                postModelQuery, 
                queryModel.Page, 
                queryModel.PageSize);
            return ResponseModel.Success(ResponseConstants.Get("bài viết", posts.TotalCount > 0), posts);
        }
        private static Expression<Func<Post, object>> GetSortProperty(PostQueryModel queryModel)
            => queryModel.SortColumn?.ToLower().Replace(" ", "") switch
            {
                "title" => p => p.Title,
                "createdat" => p => p.CreatedAt,
                _ => p => p.ModifiedAt ?? p.CreatedAt
            };
        public async Task<ResponseModel> UpdatePostAsync(Guid userId, Guid postId, UpdatePostModel model)
        {
            var post = await _postRepository.GetByIdAsync(postId);
            if (post == null)
            {
                return ResponseModel.BadRequest(ResponseConstants.NotFound("bài viết"));
            }
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return ResponseModel.BadRequest(ResponseConstants.NotFound("người dùng"));
            }
            // Only admin or actual author can update post
            if (user.RoleId != (int)RoleId.ADMIN && userId != post.AuthorId)
            {
                return ResponseModel.BadRequest(ResponseConstants.NotEnoughPermission);
            }
            post.Title = string.IsNullOrEmpty(model.Title) ? post.Title : model.Title;
            post.Content = string.IsNullOrEmpty(model.Content) ? post.Content : model.Content;
            post.MetaTitle = string.IsNullOrEmpty(model.MetaTitle) ? post.MetaTitle : model.MetaTitle;
            post.MetaDescription = string.IsNullOrEmpty(model.MetaDescription) ? post.MetaDescription : model.MetaDescription;
            post.IsActive = model.IsActive;
            _postRepository.Update(post);
            var result = await _unitOfWork.SaveChangesAsync();
            if (result > 0)
            {
                return ResponseModel.Success(ResponseConstants.Update("bài viết", true), null);
            }
            return ResponseModel.Error(ResponseConstants.Update("bài viết", false));
        }
    }
}
