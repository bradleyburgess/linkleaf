using System.ComponentModel.DataAnnotations;

namespace LinkLeaf.Api.DTOs.Bookmarks;

public class BookmarkDto
{
    [Required]
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    [Required]
    public string Url { get; set; } = string.Empty;

    [Required]
    public DateTime CreatedAt { get; set; }

    [Required]
    public Guid UserId { get; set; }
}
