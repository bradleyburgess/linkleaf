namespace LinkLeaf.Api.DTOs.Bookmarks;

public class GetUserBookmarksResponseDto
{
    public List<BookmarkDto> Bookmarks { get; set; } = new();
}
