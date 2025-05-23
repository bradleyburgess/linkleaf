using LinkLeaf.Api.DTOs.Auth;
using LinkLeaf.Api.DTOs.User;

namespace LinkLeaf.Api.Services;

public interface IAuthService
{
    Task<(TokensDto Tokens, UserDto User)?> Register(RegisterRequestDto request);
    Task<(TokensDto Tokens, UserDto User)?> Login(LoginRequestDto request);
    Task<(TokensDto Tokens, UserDto User)?> RefreshTokens(string refreshToken);
}
