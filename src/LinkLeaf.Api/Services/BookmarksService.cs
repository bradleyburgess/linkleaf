using LinkLeaf.Api.Models;
using LinkLeaf.Api.Repositories;

namespace LinkLeaf.Api.Services;

public class BookmarksService(IBookmarksRepository bookmarksRepository) : IBookmarksService
{
    private readonly IBookmarksRepository _bookmarksRepository = bookmarksRepository;

    public async Task<Bookmark?> AddUserBookmarkAsync(Bookmark bookmark)
    {
        var _bookmark = await _bookmarksRepository.AddBookmarkAsync(bookmark);
        return _bookmark;
    }

    public async Task<bool> DeleteUserBookmarkAsync(Guid bookmarkId, Guid userId)
    {
        var bookmark = await _bookmarksRepository.GetBookmarkByIdAsync(bookmarkId);
        if (userId != bookmark?.UserId)
            return false;
        await _bookmarksRepository.DeleteBookmarkAsync(bookmarkId);
        return true;
    }

    public async Task<List<Bookmark>> GetUserBookmarksAsync(Guid userId)
    {
        var bookmarks = await _bookmarksRepository.GetBookmarksByUserAsync(userId);
        return bookmarks;
    }
}
