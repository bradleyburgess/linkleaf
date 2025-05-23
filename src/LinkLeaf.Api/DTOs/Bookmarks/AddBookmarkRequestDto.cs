using System.ComponentModel.DataAnnotations;

namespace LinkLeaf.Api.DTOs.Bookmarks;

public class AddBookmarkRequestDto
{
    public string Title { get; set; } = string.Empty;

    [Required]
    [Url]
    public string Url { get; set; } = string.Empty;
}
