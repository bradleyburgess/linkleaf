namespace LinkLeaf.Api.DTOs.Auth;

public class TokensDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}
