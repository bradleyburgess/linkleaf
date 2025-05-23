using System.Net;
using LinkLeaf.Api.DTOs;
using LinkLeaf.Api.DTOs.Auth;
using LinkLeaf.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace LinkLeaf.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService, IConfiguration config) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    private readonly int refreshTokenExpirationMinutes = config.GetValue<int>("Jwt:refreshTokenExpirationMinutes");

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterRequestDto request)
    {
        request.Email = request.Email.ToLower();

        // TODO: Make `ModelState` errors consistent with `ApiResponse`
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.Register(request);
        if (result is null)
            return BadRequest(
                ApiResponse<AuthResponseDto>.FailureResponse("Email already exists")
            );

        var (tokens, user) = result.Value;


        var response = new AuthResponseDto()
        {
            AccessToken = tokens.AccessToken,
            User = user
        };
        SetRefreshTokenCookie(Response, tokens.RefreshToken);

        return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(data: response));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginRequestDto request)
    {
        request.Email = request.Email.ToLower();

        // TODO: Make `ModelState` errors consistent with `ApiResponse`
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _authService.Login(request);
        if (result is null)
            return BadRequest(
                ApiResponse<AuthResponseDto>.FailureResponse("Invalid credentials")
            );

        var (tokens, user) = result.Value;

        var response = new AuthResponseDto()
        {
            AccessToken = tokens.AccessToken,
            User = user
        };

        SetRefreshTokenCookie(Response, tokens.RefreshToken);

        return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(data: response));
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<AuthResponseDto?>> RefreshToken()
    {
        var token = Request.Cookies["refreshToken"];
        if (token is null)
            return BadRequest(ApiResponse<AuthResponseDto>.FailureResponse("Invalid refresh token"));
        var result = await _authService.RefreshTokens(token);
        if (result is null)
            return BadRequest(ApiResponse<AuthResponseDto>.FailureResponse("Invalid refresh token"));
            
        var (tokens, userDto) = result.Value;

        var response = new AuthResponseDto()
        {
            AccessToken = tokens.AccessToken,
            User = userDto
        };

        SetRefreshTokenCookie(Response, tokens.RefreshToken);

        return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(data: response));
    }

    private void SetRefreshTokenCookie(HttpResponse response, string refreshToken)
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
