using LinkLeaf.Api.Models;

namespace LinkLeaf.Api.Services;

public interface IBookmarksService
{
    Task<Bookmark?> AddUserBookmarkAsync(Bookmark bookmark);
    Task<bool> DeleteUserBookmarkAsync(Guid bookmarkId, Guid userId);
    Task<List<Bookmark>> GetUserBookmarksAsync(Guid userId);
}
