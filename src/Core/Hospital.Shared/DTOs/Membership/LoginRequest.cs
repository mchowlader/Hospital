using System.ComponentModel.DataAnnotations;

namespace Hospital.Shared.DTOs.Membership;

public class LoginRequest
{
    [Required(ErrorMessage = "Username or Email is required")]
    public string UsernameOrEmail { get; set; } = default!;

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = default!;
}
