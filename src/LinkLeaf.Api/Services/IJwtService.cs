using LinkLeaf.Api.Models;

namespace LinkLeaf.Api.Services;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken(int size = 64);
}
