using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using LinkLeaf.Api.Controllers;
using LinkLeaf.Api.DTOs;
using LinkLeaf.Api.DTOs.Auth;
using LinkLeaf.Api.DTOs.User;
using LinkLeaf.Api.Options;
using LinkLeaf.Api.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;

namespace LinkLeaf.Api.Tests.Controllers;

public class AuthControllerRegisterTests
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly IOptions<JwtOptions> _jwtOptions;
    private readonly AuthController _controller;

    public AuthControllerRegisterTests()
    {
        _authServiceMock = new Mock<IAuthService>();

        var config = new Dictionary<string, string?>
        {
            { "Jwt:RefreshTokenExpirationMinutes", "60" }
        };
        _jwtOptions = Microsoft.Extensions.Options.Options.Create<JwtOptions>(new JwtOptions());

        _controller = new AuthController(_authServiceMock.Object, _jwtOptions);

        // Required to set cookies in the controller
        var httpContext = new DefaultHttpContext();
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
    }

    [Fact]
    public async Task Register_ReturnsOk_WhenRegistrationSucceeds()
    {
        // Arrange
        var request = new RegisterRequestDto { Email = "Test@Example.com", Password = "abc", PasswordConfirm = "abc" };

        var tokens = new TokensDto { AccessToken = "access", RefreshToken = "refresh" };
        var user = new UserDto { Email = "test@example.com" };

        _authServiceMock
            .Setup(s => s.Register(It.IsAny<RegisterRequestDto>()))
            .ReturnsAsync((tokens, user));

        // Act
        var result = await _controller.Register(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ApiResponse<AuthResponseDto>>(okResult.Value)!;

        Assert.True(response.Success);
        Assert.Equal("access", response.Data?.AccessToken);
        Assert.Equal("test@example.com", response.Data?.User.Email);
        Assert.Equal(200, okResult.StatusCode ?? 200);
    }

    [Fact]
    public async Task Register_ReturnsBadRequest_WhenPasswordMissing()
    {
        // Arrange
        var request = new RegisterRequestDto { Email = "Test@Example.com" };

        ValidateModel(request); // simulates ASP.NET Core's validation pipeline

        // Act
        var result = await _controller.Register(request);

        // Assert
        var badResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var badResultData = Assert.IsType<ApiResponse<AuthResponseDto>>(badResult.Value);
        Assert.Equal(400, badResult.StatusCode);
        Assert.Equal(ApiStatusCode.VALIDATION_ERROR, badResultData.Code);
    }

    [Fact]
    public async Task Register_ReturnsBadRequest_WhenPasswordsDontMatch()
    {
        // Arrange
        var request = new RegisterRequestDto { Email = "Test@Example.com", Password = "abc", PasswordConfirm = "123" };

        ValidateModel(request);

        // Act
        var result = await _controller.Register(request);

        // Assert
        var badResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var badResultData = Assert.IsType<ApiResponse<AuthResponseDto>>(badResult.Value);
        Assert.Equal(400, badResult.StatusCode);
        Assert.Equal(ApiStatusCode.VALIDATION_ERROR, badResultData.Code);
    }

    [Fact]
    public async Task Register_ReturnsBadRequest_WhenNotValidEmail()
    {
        // Arrange
        var request = new RegisterRequestDto { Email = "Test@Example", Password = "abc", PasswordConfirm = "abc" };

        ValidateModel(request);
        Assert.False(_controller.ModelState.IsValid);
        Assert.True(_controller.ModelState.ContainsKey(nameof(request.Email)));

        // Act
        var result = await _controller.Register(request);

        // Assert
        var badResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        var badResultData = Assert.IsType<ApiResponse<AuthResponseDto>>(badResult.Value);
        Assert.Equal(400, badResult.StatusCode);
        Assert.Equal(ApiStatusCode.VALIDATION_ERROR, badResultData.Code);
    }

    private void ValidateModel(object model)
    {
        var validationContext = new ValidationContext(model);
        var validationResults = new List<ValidationResult>();
        Validator.TryValidateObject(model, validationContext, validationResults, validateAllProperties: true);

        foreach (var validationResult in validationResults)
        {
            foreach (var memberName in validationResult.MemberNames)
            {
                _controller.ModelState.AddModelError(memberName, validationResult.ErrorMessage!);
            }
        }
    }
}
