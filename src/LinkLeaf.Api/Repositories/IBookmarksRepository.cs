using LinkLeaf.Api.Models;

namespace LinkLeaf.Api.Repositories;

public interface IBookmarksRepository
{
    Task<Bookmark?> AddBookmarkAsync(Bookmark bookmark);
    Task<Bookmark?> GetBookmarkByIdAsync(Guid id);
    Task<List<Bookmark>> GetBookmarksByUserAsync(Guid userId);
    Task DeleteBookmarkAsync(Guid id);
}
