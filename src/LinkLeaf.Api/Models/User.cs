namespace LinkLeaf.Api.Models;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime RefreshTokenExpiration { get; set; } = DateTime.UtcNow;
    public List<Bookmark> Bookmarks { get; set; } = new();
}
