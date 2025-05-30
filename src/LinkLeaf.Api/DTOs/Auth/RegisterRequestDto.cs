using System.ComponentModel.DataAnnotations;

namespace LinkLeaf.Api.DTOs.Auth;

public class RegisterRequestDto
{
    [Required]
    [RegularExpression(
      @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
      ErrorMessage = "Email must be in the form user@domain.tld"
    )]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Compare("Password")]
    public string PasswordConfirm { get; set; } = string.Empty;
}
