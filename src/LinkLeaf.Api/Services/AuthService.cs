using LinkLeaf.Api.DTOs.Auth;
using LinkLeaf.Api.DTOs.User;
using LinkLeaf.Api.Models;
using LinkLeaf.Api.Security;
using LinkLeaf.Api.Repositories;
using Microsoft.AspNetCore.Identity;
using LinkLeaf.Api.Options;
using Microsoft.Extensions.Options;

namespace LinkLeaf.Api.Services;

public class AuthService(
    IJwtService tokenService,
    IRefreshTokensRepository refreshTokensRepository,
    IUsersRepository usersRepository,
    ITokenHasher tokenHasher,
    IOptions<JwtOptions> jwtOptions
) : IAuthService
{
    private readonly IJwtService _tokenService = tokenService;
    private readonly IUsersRepository _usersRepo = usersRepository;
    private readonly IRefreshTokensRepository _refreshTokensRepo = refreshTokensRepository;
    private readonly ITokenHasher _tokenHasher = tokenHasher;
    private readonly int _refreshTokenExpirationMinutes = jwtOptions.Value.RefreshTokenExpirationMinutes;

    public async Task<(TokensDto Tokens, UserDto User)?> Login(LoginRequestDto request)
    {
        var user = await _usersRepo.FindByEmail(request.Email);
        if (user is null)
            return null;

        var passwordResult = new PasswordHasher<User>()
            .VerifyHashedPassword(user, user.PasswordHash, request.Password);

        if (passwordResult == PasswordVerificationResult.Failed)
            return null;

        var tokens = GenerateTokens(user);

        await _refreshTokensRepo.AddUserRefreshToken(user.Id, _tokenHasher.Hash(tokens.RefreshToken));

        return (
            tokens, new UserDto()
            {
                Id = user.Id,
                Email = user.Email
            }
        );
    }

    public async Task<(TokensDto Tokens, UserDto User)?> Register(RegisterRequestDto request)
    {
        var existingUser = await _usersRepo.FindByEmail(request.Email);
        if (existingUser is not null)
            return null;

        var newUser = new User()
        {
            Email = request.Email,
            CreatedAt = DateTime.UtcNow,
        };

        var hashedPassword = new PasswordHasher<User>()
            .HashPassword(newUser, request.Password);

        newUser.PasswordHash = hashedPassword;

        var tokens = GenerateTokens(newUser);
        var refreshToken = _tokenHasher.Hash(tokens.RefreshToken);

        var user = await _usersRepo.AddAsync(newUser);
        if (user is null)
            return null;
        await _refreshTokensRepo.AddUserRefreshToken(user.Id, refreshToken);

        return (
                tokens,
                new UserDto()
                {
                    Id = user.Id,
                    Email = user.Email
                }
            );
    }

    public async Task<(TokensDto Tokens, UserDto User)?> RefreshTokens(string refreshToken)
    {
        if (refreshToken is null)
            return null;

        var hashedToken = _tokenHasher.Hash(refreshToken);
        var savedToken = await _refreshTokensRepo.FindHashedToken(hashedToken);
        if (savedToken is null)
            return null;

        if (savedToken.TokenExpiresAt < DateTime.UtcNow)
            return null;

        var user = savedToken.User;

        var tokens = GenerateTokens(user);
        await _refreshTokensRepo.AddUserRefreshToken(user.Id, _tokenHasher.Hash(tokens.RefreshToken));

        return (
            tokens,
            new UserDto()
            {
                Id = user.Id,
                Email = user.Email
            }
        );
    }

    private TokensDto GenerateTokens(User user) =>
        new TokensDto()
        {
            AccessToken = _tokenService.GenerateAccessToken(user),
            RefreshToken = _tokenService.GenerateRefreshToken(),
        };
}
