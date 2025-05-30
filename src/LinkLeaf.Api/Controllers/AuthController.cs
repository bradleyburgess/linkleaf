using LinkLeaf.Api.DTOs;
using LinkLeaf.Api.DTOs.Auth;
using LinkLeaf.Api.Options;
using LinkLeaf.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using LinkLeaf.Api.Utils;

namespace LinkLeaf.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService, IOptions<JwtOptions> jwtOptions) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    private readonly int refreshTokenExpirationMinutes = jwtOptions.Value.RefreshTokenExpirationMinutes;



    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(
        RegisterRequestDto request,
        [FromHeader(Name = "X-Client")] string? xClient = ""
    )
    {
        request.Email = request.Email.ToLower();

        if (!ModelState.IsValid)
        {
            var errors = ControllerUtils.GetModelErrors(ModelState);
            return BadRequest(ApiResponse<AuthResponseDto>.FailureResponse(
                code: ApiStatusCode.VALIDATION_ERROR,
                errors: errors
            ));
        }

        var result = await _authService.Register(request);
        if (result is null)
            return BadRequest(
                ApiResponse<AuthResponseDto>.FailureResponse(
                    code: ApiStatusCode.USER_EXISTS,
                    message: "Email address already exists"
                )
            );

        var (tokens, user) = result.Value;


        var response = new AuthResponseDto()
        {
            AccessToken = tokens.AccessToken,
            User = user
        };
        if (string.Equals(xClient, "mobile", StringComparison.CurrentCultureIgnoreCase))
            response.RefreshToken = tokens.RefreshToken;
        SetRefreshTokenCookie(Response, tokens.RefreshToken);

        return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(data: response));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(
        LoginRequestDto request,
        [FromHeader(Name = "X-Client")] string? xClient = ""
    )
    {
        request.Email = request.Email.ToLower();

        if (!ModelState.IsValid)
        {
            var errors = ControllerUtils.GetModelErrors(ModelState);
            return BadRequest(ApiResponse<AuthResponseDto>.FailureResponse(
                code: ApiStatusCode.VALIDATION_ERROR,
                errors: errors
            ));
        }

        var result = await _authService.Login(request);
        if (result is null)
            return BadRequest(
                ApiResponse<AuthResponseDto>.FailureResponse(
                    code: ApiStatusCode.INVALID_CREDENTIALS
                )
            );

        var (tokens, user) = result.Value;

        var response = new AuthResponseDto()
        {
            AccessToken = tokens.AccessToken,
            User = user
        };
        if (string.Equals(xClient, "mobile", StringComparison.CurrentCultureIgnoreCase))
            response.RefreshToken = tokens.RefreshToken;

        SetRefreshTokenCookie(Response, tokens.RefreshToken);

        return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(data: response));
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<AuthResponseDto?>> RefreshToken(
        [FromHeader(Name = "X-Refresh-Token")] string? refreshTokenHeader,
        [FromHeader(Name = "X-Client")] string? xClient = ""
    )
    {
        Request.Cookies.TryGetValue("refreshToken", out var refreshTokenCookie);
        var token = refreshTokenCookie ?? refreshTokenHeader;

        if (token is null)
            return BadRequest(ApiResponse<AuthResponseDto>.FailureResponse(
                code: ApiStatusCode.TOKEN_INVALID_OR_EXPIRED
            ));
        var result = await _authService.RefreshTokens(token);
        if (result is null)
            return BadRequest(ApiResponse<AuthResponseDto>.FailureResponse(
                code: ApiStatusCode.TOKEN_INVALID_OR_EXPIRED,
                message: "Invalid or expired token"
            ));

        var (tokens, userDto) = result.Value;

        var response = new AuthResponseDto()
        {
            AccessToken = tokens.AccessToken,
            User = userDto
        };
        if (string.Equals(xClient, "mobile", StringComparison.CurrentCultureIgnoreCase))
            response.RefreshToken = tokens.RefreshToken;

        SetRefreshTokenCookie(Response, tokens.RefreshToken);

        return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(data: response));
    }

    private void SetRefreshTokenCookie(
        HttpResponse response,
        string refreshToken
    )
    {
        response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddMinutes(refreshTokenExpirationMinutes)
        });
    }
}
