using LinkLeaf.Api.Data;
using LinkLeaf.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace LinkLeaf.Api.Repositories;

public class BookmarksRepository(AppDbContext dbContext) : IBookmarksRepository
{
    private readonly AppDbContext _db = dbContext;

    public async Task<Bookmark?> AddBookmarkAsync(Bookmark bookmark)
    {
        await _db.Bookmarks.AddAsync(bookmark);
        await _db.SaveChangesAsync();
        return bookmark;
    }

    public async Task DeleteBookmarkAsync(Guid id)
    {
        var bookmark = await _db.Bookmarks.FirstOrDefaultAsync(b => b.Id == id);
        if (bookmark is not null)
        {
            _db.Bookmarks.Remove(bookmark);
            await _db.SaveChangesAsync();
        }

    }

    public async Task<Bookmark?> GetBookmarkByIdAsync(Guid id)
    {
        var bookmark = await _db.Bookmarks.FirstOrDefaultAsync(b => b.Id == id);
        return bookmark;
    }

    public async Task<List<Bookmark>> GetBookmarksByUserAsync(Guid userId)
    {
        var bookmarks = await _db.Bookmarks.Where(b => b.UserId == userId).ToListAsync();
        return bookmarks;
    }
}
