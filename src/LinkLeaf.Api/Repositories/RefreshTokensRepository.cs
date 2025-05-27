using LinkLeaf.Api.Data;
using LinkLeaf.Api.Models;
using LinkLeaf.Api.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LinkLeaf.Api.Services;

public class RefreshTokensRepository(AppDbContext dbContext, IOptions<JwtOptions> jwtOptions) : IRefreshTokensRepository
{
    private readonly AppDbContext _db = dbContext;
    private readonly int tokenExpirationMinutes = jwtOptions.Value.RefreshTokenExpirationMinutes;

    public async Task<RefreshToken?> AddUserRefreshToken(Guid userId, string token)
    {
        var refreshToken = new RefreshToken()
        {
            Token = token,
            TokenExpiresAt = DateTime.UtcNow.AddMinutes(tokenExpirationMinutes),
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };
        await _db.RefreshTokens.AddAsync(refreshToken);
        await _db.SaveChangesAsync();
        return refreshToken;
    }

    public async Task<RefreshToken?> FindHashedToken(string token)
    {
        var savedToken = await _db.RefreshTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Token == token);
        return savedToken;
    }
}
