using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using LinkLeaf.Api.DTOs;
using LinkLeaf.Api.DTOs.Bookmarks;
using LinkLeaf.Api.Models;
using LinkLeaf.Api.Services;
using LinkLeaf.Api.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace LinkLeaf.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookmarksController(IBookmarksService bookmarksService) : ControllerBase
{
    private readonly IBookmarksService _service = bookmarksService;

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<AddBookmarkResponseDto>> AddBookmark(AddBookmarkRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ControllerUtils.GetModelErrors(ModelState);
            return BadRequest(ApiResponse<AddBookmarkResponseDto>.FailureResponse(
                code: ApiStatusCode.VALIDATION_ERROR,
                errors: errors
            ));
        }


        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
            return BadRequest();

        var newBookmark = new Bookmark()
        {
            Title = request.Title,
            Url = request.Url,
            UserId = Guid.Parse(userId)
        };
        var bookmark = await _service.AddUserBookmarkAsync(newBookmark);

        if (bookmark is null)
            return StatusCode(500);

        return Ok(ApiResponse<AddBookmarkResponseDto>.SuccessResponse(new AddBookmarkResponseDto()
        {
            Id = bookmark.Id,
            Title = bookmark!.Title ?? "",
            Url = bookmark!.Url,
            UserId = bookmark.UserId,
            CreatedAt = bookmark.CreatedAt
        }));
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<GetUserBookmarksResponseDto>> GetUserBookmarks()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
            return BadRequest();

        var bookmarks = await _service.GetUserBookmarksAsync(Guid.Parse(userId));

        var bookmarkDtos = bookmarks.Select(b => new BookmarkDto()
        {
            Id = b.Id,
            Title = b.Title,
            Url = b.Url,
            CreatedAt = b.CreatedAt,
            UserId = b.UserId
        }).ToList();

        return Ok(ApiResponse<GetUserBookmarksResponseDto>.SuccessResponse(
            new GetUserBookmarksResponseDto()
            {
                Bookmarks = bookmarkDtos
            }
        ));
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<GetBookmarkByIdResponseDto>> GetBookmarkById(Guid id)
    {
        var userResult = Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId);
        if (!userResult)
            return BadRequest();

        var bookmark = await _service.GetBookmarkByIdAsync(id);
        if (bookmark?.UserId != userId)
            return Unauthorized();

        return Ok(ApiResponse<GetBookmarkByIdResponseDto>.SuccessResponse(
            new GetBookmarkByIdResponseDto()
            {
                Bookmark = new BookmarkDto()
                {
                    Id = bookmark.Id,
                    Title = bookmark.Title,
                    Url = bookmark.Url,
                    UserId = bookmark.UserId
                }
            }
        ));
    }
}
