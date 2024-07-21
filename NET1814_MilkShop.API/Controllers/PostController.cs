using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NET1814_MilkShop.API.CoreHelpers.ActionFilters;
using NET1814_MilkShop.API.CoreHelpers.Extensions;
using NET1814_MilkShop.Repositories.Models.PostModel;
using NET1814_MilkShop.Services.Services.Interfaces;
using ILogger = Serilog.ILogger;

namespace NET1814_MilkShop.API.Controllers;

[ApiController]
[Route("api/posts")]
public class PostController : Controller
{
    private readonly IPostService _postService;
    private readonly ILogger _logger;

    public PostController(IPostService postService, ILogger logger)
    {
        _postService = postService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetPosts([FromQuery] PostQueryModel queryModel)
    {
        _logger.Information("Get posts log");
        var response = await _postService.GetPostsAsync(queryModel);
        return ResponseExtension.Result(response);
    }
    [HttpGet("{postId}")]
    public async Task<IActionResult> GetPost(Guid postId)
    {
        _logger.Information("Get post {postId}", postId);
        var response = await _postService.GetPostByIdAsync(postId);
        return ResponseExtension.Result(response);
    }
    [HttpPost]
    [Authorize(AuthenticationSchemes = "Access", Roles = "1,2")]
    [ServiceFilter(typeof(UserExistsFilter))]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostModel model)
    {
        var userId = (HttpContext.Items["UserId"] as Guid?)!.Value;
        _logger.Information("User {userId} create a post", userId);
        var res = await _postService.CreatePostAsync(userId, model);
        return ResponseExtension.Result(res);
    }

    [HttpPatch("{postId}")]
    [Authorize(AuthenticationSchemes = "Access", Roles = "1,2")]
    [ServiceFilter(typeof(UserExistsFilter))]
    public async Task<IActionResult> UpdatePost(Guid postId, [FromBody] UpdatePostModel model)
    {
        var userId = (HttpContext.Items["UserId"] as Guid?)!.Value;
        _logger.Information("User {userId} update post {postId}", userId, postId);
        var res = await _postService.UpdatePostAsync(userId, postId, model);
        return ResponseExtension.Result(res);
    }

    /// <summary>
    /// Only admin can delete post
    /// A staff can unpublish a post using UpdatePost
    /// </summary>
    /// <param name="postId"></param>
    /// <returns></returns>
    [HttpDelete("{postId}")]
    [Authorize(AuthenticationSchemes = "Access", Roles = "1")]
    public async Task<IActionResult> DeletePost(Guid postId)
    {
        _logger.Information("Delete post {postId}", postId);
        var res = await _postService.DeletePostAsync(postId);
        return ResponseExtension.Result(res);
    }
}