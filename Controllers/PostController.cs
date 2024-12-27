using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UsersApiDotnet.Data;
using UsersApiDotnet.Dtos;
using UsersApiDotnet.Models;

namespace UsersApiDotnet.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class PostController : ControllerBase
{
    private readonly DataContextDapper _dapper;

    public PostController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
    }

    /* Get Posts -----------------------------------------------------------------------*/
    [HttpGet("Posts")]
    public IEnumerable<Post> GetPosts()
    {
        string sql = @"SELECT [PostId],
                [UserId],
                [PostTitle],
                [PostContent],
                [PostCreated],
                [PostUpdated] 
            FROM Posts";
            
        return _dapper.LoadData<Post>(sql);
    }

    /* Get Single Post -----------------------------------------------------------------------*/
    [HttpGet("PostSingle/{postId}")]
    public Post GetPostSingle(int postId)
    {
        string sql = @"
            SELECT [PostId],
                [UserId],
                [PostTitle],
                [PostContent],
                [PostCreated],
                [PostUpdated] 
            FROM Posts
            WHERE PostId = @PostId";

        var parameters = new { PostId = postId };
        return _dapper.LoadDataSingle<Post>(sql, parameters);
    }


    /* Get Post by user ID -----------------------------------------------------------------------*/
    [HttpGet("PostsByUser/{userId}")]
    public IEnumerable<Post> GetPostsByUser(int userId)
    {
        string sql = @"
            SELECT [PostId],
                [UserId],
                [PostTitle],
                [PostContent],
                [PostCreated],
                [PostUpdated] 
            FROM Posts
            WHERE UserId = @UserId";

        var parameters = new { UserId = userId };
        return _dapper.LoadData<Post>(sql, parameters);
    }


    /* Get my Posts -----------------------------------------------------------------------*/
    [HttpGet("MyPosts")]
    public IEnumerable<Post> GetMyPosts()
    {
        string userId = this.User.FindFirst("userId")?.Value ?? throw new UnauthorizedAccessException();
        string sql = @"
            SELECT [PostId],
                [UserId],
                [PostTitle],
                [PostContent],
                [PostCreated],
                [PostUpdated] 
            FROM Posts
            WHERE UserId = @UserId";

        var parameters = new { UserId = userId };
        return _dapper.LoadData<Post>(sql, parameters);
    }


    /* Search by post title or content -----------------------------------------------------------------------*/
    [HttpGet("PostsBySearch/{searchParam}")]
    public IEnumerable<Post> PostsBySearch(string searchParam)
    {
        string sql = @"
            SELECT [PostId],
                [UserId],
                [PostTitle],
                [PostContent],
                [PostCreated],
                [PostUpdated] 
            FROM Posts
            WHERE PostTitle LIKE @SearchParam
                OR PostContent LIKE @SearchParam";

        var parameters = new { SearchParam = $"%{searchParam}%" };
        return _dapper.LoadData<Post>(sql, parameters);
    }


    /* New post -----------------------------------------------------------------------*/
    [HttpPost("Post")]
    public IActionResult AddPost(PostToAddDto postToAdd)
    {
        string userId = this.User.FindFirst("userId")?.Value ?? throw new UnauthorizedAccessException();
        string sql = @"
            INSERT INTO Posts(
                [UserId],
                [PostTitle],
                [PostContent],
                [PostCreated],
                [PostUpdated]
            ) VALUES (
                @UserId,
                @PostTitle,
                @PostContent,
                GETDATE(),
                GETDATE()
            )";

        var parameters = new
        {
            UserId = userId,
            PostTitle = postToAdd.PostTitle,
            PostContent = postToAdd.PostContent
        };

        if (_dapper.ExecuteSql(sql, parameters))
        {
            return Ok();
        }

        throw new Exception("Failed to create new post!");
    }

    /* Edit Post -----------------------------------------------------------------------*/
    [HttpPut("Post")]
    public IActionResult EditPost(PostToEditDto postToEdit)
    {
        string userId = this.User.FindFirst("userId")?.Value ?? throw new UnauthorizedAccessException();
        string sql = @"
            UPDATE Posts 
            SET PostContent = @PostContent,
                PostTitle = @PostTitle,
                PostUpdated = GETDATE()
            WHERE PostId = @PostId
                AND UserId = @UserId";

        var parameters = new
        {
            PostContent = postToEdit.PostContent,
            PostTitle = postToEdit.PostTitle,
            PostId = postToEdit.PostId,
            UserId = userId
        };

        if (_dapper.ExecuteSql(sql, parameters))
        {
            return Ok();
        }

        throw new Exception("Failed to edit post!");
    }

    /* Delete Post -----------------------------------------------------------------------*/
    [HttpDelete("Post/{postId}")]
    public IActionResult DeletePost(int postId)
    {
        string userId = this.User.FindFirst("userId")?.Value ?? throw new UnauthorizedAccessException();
        string sql = @"
            DELETE FROM Posts 
            WHERE PostId = @PostId
                AND UserId = @UserId";

        var parameters = new
        {
            PostId = postId,
            UserId = userId
        };

        if (_dapper.ExecuteSql(sql, parameters))
        {
            return Ok();
        }

        throw new Exception("Failed to delete post!");
    }
}
