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
    IUsersRepository usersRepository,
    ITokenHasher tokenHasher,
    IOptions<JwtOptions> jwtOptions
) : IAuthService
{
    private readonly IJwtService _tokenService = tokenService;
    private readonly IUsersRepository _usersRepo = usersRepository;
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

        await _usersRepo.UpdateRefreshToken(
            id: user.Id,
            token: _tokenHasher.Hash(tokens.RefreshToken),
            expirationDate: DateTime.UtcNow.AddMinutes(_refreshTokenExpirationMinutes)
        );

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
        newUser.RefreshToken = _tokenHasher.Hash(tokens.RefreshToken);
        newUser.RefreshTokenExpiration = DateTime.UtcNow.AddMinutes(_refreshTokenExpirationMinutes);

        var user = await _usersRepo.AddAsync(newUser);
        if (user is null)
            return null;

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
        var user = await _usersRepo.FindByRefreshToken(hashedToken);
        if (user is null)
            return null;

        if (
            user.RefreshTokenExpiration < DateTime.UtcNow ||
            user.RefreshToken != hashedToken
        )
            return null;


        var tokens = GenerateTokens(user);
        await _usersRepo.UpdateRefreshToken(
            id: user.Id,
            refreshToken = _tokenHasher.Hash(tokens.RefreshToken),
            expirationDate: DateTime.UtcNow.AddMinutes(_refreshTokenExpirationMinutes)
        );

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
