using LinkLeaf.Api.Models;

namespace LinkLeaf.Api.Services;

public interface IRefreshTokensRepository
{
    Task<RefreshToken?> AddUserRefreshToken(Guid userId, string token);
    Task<RefreshToken?> FindHashedToken(string token);
}
