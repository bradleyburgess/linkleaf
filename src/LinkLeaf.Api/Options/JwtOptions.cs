namespace LinkLeaf.Api.Options;

public class JwtOptions
{
    public string Key { get; set; } = null!;
    public int AccessTokenExpirationMinutes { get; set; } = 15;
    public int RefreshTokenExpirationMinutes { get; set; } = 60 * 24 * 7;
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
}